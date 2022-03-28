using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueCareer.Entities;
using TrueCareer;
using TrueCareer.Repositories;
using TrueCareer.Common;

namespace TrueCareer.Services.MConversation
{
    public interface IConversationValidator : IServiceScoped
    {
        Task<bool> Create(Conversation Conversation);
        Task<bool> Update(Conversation Conversation);
        Task<bool> Delete(Conversation Conversation);
        Task<bool> BulkDelete(List<Conversation> Conversations);
        Task<bool> Import(List<Conversation> Conversations);
    }

    public class ConversationValidator : IConversationValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            ParticipantIsExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ConversationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Conversation Conversation)
        {
            ConversationFilter ConversationFilter = new ConversationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Conversation.Id },
                Selects = ConversationSelect.Id
            };

            int count = await UOW.ConversationRepository.Count(ConversationFilter);
            if (count == 0)
                Conversation.AddError(nameof(ConversationValidator), nameof(Conversation.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Conversation Conversation)
        {
            return Conversation.IsValidated;
        }

        public async Task<bool> Update(Conversation Conversation)
        {
            if (await ValidateId(Conversation))
            {
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
    }
}
