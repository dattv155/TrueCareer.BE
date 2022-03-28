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

namespace TrueCareer.Services.MConversationConfiguration
{
    public interface IConversationConfigurationValidator : IServiceScoped
    {
        Task Get(ConversationConfiguration ConversationConfiguration);
        Task<bool> Create(ConversationConfiguration ConversationConfiguration);
        Task<bool> Update(ConversationConfiguration ConversationConfiguration);
        Task<bool> Delete(ConversationConfiguration ConversationConfiguration);
        Task<bool> BulkDelete(List<ConversationConfiguration> ConversationConfigurations);
        Task<bool> Import(List<ConversationConfiguration> ConversationConfigurations);
    }

    public class ConversationConfigurationValidator : IConversationConfigurationValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private ConversationConfigurationMessage ConversationConfigurationMessage;

        public ConversationConfigurationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ConversationConfigurationMessage = new ConversationConfigurationMessage();
        }

        public async Task Get(ConversationConfiguration ConversationConfiguration)
        {
        }

        public async Task<bool> Create(ConversationConfiguration ConversationConfiguration)
        {
            await ValidateAppSecret(ConversationConfiguration);
            await ValidateAppName(ConversationConfiguration);
            await ValidateOASecretKey(ConversationConfiguration);
            await ValidateExpiredAt(ConversationConfiguration);
            await ValidateConversationType(ConversationConfiguration);
            return ConversationConfiguration.IsValidated;
        }

        public async Task<bool> Update(ConversationConfiguration ConversationConfiguration)
        {
            if (await ValidateId(ConversationConfiguration))
            {
                await ValidateAppSecret(ConversationConfiguration);
                await ValidateAppName(ConversationConfiguration);
                await ValidateOASecretKey(ConversationConfiguration);
                await ValidateExpiredAt(ConversationConfiguration);
                await ValidateConversationType(ConversationConfiguration);
            }
            return ConversationConfiguration.IsValidated;
        }

        public async Task<bool> Delete(ConversationConfiguration ConversationConfiguration)
        {
            if (await ValidateId(ConversationConfiguration))
            {
            }
            return ConversationConfiguration.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ConversationConfiguration> ConversationConfigurations)
        {
            foreach (ConversationConfiguration ConversationConfiguration in ConversationConfigurations)
            {
                await Delete(ConversationConfiguration);
            }
            return ConversationConfigurations.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<ConversationConfiguration> ConversationConfigurations)
        {
            return true;
        }
        
        public async Task<bool> ValidateId(ConversationConfiguration ConversationConfiguration)
        {
            ConversationConfigurationFilter ConversationConfigurationFilter = new ConversationConfigurationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ConversationConfiguration.Id },
                Selects = ConversationConfigurationSelect.Id
            };

            int count = await UOW.ConversationConfigurationRepository.Count(ConversationConfigurationFilter);
            if (count == 0)
                ConversationConfiguration.AddError(nameof(ConversationConfigurationValidator), nameof(ConversationConfiguration.Id), ConversationConfigurationMessage.Error.IdNotExisted, ConversationConfigurationMessage);
            return count == 1;
        }

        public async Task<bool> ValidateAppSecret(ConversationConfiguration ConversationConfiguration)
        {
            if(string.IsNullOrEmpty(ConversationConfiguration.AppSecret))
            {
                ConversationConfiguration.AddError(nameof(ConversationConfigurationValidator), nameof(ConversationConfiguration.AppSecret), ConversationConfigurationMessage.Error.AppSecretEmpty, ConversationConfigurationMessage);
            }
            else if(ConversationConfiguration.AppSecret.Count() > 500)
            {
                ConversationConfiguration.AddError(nameof(ConversationConfigurationValidator), nameof(ConversationConfiguration.AppSecret), ConversationConfigurationMessage.Error.AppSecretOverLength, ConversationConfigurationMessage);
            }
            return true;
        }
        public async Task<bool> ValidateAppName(ConversationConfiguration ConversationConfiguration)
        {
            if(string.IsNullOrEmpty(ConversationConfiguration.AppName))
            {
                ConversationConfiguration.AddError(nameof(ConversationConfigurationValidator), nameof(ConversationConfiguration.AppName), ConversationConfigurationMessage.Error.AppNameEmpty, ConversationConfigurationMessage);
            }
            else if(ConversationConfiguration.AppName.Count() > 500)
            {
                ConversationConfiguration.AddError(nameof(ConversationConfigurationValidator), nameof(ConversationConfiguration.AppName), ConversationConfigurationMessage.Error.AppNameOverLength, ConversationConfigurationMessage);
            }
            return true;
        }
        public async Task<bool> ValidateOASecretKey(ConversationConfiguration ConversationConfiguration)
        {
            if(string.IsNullOrEmpty(ConversationConfiguration.OaSecretKey))
            {
                ConversationConfiguration.AddError(nameof(ConversationConfigurationValidator), nameof(ConversationConfiguration.OaSecretKey), ConversationConfigurationMessage.Error.OASecretKeyEmpty, ConversationConfigurationMessage);
            }
            else if(ConversationConfiguration.OaSecretKey.Count() > 500)
            {
                ConversationConfiguration.AddError(nameof(ConversationConfigurationValidator), nameof(ConversationConfiguration.OaSecretKey), ConversationConfigurationMessage.Error.OASecretKeyOverLength, ConversationConfigurationMessage);
            }
            return true;
        }
        public async Task<bool> ValidateExpiredAt(ConversationConfiguration ConversationConfiguration)
        {       
            if(ConversationConfiguration.ExpiredAt <= new DateTime(2000, 1, 1))
            {
                ConversationConfiguration.AddError(nameof(ConversationConfigurationValidator), nameof(ConversationConfiguration.ExpiredAt), ConversationConfigurationMessage.Error.ExpiredAtEmpty, ConversationConfigurationMessage);
            }
            return true;
        }
        public async Task<bool> ValidateConversationType(ConversationConfiguration ConversationConfiguration)
        {       
            if(ConversationConfiguration.ConversationTypeId == 0)
            {
                ConversationConfiguration.AddError(nameof(ConversationConfigurationValidator), nameof(ConversationConfiguration.ConversationType), ConversationConfigurationMessage.Error.ConversationTypeEmpty, ConversationConfigurationMessage);
                return false;
            }

            if(!ConversationTypeEnum.ConversationTypeEnumList.Any(x => ConversationConfiguration.ConversationTypeId == x.Id))
            {
                ConversationConfiguration.AddError(nameof(ConversationConfigurationValidator), nameof(ConversationConfiguration.ConversationType), ConversationConfigurationMessage.Error.ConversationTypeNotExisted, ConversationConfigurationMessage);
                return false;
            }
            return true;
        }
    }
}
