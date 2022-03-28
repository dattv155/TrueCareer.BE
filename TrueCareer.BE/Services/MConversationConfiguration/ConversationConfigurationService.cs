using TrueSight.Common;
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


namespace TrueCareer.Services.MConversationConfiguration
{
    public interface IConversationConfigurationService :  IServiceScoped
    {
        Task<int> Count(ConversationConfigurationFilter ConversationConfigurationFilter);
        Task<List<ConversationConfiguration>> List(ConversationConfigurationFilter ConversationConfigurationFilter);
        Task<ConversationConfiguration> Get(long Id);
        Task<ConversationConfiguration> Create(ConversationConfiguration ConversationConfiguration);
        Task<ConversationConfiguration> Update(ConversationConfiguration ConversationConfiguration);
        Task<ConversationConfiguration> Delete(ConversationConfiguration ConversationConfiguration);
    }

    public class ConversationConfigurationService :  IConversationConfigurationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IConversationConfigurationValidator ConversationConfigurationValidator;

        public ConversationConfigurationService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IConversationConfigurationValidator ConversationConfigurationValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.ConversationConfigurationValidator = ConversationConfigurationValidator;
        }
        public async Task<int> Count(ConversationConfigurationFilter ConversationConfigurationFilter)
        {
            try
            {
                int result = await UOW.ConversationConfigurationRepository.Count(ConversationConfigurationFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationConfigurationService));
            }
            return 0;
        }

        public async Task<List<ConversationConfiguration>> List(ConversationConfigurationFilter ConversationConfigurationFilter)
        {
            try
            {
                List<ConversationConfiguration> ConversationConfigurations = await UOW.ConversationConfigurationRepository.List(ConversationConfigurationFilter);
                return ConversationConfigurations;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationConfigurationService));
            }
            return null;
        }

        public async Task<ConversationConfiguration> Get(long Id)
        {
            ConversationConfiguration ConversationConfiguration = await UOW.ConversationConfigurationRepository.Get(Id);
            await ConversationConfigurationValidator.Get(ConversationConfiguration);
            if (ConversationConfiguration == null)
                return null;
            return ConversationConfiguration;
        }
        
        public async Task<ConversationConfiguration> Create(ConversationConfiguration ConversationConfiguration)
        {
            if (!await ConversationConfigurationValidator.Create(ConversationConfiguration))
                return ConversationConfiguration;

            try
            {
                await UOW.ConversationConfigurationRepository.Create(ConversationConfiguration);
                ConversationConfiguration = await UOW.ConversationConfigurationRepository.Get(ConversationConfiguration.Id);
                Logging.CreateAuditLog(ConversationConfiguration, new { }, nameof(ConversationConfigurationService));
                return ConversationConfiguration;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationConfigurationService));
            }
            return null;
        }

        public async Task<ConversationConfiguration> Update(ConversationConfiguration ConversationConfiguration)
        {
            if (!await ConversationConfigurationValidator.Update(ConversationConfiguration))
                return ConversationConfiguration;
            try
            {
                var oldData = await UOW.ConversationConfigurationRepository.Get(ConversationConfiguration.Id);

                await UOW.ConversationConfigurationRepository.Update(ConversationConfiguration);

                ConversationConfiguration = await UOW.ConversationConfigurationRepository.Get(ConversationConfiguration.Id);
                Logging.CreateAuditLog(ConversationConfiguration, oldData, nameof(ConversationConfigurationService));
                return ConversationConfiguration;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationConfigurationService));
            }
            return null;
        }

        public async Task<ConversationConfiguration> Delete(ConversationConfiguration ConversationConfiguration)
        {
            if (!await ConversationConfigurationValidator.Delete(ConversationConfiguration))
                return ConversationConfiguration;

            try
            {
                await UOW.ConversationConfigurationRepository.Delete(ConversationConfiguration);
                Logging.CreateAuditLog(new { }, ConversationConfiguration, nameof(ConversationConfigurationService));
                return ConversationConfiguration;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationConfigurationService));
            }
            return null;
        }

     
    }
}
