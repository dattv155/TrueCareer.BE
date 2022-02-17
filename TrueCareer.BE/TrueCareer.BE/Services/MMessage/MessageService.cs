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

namespace TrueCareer.Services.MMessage
{
    public interface IMessageService :  IServiceScoped
    {
        Task<int> Count(MessageFilter MessageFilter);
        Task<List<Message>> List(MessageFilter MessageFilter);
        Task<Message> Get(long Id);
        Task<Message> Create(Message Message);
        Task<Message> Update(Message Message);
        Task<Message> Delete(Message Message);
        Task<List<Message>> BulkDelete(List<Message> Messages);
        Task<List<Message>> Import(List<Message> Messages);
        Task<MessageFilter> ToFilter(MessageFilter MessageFilter);
    }

    public class MessageService : BaseService, IMessageService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IMessageValidator MessageValidator;

        public MessageService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IMessageValidator MessageValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.MessageValidator = MessageValidator;
        }
        public async Task<int> Count(MessageFilter MessageFilter)
        {
            try
            {
                int result = await UOW.MessageRepository.Count(MessageFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MessageService));
            }
            return 0;
        }

        public async Task<List<Message>> List(MessageFilter MessageFilter)
        {
            try
            {
                List<Message> Messages = await UOW.MessageRepository.List(MessageFilter);
                return Messages;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MessageService));
            }
            return null;
        }

        public async Task<Message> Get(long Id)
        {
            Message Message = await UOW.MessageRepository.Get(Id);
            await MessageValidator.Get(Message);
            if (Message == null)
                return null;
            return Message;
        }
        
        public async Task<Message> Create(Message Message)
        {
            if (!await MessageValidator.Create(Message))
                return Message;

            try
            {
                await UOW.MessageRepository.Create(Message);
                Message = await UOW.MessageRepository.Get(Message.Id);
                Logging.CreateAuditLog(Message, new { }, nameof(MessageService));
                return Message;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MessageService));
            }
            return null;
        }

        public async Task<Message> Update(Message Message)
        {
            if (!await MessageValidator.Update(Message))
                return Message;
            try
            {
                var oldData = await UOW.MessageRepository.Get(Message.Id);

                await UOW.MessageRepository.Update(Message);

                Message = await UOW.MessageRepository.Get(Message.Id);
                Logging.CreateAuditLog(Message, oldData, nameof(MessageService));
                return Message;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MessageService));
            }
            return null;
        }

        public async Task<Message> Delete(Message Message)
        {
            if (!await MessageValidator.Delete(Message))
                return Message;

            try
            {
                await UOW.MessageRepository.Delete(Message);
                Logging.CreateAuditLog(new { }, Message, nameof(MessageService));
                return Message;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MessageService));
            }
            return null;
        }

        public async Task<List<Message>> BulkDelete(List<Message> Messages)
        {
            if (!await MessageValidator.BulkDelete(Messages))
                return Messages;

            try
            {
                await UOW.MessageRepository.BulkDelete(Messages);
                Logging.CreateAuditLog(new { }, Messages, nameof(MessageService));
                return Messages;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MessageService));
            }
            return null;

        }
        
        public async Task<List<Message>> Import(List<Message> Messages)
        {
            if (!await MessageValidator.Import(Messages))
                return Messages;
            try
            {
                await UOW.MessageRepository.BulkMerge(Messages);

                Logging.CreateAuditLog(Messages, new { }, nameof(MessageService));
                return Messages;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MessageService));
            }
            return null;
        }     
        
        public async Task<MessageFilter> ToFilter(MessageFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<MessageFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                MessageFilter subFilter = new MessageFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UserId))
                        subFilter.UserId = FilterBuilder.Merge(subFilter.UserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Content))
                        subFilter.Content = FilterBuilder.Merge(subFilter.Content, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ConversationId))
                        subFilter.ConversationId = FilterBuilder.Merge(subFilter.ConversationId, FilterPermissionDefinition.IdFilter);
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

        private void Sync(List<Message> Messages)
        {
            List<Conversation> Conversations = new List<Conversation>();
            Conversations.AddRange(Messages.Select(x => new Conversation { Id = x.ConversationId }));
            
            Conversations = Conversations.Distinct().ToList();
            RabbitManager.PublishList(Conversations, RoutingKeyEnum.ConversationUsed.Code);
        }

    }
}
