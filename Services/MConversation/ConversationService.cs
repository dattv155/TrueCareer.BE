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

namespace TrueCareer.Services.MConversation
{
    public interface IConversationService :  IServiceScoped
    {
        Task<int> Count(ConversationFilter ConversationFilter);
        Task<List<Conversation>> List(ConversationFilter ConversationFilter);
        Task<Conversation> Get(long Id);
        Task<Conversation> Create(Conversation Conversation);
        Task<Conversation> Update(Conversation Conversation);
        Task<Conversation> Delete(Conversation Conversation);
        Task<List<Conversation>> BulkDelete(List<Conversation> Conversations);
        Task<List<Conversation>> Import(List<Conversation> Conversations);
        Task<ConversationFilter> ToFilter(ConversationFilter ConversationFilter);
    }

    public class ConversationService : BaseService, IConversationService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IConversationValidator ConversationValidator;

        public ConversationService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IConversationValidator ConversationValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.ConversationValidator = ConversationValidator;
        }
        public async Task<int> Count(ConversationFilter ConversationFilter)
        {
            try
            {
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
                List<Conversation> Conversations = await UOW.ConversationRepository.List(ConversationFilter);
                return Conversations;
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
            await ConversationValidator.Get(Conversation);
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
                await UOW.ConversationRepository.Create(Conversation);
                Conversation = await UOW.ConversationRepository.Get(Conversation.Id);
                Logging.CreateAuditLog(Conversation, new { }, nameof(ConversationService));
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
                var oldData = await UOW.ConversationRepository.Get(Conversation.Id);

                await UOW.ConversationRepository.Update(Conversation);

                Conversation = await UOW.ConversationRepository.Get(Conversation.Id);
                Logging.CreateAuditLog(Conversation, oldData, nameof(ConversationService));
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
                Logging.CreateAuditLog(new { }, Conversation, nameof(ConversationService));
                return Conversation;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;
        }

        public async Task<List<Conversation>> BulkDelete(List<Conversation> Conversations)
        {
            if (!await ConversationValidator.BulkDelete(Conversations))
                return Conversations;

            try
            {
                await UOW.ConversationRepository.BulkDelete(Conversations);
                Logging.CreateAuditLog(new { }, Conversations, nameof(ConversationService));
                return Conversations;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;

        }
        
        public async Task<List<Conversation>> Import(List<Conversation> Conversations)
        {
            if (!await ConversationValidator.Import(Conversations))
                return Conversations;
            try
            {
                await UOW.ConversationRepository.BulkMerge(Conversations);

                Logging.CreateAuditLog(Conversations, new { }, nameof(ConversationService));
                return Conversations;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;
        }     
        
        public async Task<ConversationFilter> ToFilter(ConversationFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ConversationFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ConversationFilter subFilter = new ConversationFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.LatestContent))
                        subFilter.LatestContent = FilterBuilder.Merge(subFilter.LatestContent, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.LatestUserId))
                        subFilter.LatestUserId = FilterBuilder.Merge(subFilter.LatestUserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Hash))
                        subFilter.Hash = FilterBuilder.Merge(subFilter.Hash, FilterPermissionDefinition.StringFilter);
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

        private void Sync(List<Conversation> Conversations)
        {
            List<Message> Messages = new List<Message>();
            Messages.AddRange(Conversations.SelectMany(x => x.Messages.Select(y => new Message { Id = y.Id })));
            
            Messages = Messages.Distinct().ToList();
            RabbitManager.PublishList(Messages, RoutingKeyEnum.MessageUsed.Code);
        }

    }
}
