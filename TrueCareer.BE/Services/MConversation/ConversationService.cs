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

namespace TrueCareer.Services.MConversation
{
    public interface IConversationService : IServiceScoped
    {
        Task<int> Count(ConversationFilter ConversationFilter);
        Task<List<Conversation>> List(ConversationFilter ConversationFilter);
        Task<Conversation> Get(long Id);
        Task<Conversation> Create(Conversation Conversation);
        Task<Conversation> Update(Conversation Conversation);
        Task<Conversation> Delete(Conversation Conversation);
    }

    public class ConversationService : IConversationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IConversationValidator ConversationValidator;

        public ConversationService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IConversationValidator ConversationValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ConversationValidator = ConversationValidator;
        }
        public async Task<int> Count(ConversationFilter ConversationFilter)
        {
            try
            {
                GlobalUser GlobalUser = await UOW.GlobalUserRepository.Get(CurrentContext.UserRowId);
                if (GlobalUser != null)
                    ConversationFilter.GlobalUserId = GlobalUser.Id;
                int result = await UOW.ConversationRepository.Count(ConversationFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return 0;
        }

        public async Task<List<Conversation>> List(ConversationFilter ConversationFilter)
        {
            try
            {
                GlobalUser GlobalUser = await UOW.GlobalUserRepository.Get(CurrentContext.UserRowId);
                if (GlobalUser != null)
                {
                    ConversationFilter.GlobalUserId = GlobalUser.Id;
                    List<Conversation> Conversations = await UOW.ConversationRepository.List(ConversationFilter);
                    List<long> ConversationIds = Conversations.Select(x => x.Id).ToList();
                    List<ConversationReadHistory> ConversationReadHistories = await UOW.ConversationReadHistoryRepository.List(ConversationIds, GlobalUser.Id);
                    foreach(Conversation Conversation in Conversations)
                    {
                        Conversation.CountUnread = ConversationReadHistories.Where(x => x.ConversationId == Conversation.Id)
                            .Select(x => x.CountUnread)
                            .FirstOrDefault();
                    }    
                    return Conversations;
                }
                return null;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;
        }

        public async Task<Conversation> Get(long Id)
        {
            Conversation Conversation = await UOW.ConversationRepository.Get(Id);
            if (Conversation == null)
                return null;
            return Conversation;
        }

        public async Task<Conversation> Create(Conversation Conversation)
        {
            if (!await ConversationValidator.Create(Conversation))
                return Conversation;

            try
            {
                List<long> GlobalUserIds = Conversation.ConversationParticipants
                    .Select(x => x.GlobalUserId)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                string hash = string.Join(";", GlobalUserIds);
                int count = await UOW.ConversationRepository.Count(new ConversationFilter
                {
                    Hash = new StringFilter { Equal = hash },
                });
                Conversation.Hash = hash;
                if (count == 0)
                {
                    Conversation.ConversationTypeId = ConversationTypeEnum.LOCAL.Id;
                    await UOW.ConversationRepository.Create(Conversation);
                    Conversation = await UOW.ConversationRepository.Get(Conversation.Id);
                }
                else
                {
                    Conversation = await UOW.ConversationRepository.Get(hash);
                }
                return Conversation;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;
        }

        public async Task<Conversation> Update(Conversation Conversation)
        {
            if (!await ConversationValidator.Update(Conversation))
                return Conversation;
            try
            {
                List<long> GlobalUserIds = Conversation.ConversationParticipants
                   .Select(x => x.GlobalUserId)
                   .Distinct()
                   .OrderBy(x => x)
                   .ToList();
                string hash = string.Join(";", GlobalUserIds);
                int count = await UOW.ConversationRepository.Count(new ConversationFilter
                {
                    Hash = new StringFilter { Equal = hash },
                });
                Conversation.Hash = hash;

                var oldData = await UOW.ConversationRepository.Get(Conversation.Id);
                Conversation.ConversationTypeId = oldData.ConversationTypeId;
                await UOW.ConversationRepository.Update(Conversation);

                Conversation = await UOW.ConversationRepository.Get(Conversation.Id);
                return Conversation;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;
        }

        public async Task<Conversation> Delete(Conversation Conversation)
        {
            if (!await ConversationValidator.Delete(Conversation))
                return Conversation;

            try
            {
                await UOW.ConversationRepository.Delete(Conversation);
                List<Conversation> Conversations = await UOW.ConversationRepository.List(new List<long> { Conversation.Id });
                return Conversation;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;
        }
    }
}
