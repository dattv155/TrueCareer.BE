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

namespace TrueCareer.Services.MChoice
{
    public interface IChoiceService :  IServiceScoped
    {
        Task<int> Count(ChoiceFilter ChoiceFilter);
        Task<List<Choice>> List(ChoiceFilter ChoiceFilter);
        Task<Choice> Get(long Id);
        Task<Choice> Create(Choice Choice);
        Task<Choice> Update(Choice Choice);
        Task<Choice> Delete(Choice Choice);
        Task<List<Choice>> BulkDelete(List<Choice> Choices);
        Task<List<Choice>> Import(List<Choice> Choices);
        Task<ChoiceFilter> ToFilter(ChoiceFilter ChoiceFilter);
    }

    public class ChoiceService : BaseService, IChoiceService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IChoiceValidator ChoiceValidator;

        public ChoiceService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IChoiceValidator ChoiceValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.ChoiceValidator = ChoiceValidator;
        }
        public async Task<int> Count(ChoiceFilter ChoiceFilter)
        {
            try
            {
                int result = await UOW.ChoiceRepository.Count(ChoiceFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ChoiceService));
            }
            return 0;
        }

        public async Task<List<Choice>> List(ChoiceFilter ChoiceFilter)
        {
            try
            {
                List<Choice> Choices = await UOW.ChoiceRepository.List(ChoiceFilter);
                return Choices;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ChoiceService));
            }
            return null;
        }

        public async Task<Choice> Get(long Id)
        {
            Choice Choice = await UOW.ChoiceRepository.Get(Id);
            await ChoiceValidator.Get(Choice);
            if (Choice == null)
                return null;
            return Choice;
        }
        
        public async Task<Choice> Create(Choice Choice)
        {
            if (!await ChoiceValidator.Create(Choice))
                return Choice;

            try
            {
                await UOW.ChoiceRepository.Create(Choice);
                Choice = await UOW.ChoiceRepository.Get(Choice.Id);
                Logging.CreateAuditLog(Choice, new { }, nameof(ChoiceService));
                return Choice;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ChoiceService));
            }
            return null;
        }

        public async Task<Choice> Update(Choice Choice)
        {
            if (!await ChoiceValidator.Update(Choice))
                return Choice;
            try
            {
                var oldData = await UOW.ChoiceRepository.Get(Choice.Id);

                await UOW.ChoiceRepository.Update(Choice);

                Choice = await UOW.ChoiceRepository.Get(Choice.Id);
                Logging.CreateAuditLog(Choice, oldData, nameof(ChoiceService));
                return Choice;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ChoiceService));
            }
            return null;
        }

        public async Task<Choice> Delete(Choice Choice)
        {
            if (!await ChoiceValidator.Delete(Choice))
                return Choice;

            try
            {
                await UOW.ChoiceRepository.Delete(Choice);
                Logging.CreateAuditLog(new { }, Choice, nameof(ChoiceService));
                return Choice;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ChoiceService));
            }
            return null;
        }

        public async Task<List<Choice>> BulkDelete(List<Choice> Choices)
        {
            if (!await ChoiceValidator.BulkDelete(Choices))
                return Choices;

            try
            {
                await UOW.ChoiceRepository.BulkDelete(Choices);
                Logging.CreateAuditLog(new { }, Choices, nameof(ChoiceService));
                return Choices;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ChoiceService));
            }
            return null;

        }
        
        public async Task<List<Choice>> Import(List<Choice> Choices)
        {
            if (!await ChoiceValidator.Import(Choices))
                return Choices;
            try
            {
                await UOW.ChoiceRepository.BulkMerge(Choices);

                Logging.CreateAuditLog(Choices, new { }, nameof(ChoiceService));
                return Choices;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ChoiceService));
            }
            return null;
        }     
        
        public async Task<ChoiceFilter> ToFilter(ChoiceFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ChoiceFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ChoiceFilter subFilter = new ChoiceFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ChoiceContent))
                        subFilter.ChoiceContent = FilterBuilder.Merge(subFilter.ChoiceContent, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = FilterBuilder.Merge(subFilter.Description, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.QuestionId))
                        subFilter.QuestionId = FilterBuilder.Merge(subFilter.QuestionId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.MbtiSingleTypeId))
                        subFilter.MbtiSingleTypeId = FilterBuilder.Merge(subFilter.MbtiSingleTypeId, FilterPermissionDefinition.IdFilter);
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

        private void Sync(List<Choice> Choices)
        {
            List<MbtiSingleType> MbtiSingleTypes = new List<MbtiSingleType>();
            List<Question> Questions = new List<Question>();
            MbtiSingleTypes.AddRange(Choices.Select(x => new MbtiSingleType { Id = x.MbtiSingleTypeId }));
            Questions.AddRange(Choices.Select(x => new Question { Id = x.QuestionId }));
            
            MbtiSingleTypes = MbtiSingleTypes.Distinct().ToList();
            Questions = Questions.Distinct().ToList();
            RabbitManager.PublishList(MbtiSingleTypes, RoutingKeyEnum.MbtiSingleTypeUsed.Code);
            RabbitManager.PublishList(Questions, RoutingKeyEnum.QuestionUsed.Code);
        }

    }
}
