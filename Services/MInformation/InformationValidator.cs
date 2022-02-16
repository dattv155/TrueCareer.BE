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

namespace TrueCareer.Services.MInformation
{
    public interface IInformationValidator : IServiceScoped
    {
        Task Get(Information Information);
        Task<bool> Create(Information Information);
        Task<bool> Update(Information Information);
        Task<bool> Delete(Information Information);
        Task<bool> BulkDelete(List<Information> Information);
        Task<bool> Import(List<Information> Information);
    }

    public class InformationValidator : IInformationValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private InformationMessage InformationMessage;

        public InformationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.InformationMessage = new InformationMessage();
        }

        public async Task Get(Information Information)
        {
        }

        public async Task<bool> Create(Information Information)
        {
            await ValidateName(Information);
            await ValidateDescription(Information);
            await ValidateStartAt(Information);
            await ValidateRole(Information);
            await ValidateImage(Information);
            await ValidateEndAt(Information);
            await ValidateInformationType(Information);
            await ValidateTopic(Information);
            await ValidateUser(Information);
            return Information.IsValidated;
        }

        public async Task<bool> Update(Information Information)
        {
            if (await ValidateId(Information))
            {
                await ValidateName(Information);
                await ValidateDescription(Information);
                await ValidateStartAt(Information);
                await ValidateRole(Information);
                await ValidateImage(Information);
                await ValidateEndAt(Information);
                await ValidateInformationType(Information);
                await ValidateTopic(Information);
                await ValidateUser(Information);
            }
            return Information.IsValidated;
        }

        public async Task<bool> Delete(Information Information)
        {
            if (await ValidateId(Information))
            {
            }
            return Information.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Information> Information)
        {
            foreach (Information Information in Information)
            {
                await Delete(Information);
            }
            return Information.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Information> Information)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Information Information)
        {
            InformationFilter InformationFilter = new InformationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Information.Id },
                Selects = InformationSelect.Id
            };

            int count = await UOW.InformationRepository.CountAll(InformationFilter);
            if (count == 0)
                Information.AddError(nameof(InformationValidator), nameof(Information.Id), InformationMessage.Error.IdNotExisted, InformationMessage);
            return Information.IsValidated;
        }

        private async Task<bool> ValidateName(Information Information)
        {
            if(string.IsNullOrEmpty(Information.Name))
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.Name), InformationMessage.Error.NameEmpty, InformationMessage);
            }
            else if(Information.Name.Count() > 500)
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.Name), InformationMessage.Error.NameOverLength, InformationMessage);
            }
            return Information.IsValidated;
        }
        private async Task<bool> ValidateDescription(Information Information)
        {
            if(string.IsNullOrEmpty(Information.Description))
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.Description), InformationMessage.Error.DescriptionEmpty, InformationMessage);
            }
            else if(Information.Description.Count() > 500)
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.Description), InformationMessage.Error.DescriptionOverLength, InformationMessage);
            }
            return Information.IsValidated;
        }
        private async Task<bool> ValidateStartAt(Information Information)
        {       
            if(Information.StartAt <= new DateTime(2000, 1, 1))
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.StartAt), InformationMessage.Error.StartAtEmpty, InformationMessage);
            }
            return true;
        }
        private async Task<bool> ValidateRole(Information Information)
        {
            if(string.IsNullOrEmpty(Information.Role))
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.Role), InformationMessage.Error.RoleEmpty, InformationMessage);
            }
            else if(Information.Role.Count() > 500)
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.Role), InformationMessage.Error.RoleOverLength, InformationMessage);
            }
            return Information.IsValidated;
        }
        private async Task<bool> ValidateImage(Information Information)
        {
            if(string.IsNullOrEmpty(Information.Image))
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.Image), InformationMessage.Error.ImageEmpty, InformationMessage);
            }
            else if(Information.Image.Count() > 500)
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.Image), InformationMessage.Error.ImageOverLength, InformationMessage);
            }
            return Information.IsValidated;
        }
        private async Task<bool> ValidateEndAt(Information Information)
        {       
            if(Information.EndAt <= new DateTime(2000, 1, 1))
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.EndAt), InformationMessage.Error.EndAtEmpty, InformationMessage);
            }
            return true;
        }
        private async Task<bool> ValidateInformationType(Information Information)
        {       
            if(Information.InformationTypeId == 0)
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.InformationType), InformationMessage.Error.InformationTypeEmpty, InformationMessage);
            }
            else
            {
                if(!InformationTypeEnum.InformationTypeEnumList.Any(x => Information.InformationTypeId == x.Id))
                {
                    Information.AddError(nameof(InformationValidator), nameof(Information.InformationType), InformationMessage.Error.InformationTypeNotExisted, InformationMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateTopic(Information Information)
        {       
            if(Information.TopicId == 0)
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.Topic), InformationMessage.Error.TopicEmpty, InformationMessage);
            }
            else
            {
                int count = await UOW.TopicRepository.CountAll(new TopicFilter
                {
                    Id = new IdFilter{ Equal =  Information.TopicId },
                });
                if(count == 0)
                {
                    Information.AddError(nameof(InformationValidator), nameof(Information.Topic), InformationMessage.Error.TopicNotExisted, InformationMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateUser(Information Information)
        {       
            if(Information.UserId == 0)
            {
                Information.AddError(nameof(InformationValidator), nameof(Information.User), InformationMessage.Error.UserEmpty, InformationMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  Information.UserId },
                });
                if(count == 0)
                {
                    Information.AddError(nameof(InformationValidator), nameof(Information.User), InformationMessage.Error.UserNotExisted, InformationMessage);
                }
            }
            return true;
        }
    }
}
