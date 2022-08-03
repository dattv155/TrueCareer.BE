using TrueSight.Common;
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
using TrueCareer.Common;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TrueCareer.Service;
using System.Net;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Configuration;
using TrueCareer.Hub;

namespace TrueCareer.Services.MConversationMessage
{
    public interface IConversationMessageService : IServiceScoped
    {
        Task<int> Count(ConversationMessageFilter ConversationMessageFilter);
        Task<List<ConversationMessage>> List(ConversationMessageFilter ConversationMessageFilter);
        Task<ConversationMessage> Get(long Id);
        Task<ConversationMessage> CreateFromOutside(ConversationMessage ConversationMessage);
        Task<ConversationMessage> CreateFromInside(ConversationMessage ConversationMessage);
        Task<ConversationMessage> Update(ConversationMessage ConversationMessage);
        Task<ConversationMessage> Delete(ConversationMessage ConversationMessage);
        Task<bool> Read(long ConversationId, long GlobalUserId);
    }

    public class ConversationMessageService : IConversationMessageService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IConfiguration Configuration;
        private IConversationMessageValidator ConversationMessageValidator;
        private IHubContext<ConversationHub> ConversationHub;

        public ConversationMessageService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IConfiguration Configuration,
            IConversationMessageValidator ConversationMessageValidator,
            IHubContext<ConversationHub> ConversationHub,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.Configuration = Configuration;
            this.ConversationMessageValidator = ConversationMessageValidator;
            this.CurrentContext = CurrentContext;
            this.ConversationHub = ConversationHub;
        }
        public async Task<int> Count(ConversationMessageFilter ConversationMessageFilter)
        {
            try
            {
                int result = await UOW.ConversationMessageRepository.Count(ConversationMessageFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return 0;
        }

        public async Task<List<ConversationMessage>> List(ConversationMessageFilter ConversationMessageFilter)
        {
            try
            {
                List<ConversationMessage> ConversationMessages = await UOW.ConversationMessageRepository.List(ConversationMessageFilter);
                GlobalUser GlobalUser = await UOW.GlobalUserRepository.Get(CurrentContext.UserRowId);
                if (GlobalUser != null)
                {
                    await UOW.ConversationReadHistoryRepository.Read(ConversationMessageFilter.ConversationId, GlobalUser.Id);
                }
                return ConversationMessages;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return null;
        }
        public async Task<bool> Read(long ConversationId, long GlobalUserId)
        {
            try
            {
                var rs = await UOW.ConversationReadHistoryRepository.Read(ConversationId, GlobalUserId);
                Conversation Conversation = await UOW.ConversationRepository.Get(ConversationId);
                return rs;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return false;
        }
        public async Task<ConversationMessage> Get(long Id)
        {
            ConversationMessage ConversationMessage = await UOW.ConversationMessageRepository.Get(Id);
            if (ConversationMessage == null)
                return null;
            return ConversationMessage;
        }
        public async Task<ConversationMessage> CreateFromOutside(ConversationMessage ConversationMessage)
        {
            if (!await ConversationMessageValidator.Create(ConversationMessage))
                return ConversationMessage;

            try
            {
                await UOW.ConversationMessageRepository.Create(ConversationMessage);
                ConversationMessage = await UOW.ConversationMessageRepository.Get(ConversationMessage.Id);
                Conversation Conversation = await UOW.ConversationRepository.Get(ConversationMessage.ConversationId);
                foreach (ConversationParticipant ConversationParticipant in Conversation.ConversationParticipants)
                {
                    var message = JsonConvert.SerializeObject(ConversationMessage, BuildJsonSerializerSettings());
                    _ = ConversationHub.Clients.User(ConversationParticipant.GlobalUser.Id.ToString())
                        .SendAsync("ReceiveMessage", Conversation.Id.ToString(), "CREATE", message);
                }
                return ConversationMessage;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return null;
        }

        public async Task<ConversationMessage> CreateFromInside(ConversationMessage ConversationMessage)
        {
            if (!await ConversationMessageValidator.Create(ConversationMessage))
                return ConversationMessage;

            try
            {
                Conversation Conversation = await UOW.ConversationRepository.Get(ConversationMessage.ConversationId);

                await UOW.ConversationMessageRepository.Create(ConversationMessage);
                ConversationMessage = await UOW.ConversationMessageRepository.Get(ConversationMessage.Id);
                await UOW.ConversationReadHistoryRepository.Read(ConversationMessage.ConversationId, ConversationMessage.GlobalUserId);

                Conversation = await UOW.ConversationRepository.Get(ConversationMessage.ConversationId);

                foreach (ConversationParticipant ConversationParticipant in Conversation.ConversationParticipants)
                {
                    var message = JsonConvert.SerializeObject(ConversationMessage, BuildJsonSerializerSettings());
                    _ = ConversationHub.Clients.User(ConversationParticipant.GlobalUser.Id.ToString())
                        .SendAsync("ReceiveMessage", Conversation.Id.ToString(), "CREATE", message);
                }

                return ConversationMessage;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return null;
        }

        public async Task<ConversationMessage> Update(ConversationMessage ConversationMessage)
        {
            if (!await ConversationMessageValidator.Update(ConversationMessage))
                return ConversationMessage;
            try
            {
                var oldData = await UOW.ConversationMessageRepository.Get(ConversationMessage.Id);

                await UOW.ConversationMessageRepository.Update(ConversationMessage);
                ConversationMessage = await UOW.ConversationMessageRepository.Get(ConversationMessage.Id);

                Conversation Conversation = await UOW.ConversationRepository.Get(ConversationMessage.ConversationId);
                if (Conversation.ConversationTypeId == ConversationTypeEnum.LOCAL.Id)
                {
                    foreach (ConversationParticipant ConversationParticipant in Conversation.ConversationParticipants)
                    {
                        var message = JsonConvert.SerializeObject(ConversationMessage, BuildJsonSerializerSettings());
                        _ = ConversationHub.Clients.User(ConversationParticipant.GlobalUser.RowId.ToString())
                            .SendAsync("ReceiveMessage", Conversation.Id.ToString(), "UPDATE", message);
                    }
                }
                return ConversationMessage;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return null;
        }

        public async Task<ConversationMessage> Delete(ConversationMessage ConversationMessage)
        {
            if (!await ConversationMessageValidator.Delete(ConversationMessage))
                return ConversationMessage;

            try
            {
                await UOW.ConversationMessageRepository.Delete(ConversationMessage);

                Conversation Conversation = await UOW.ConversationRepository.Get(ConversationMessage.ConversationId);
                var ConversationMessages = await UOW.ConversationMessageRepository.List(new List<long> { ConversationMessage.Id });
                if (Conversation.ConversationTypeId == ConversationTypeEnum.LOCAL.Id)
                {
                    foreach (ConversationParticipant ConversationParticipant in Conversation.ConversationParticipants)
                    {
                        var message = JsonConvert.SerializeObject(ConversationMessage, BuildJsonSerializerSettings());
                        _ = ConversationHub.Clients.User(ConversationParticipant.GlobalUser.RowId.ToString())
                            .SendAsync("ReceiveMessage", Conversation.Id.ToString(), "DELETE", message);
                    }
                }
                return ConversationMessage;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return null;
        }

        private JsonSerializerSettings BuildJsonSerializerSettings()
        {
            JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffK"
            };
            return JsonSerializerSettings;
        }
    }
}
