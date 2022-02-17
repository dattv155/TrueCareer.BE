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

namespace TrueCareer.Services.MMessage
{
    public interface IMessageValidator : IServiceScoped
    {
        Task Get(Message Message);
        Task<bool> Create(Message Message);
        Task<bool> Update(Message Message);
        Task<bool> Delete(Message Message);
        Task<bool> BulkDelete(List<Message> Messages);
        Task<bool> Import(List<Message> Messages);
    }

    public class MessageValidator : IMessageValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private MessageMessage MessageMessage;

        public MessageValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.MessageMessage = new MessageMessage();
        }

        public async Task Get(Message Message)
        {
        }

        public async Task<bool> Create(Message Message)
        {
            await ValidateContent(Message);
            await ValidateConversation(Message);
            return Message.IsValidated;
        }

        public async Task<bool> Update(Message Message)
        {
            if (await ValidateId(Message))
            {
                await ValidateContent(Message);
                await ValidateConversation(Message);
            }
            return Message.IsValidated;
        }

        public async Task<bool> Delete(Message Message)
        {
            if (await ValidateId(Message))
            {
            }
            return Message.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Message> Messages)
        {
            foreach (Message Message in Messages)
            {
                await Delete(Message);
            }
            return Messages.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Message> Messages)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Message Message)
        {
            MessageFilter MessageFilter = new MessageFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Message.Id },
                Selects = MessageSelect.Id
            };

            int count = await UOW.MessageRepository.CountAll(MessageFilter);
            if (count == 0)
                Message.AddError(nameof(MessageValidator), nameof(Message.Id), MessageMessage.Error.IdNotExisted, MessageMessage);
            return Message.IsValidated;
        }

        private async Task<bool> ValidateContent(Message Message)
        {
            if(string.IsNullOrEmpty(Message.Content))
            {
                Message.AddError(nameof(MessageValidator), nameof(Message.Content), MessageMessage.Error.ContentEmpty, MessageMessage);
            }
            else if(Message.Content.Count() > 500)
            {
                Message.AddError(nameof(MessageValidator), nameof(Message.Content), MessageMessage.Error.ContentOverLength, MessageMessage);
            }
            return Message.IsValidated;
        }
        private async Task<bool> ValidateConversation(Message Message)
        {       
            if(Message.ConversationId == 0)
            {
                Message.AddError(nameof(MessageValidator), nameof(Message.Conversation), MessageMessage.Error.ConversationEmpty, MessageMessage);
            }
            else
            {
                int count = await UOW.ConversationRepository.CountAll(new ConversationFilter
                {
                    Id = new IdFilter{ Equal =  Message.ConversationId },
                });
                if(count == 0)
                {
                    Message.AddError(nameof(MessageValidator), nameof(Message.Conversation), MessageMessage.Error.ConversationNotExisted, MessageMessage);
                }
            }
            return true;
        }
    }
}
