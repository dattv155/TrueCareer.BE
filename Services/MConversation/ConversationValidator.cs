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

namespace TrueCareer.Services.MConversation
{
    public interface IConversationValidator : IServiceScoped
    {
        Task Get(Conversation Conversation);
        Task<bool> Create(Conversation Conversation);
        Task<bool> Update(Conversation Conversation);
        Task<bool> Delete(Conversation Conversation);
        Task<bool> BulkDelete(List<Conversation> Conversations);
        Task<bool> Import(List<Conversation> Conversations);
    }

    public class ConversationValidator : IConversationValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private ConversationMessage ConversationMessage;

        public ConversationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ConversationMessage = new ConversationMessage();
        }

        public async Task Get(Conversation Conversation)
        {
        }

        public async Task<bool> Create(Conversation Conversation)
        {
            await ValidateLatestContent(Conversation);
            await ValidateHash(Conversation);
            await ValidateMessages(Conversation);
            return Conversation.IsValidated;
        }

        public async Task<bool> Update(Conversation Conversation)
        {
            if (await ValidateId(Conversation))
            {
                await ValidateLatestContent(Conversation);
                await ValidateHash(Conversation);
                await ValidateMessages(Conversation);
            }
            return Conversation.IsValidated;
        }

        public async Task<bool> Delete(Conversation Conversation)
        {
            if (await ValidateId(Conversation))
            {
            }
            return Conversation.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Conversation> Conversations)
        {
            foreach (Conversation Conversation in Conversations)
            {
                await Delete(Conversation);
            }
            return Conversations.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Conversation> Conversations)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Conversation Conversation)
        {
            ConversationFilter ConversationFilter = new ConversationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Conversation.Id },
                Selects = ConversationSelect.Id
            };

            int count = await UOW.ConversationRepository.CountAll(ConversationFilter);
            if (count == 0)
                Conversation.AddError(nameof(ConversationValidator), nameof(Conversation.Id), ConversationMessage.Error.IdNotExisted, ConversationMessage);
            return Conversation.IsValidated;
        }

        private async Task<bool> ValidateLatestContent(Conversation Conversation)
        {
            if(string.IsNullOrEmpty(Conversation.LatestContent))
            {
                Conversation.AddError(nameof(ConversationValidator), nameof(Conversation.LatestContent), ConversationMessage.Error.LatestContentEmpty, ConversationMessage);
            }
            else if(Conversation.LatestContent.Count() > 500)
            {
                Conversation.AddError(nameof(ConversationValidator), nameof(Conversation.LatestContent), ConversationMessage.Error.LatestContentOverLength, ConversationMessage);
            }
            return Conversation.IsValidated;
        }
        private async Task<bool> ValidateHash(Conversation Conversation)
        {
            if(string.IsNullOrEmpty(Conversation.Hash))
            {
                Conversation.AddError(nameof(ConversationValidator), nameof(Conversation.Hash), ConversationMessage.Error.HashEmpty, ConversationMessage);
            }
            else if(Conversation.Hash.Count() > 500)
            {
                Conversation.AddError(nameof(ConversationValidator), nameof(Conversation.Hash), ConversationMessage.Error.HashOverLength, ConversationMessage);
            }
            return Conversation.IsValidated;
        }
        private async Task<bool> ValidateMessages(Conversation Conversation)
        {   
            if(Conversation.Messages?.Any() ?? false)
            {
                #region fetch data
                #endregion

                #region validate
                foreach(Message Message in Conversation.Messages)
                {
                    if(string.IsNullOrEmpty(Message.Content))
                    {
                        Message.AddError(nameof(ConversationValidator), nameof(Message.Content), ConversationMessage.Error.Message_ContentEmpty, ConversationMessage);
                    }
                    else if(Message.Content.Count() > 500)
                    {
                        Message.AddError(nameof(ConversationValidator), nameof(Message.Content), ConversationMessage.Error.Message_ContentOverLength, ConversationMessage);
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
