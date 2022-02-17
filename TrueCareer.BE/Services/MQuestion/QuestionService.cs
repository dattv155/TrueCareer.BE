using TrueSight.Common;
using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using TrueCareer.Repositories;
using TrueCareer.Entities;
using TrueCareer.Enums;

namespace TrueCareer.Services.MQuestion
{
    public interface IQuestionService :  IServiceScoped
    {
        Task<int> Count(QuestionFilter QuestionFilter);
        Task<List<Question>> List(QuestionFilter QuestionFilter);
        Task<Question> Get(long Id);
        Task<Question> Create(Question Question);
        Task<Question> Update(Question Question);
        Task<Question> Delete(Question Question);
        Task<List<Question>> BulkDelete(List<Question> Questions);
        Task<List<Question>> Import(List<Question> Questions);
        Task<QuestionFilter> ToFilter(QuestionFilter QuestionFilter);
    }

    public class QuestionService : BaseService, IQuestionService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IQuestionValidator QuestionValidator;

        public QuestionService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IQuestionValidator QuestionValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.QuestionValidator = QuestionValidator;
        }
        public async Task<int> Count(QuestionFilter QuestionFilter)
        {
            try
            {
                int result = await UOW.QuestionRepository.Count(QuestionFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return 0;
        }

        public async Task<List<Question>> List(QuestionFilter QuestionFilter)
        {
            try
            {
                List<Question> Questions = await UOW.QuestionRepository.List(QuestionFilter);
                return Questions;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }

        public async Task<Question> Get(long Id)
        {
            Question Question = await UOW.QuestionRepository.Get(Id);
            await QuestionValidator.Get(Question);
            if (Question == null)
                return null;
            return Question;
        }
        
        public async Task<Question> Create(Question Question)
        {
            if (!await QuestionValidator.Create(Question))
                return Question;

            try
            {
                await UOW.QuestionRepository.Create(Question);
                Question = await UOW.QuestionRepository.Get(Question.Id);
                Logging.CreateAuditLog(Question, new { }, nameof(QuestionService));
                return Question;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }

        public async Task<Question> Update(Question Question)
        {
            if (!await QuestionValidator.Update(Question))
                return Question;
            try
            {
                var oldData = await UOW.QuestionRepository.Get(Question.Id);

                await UOW.QuestionRepository.Update(Question);

                Question = await UOW.QuestionRepository.Get(Question.Id);
                Logging.CreateAuditLog(Question, oldData, nameof(QuestionService));
                return Question;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }

        public async Task<Question> Delete(Question Question)
        {
            if (!await QuestionValidator.Delete(Question))
                return Question;

            try
            {
                await UOW.QuestionRepository.Delete(Question);
                Logging.CreateAuditLog(new { }, Question, nameof(QuestionService));
                return Question;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }

        public async Task<List<Question>> BulkDelete(List<Question> Questions)
        {
            if (!await QuestionValidator.BulkDelete(Questions))
                return Questions;

            try
            {
                await UOW.QuestionRepository.BulkDelete(Questions);
                Logging.CreateAuditLog(new { }, Questions, nameof(QuestionService));
                return Questions;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;

        }
        
        public async Task<List<Question>> Import(List<Question> Questions)
        {
            if (!await QuestionValidator.Import(Questions))
                return Questions;
            try
            {
                await UOW.QuestionRepository.BulkMerge(Questions);

                Logging.CreateAuditLog(Questions, new { }, nameof(QuestionService));
                return Questions;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }     
        
        public async Task<QuestionFilter> ToFilter(QuestionFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<QuestionFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                QuestionFilter subFilter = new QuestionFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.QuestionContent))
                        subFilter.QuestionContent = FilterBuilder.Merge(subFilter.QuestionContent, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = FilterBuilder.Merge(subFilter.Description, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private void Sync(List<Question> Questions)
        {
            List<Choice> Choices = new List<Choice>();
            List<MbtiSingleType> MbtiSingleTypes = new List<MbtiSingleType>();
            Choices.AddRange(Questions.SelectMany(x => x.Choices.Select(y => new Choice { Id = y.Id })));
            MbtiSingleTypes.AddRange(Questions.SelectMany(x => x.Choices.Select(y => new MbtiSingleType { Id = y.MbtiSingleTypeId })));
            
            Choices = Choices.Distinct().ToList();
            MbtiSingleTypes = MbtiSingleTypes.Distinct().ToList();
            RabbitManager.PublishList(Choices, RoutingKeyEnum.ChoiceUsed.Code);
            RabbitManager.PublishList(MbtiSingleTypes, RoutingKeyEnum.MbtiSingleTypeUsed.Code);
        }

    }
}
