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

namespace TrueCareer.Services.MChoice
{
    public interface IChoiceValidator : IServiceScoped
    {
        Task Get(Choice Choice);
        Task<bool> Create(Choice Choice);
        Task<bool> Update(Choice Choice);
        Task<bool> Delete(Choice Choice);
        Task<bool> BulkDelete(List<Choice> Choices);
        Task<bool> Import(List<Choice> Choices);
    }

    public class ChoiceValidator : IChoiceValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private ChoiceMessage ChoiceMessage;

        public ChoiceValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ChoiceMessage = new ChoiceMessage();
        }

        public async Task Get(Choice Choice)
        {
        }

        public async Task<bool> Create(Choice Choice)
        {
            await ValidateChoiceContent(Choice);
            await ValidateDescription(Choice);
            await ValidateMbtiSingleType(Choice);
            await ValidateQuestion(Choice);
            return Choice.IsValidated;
        }

        public async Task<bool> Update(Choice Choice)
        {
            if (await ValidateId(Choice))
            {
                await ValidateChoiceContent(Choice);
                await ValidateDescription(Choice);
                await ValidateMbtiSingleType(Choice);
                await ValidateQuestion(Choice);
            }
            return Choice.IsValidated;
        }

        public async Task<bool> Delete(Choice Choice)
        {
            if (await ValidateId(Choice))
            {
            }
            return Choice.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Choice> Choices)
        {
            foreach (Choice Choice in Choices)
            {
                await Delete(Choice);
            }
            return Choices.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Choice> Choices)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Choice Choice)
        {
            ChoiceFilter ChoiceFilter = new ChoiceFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Choice.Id },
                Selects = ChoiceSelect.Id
            };

            int count = await UOW.ChoiceRepository.CountAll(ChoiceFilter);
            if (count == 0)
                Choice.AddError(nameof(ChoiceValidator), nameof(Choice.Id), ChoiceMessage.Error.IdNotExisted, ChoiceMessage);
            return Choice.IsValidated;
        }

        private async Task<bool> ValidateChoiceContent(Choice Choice)
        {
            if(string.IsNullOrEmpty(Choice.ChoiceContent))
            {
                Choice.AddError(nameof(ChoiceValidator), nameof(Choice.ChoiceContent), ChoiceMessage.Error.ChoiceContentEmpty, ChoiceMessage);
            }
            else if(Choice.ChoiceContent.Count() > 500)
            {
                Choice.AddError(nameof(ChoiceValidator), nameof(Choice.ChoiceContent), ChoiceMessage.Error.ChoiceContentOverLength, ChoiceMessage);
            }
            return Choice.IsValidated;
        }
        private async Task<bool> ValidateDescription(Choice Choice)
        {
            if(string.IsNullOrEmpty(Choice.Description))
            {
                Choice.AddError(nameof(ChoiceValidator), nameof(Choice.Description), ChoiceMessage.Error.DescriptionEmpty, ChoiceMessage);
            }
            else if(Choice.Description.Count() > 500)
            {
                Choice.AddError(nameof(ChoiceValidator), nameof(Choice.Description), ChoiceMessage.Error.DescriptionOverLength, ChoiceMessage);
            }
            return Choice.IsValidated;
        }
        private async Task<bool> ValidateMbtiSingleType(Choice Choice)
        {       
            if(Choice.MbtiSingleTypeId == 0)
            {
                Choice.AddError(nameof(ChoiceValidator), nameof(Choice.MbtiSingleType), ChoiceMessage.Error.MbtiSingleTypeEmpty, ChoiceMessage);
            }
            else
            {
                if(!MbtiSingleTypeEnum.MbtiSingleTypeEnumList.Any(x => Choice.MbtiSingleTypeId == x.Id))
                {
                    Choice.AddError(nameof(ChoiceValidator), nameof(Choice.MbtiSingleType), ChoiceMessage.Error.MbtiSingleTypeNotExisted, ChoiceMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateQuestion(Choice Choice)
        {       
            if(Choice.QuestionId == 0)
            {
                Choice.AddError(nameof(ChoiceValidator), nameof(Choice.Question), ChoiceMessage.Error.QuestionEmpty, ChoiceMessage);
            }
            else
            {
                int count = await UOW.QuestionRepository.CountAll(new QuestionFilter
                {
                    Id = new IdFilter{ Equal =  Choice.QuestionId },
                });
                if(count == 0)
                {
                    Choice.AddError(nameof(ChoiceValidator), nameof(Choice.Question), ChoiceMessage.Error.QuestionNotExisted, ChoiceMessage);
                }
            }
            return true;
        }
    }
}
