using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer;
using TrueCareer.Common;
using TrueCareer.Enums;
using TrueCareer.Entities;
using TrueCareer.Repositories;

namespace TrueCareer.Services.MQuestion
{
    public interface IQuestionValidator : IServiceScoped
    {
        Task Get(Question Question);
        Task<bool> Create(Question Question);
        Task<bool> Update(Question Question);
        Task<bool> Delete(Question Question);
        Task<bool> BulkDelete(List<Question> Questions);
        Task<bool> Import(List<Question> Questions);
    }

    public class QuestionValidator : IQuestionValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private QuestionMessage QuestionMessage;

        public QuestionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.QuestionMessage = new QuestionMessage();
        }

        public async Task Get(Question Question)
        {
        }

        public async Task<bool> Create(Question Question)
        {
            await ValidateQuestionContent(Question);
            await ValidateDescription(Question);
            await ValidateChoices(Question);
            return Question.IsValidated;
        }

        public async Task<bool> Update(Question Question)
        {
            if (await ValidateId(Question))
            {
                await ValidateQuestionContent(Question);
                await ValidateDescription(Question);
                await ValidateChoices(Question);
            }
            return Question.IsValidated;
        }

        public async Task<bool> Delete(Question Question)
        {
            if (await ValidateId(Question))
            {
            }
            return Question.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Question> Questions)
        {
            foreach (Question Question in Questions)
            {
                await Delete(Question);
            }
            return Questions.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Question> Questions)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Question Question)
        {
            QuestionFilter QuestionFilter = new QuestionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Question.Id },
                Selects = QuestionSelect.Id
            };

            int count = await UOW.QuestionRepository.CountAll(QuestionFilter);
            if (count == 0)
                Question.AddError(nameof(QuestionValidator), nameof(Question.Id), QuestionMessage.Error.IdNotExisted, QuestionMessage);
            return Question.IsValidated;
        }

        private async Task<bool> ValidateQuestionContent(Question Question)
        {
            if(string.IsNullOrEmpty(Question.QuestionContent))
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.QuestionContent), QuestionMessage.Error.QuestionContentEmpty, QuestionMessage);
            }
            else if(Question.QuestionContent.Count() > 500)
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.QuestionContent), QuestionMessage.Error.QuestionContentOverLength, QuestionMessage);
            }
            return Question.IsValidated;
        }
        private async Task<bool> ValidateDescription(Question Question)
        {
            if(string.IsNullOrEmpty(Question.Description))
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Description), QuestionMessage.Error.DescriptionEmpty, QuestionMessage);
            }
            else if(Question.Description.Count() > 500)
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Description), QuestionMessage.Error.DescriptionOverLength, QuestionMessage);
            }
            return Question.IsValidated;
        }
        private async Task<bool> ValidateChoices(Question Question)
        {   
            if(Question.Choices?.Any() ?? false)
            {
                #region fetch data
                List<long> MbtiSingleTypeIds = new List<long>();
                MbtiSingleTypeIds.AddRange(Question.Choices.Select(x => x.MbtiSingleTypeId));
                List<MbtiSingleType> MbtiSingleTypes = await UOW.MbtiSingleTypeRepository.List(new MbtiSingleTypeFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = MbtiSingleTypeSelect.Id,

                    Id = new IdFilter { In = MbtiSingleTypeIds },
                });
                #endregion

                #region validate
                foreach(Choice Choice in Question.Choices)
                {
                    if(string.IsNullOrEmpty(Choice.ChoiceContent))
                    {
                        Choice.AddError(nameof(QuestionValidator), nameof(Choice.ChoiceContent), QuestionMessage.Error.Choice_ChoiceContentEmpty, QuestionMessage);
                    }
                    else if(Choice.ChoiceContent.Count() > 500)
                    {
                        Choice.AddError(nameof(QuestionValidator), nameof(Choice.ChoiceContent), QuestionMessage.Error.Choice_ChoiceContentOverLength, QuestionMessage);
                    }

                    if(string.IsNullOrEmpty(Choice.Description))
                    {
                        Choice.AddError(nameof(QuestionValidator), nameof(Choice.Description), QuestionMessage.Error.Choice_DescriptionEmpty, QuestionMessage);
                    }
                    else if(Choice.Description.Count() > 500)
                    {
                        Choice.AddError(nameof(QuestionValidator), nameof(Choice.Description), QuestionMessage.Error.Choice_DescriptionOverLength, QuestionMessage);
                    }

                    if(Choice.MbtiSingleTypeId == 0)
                    {
                        Choice.AddError(nameof(QuestionValidator), nameof(Choice.MbtiSingleType), QuestionMessage.Error.Choice_MbtiSingleTypeEmpty, QuestionMessage);
                    }
                    else
                    {
                        MbtiSingleType MbtiSingleType = MbtiSingleTypes.FirstOrDefault(x => x.Id == Choice.MbtiSingleTypeId);
                        if(MbtiSingleType == null)
                        {
                            Choice.AddError(nameof(QuestionValidator), nameof(Choice.MbtiSingleType), QuestionMessage.Error.Choice_MbtiSingleTypeNotExisted, QuestionMessage);
                        }
                    }
                    
                }
                #endregion
            }
            else 
            {

            }
            return true;
        }
    }
}
