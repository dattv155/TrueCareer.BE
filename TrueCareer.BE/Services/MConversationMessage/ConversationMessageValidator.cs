using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueCareer.Entities;
using TrueCareer;
using TrueCareer.Repositories;
using TrueCareer.Common;

namespace TrueCareer.Services.MConversationMessage
{
    public interface IConversationMessageValidator : IServiceScoped
    {
        Task<bool> Create(ConversationMessage ConversationMessage);
        Task<bool> Update(ConversationMessage ConversationMessage);
        Task<bool> Delete(ConversationMessage ConversationMessage);
        Task<bool> BulkDelete(List<ConversationMessage> ConversationMessages);
        Task<bool> Import(List<ConversationMessage> ConversationMessages);
    }

    public class ConversationMessageValidator : IConversationMessageValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            MessageError,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ConversationMessageValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ConversationMessage ConversationMessage)
        {
            ConversationMessageFilter ConversationMessageFilter = new ConversationMessageFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ConversationMessage.Id },
                ConversationId = ConversationMessage.ConversationId,
                Selects = ConversationMessageSelect.Id
            };

            int count = await UOW.ConversationMessageRepository.Count(ConversationMessageFilter);
            if (count == 0)
                ConversationMessage.AddError(nameof(ConversationMessageValidator), nameof(ConversationMessage.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ConversationMessage ConversationMessage)
        {
            return ConversationMessage.IsValidated;
        }

        public async Task<bool> Update(ConversationMessage ConversationMessage)
        {
            if (await ValidateId(ConversationMessage))
            {
            }
            return ConversationMessage.IsValidated;
        }

        public async Task<bool> Delete(ConversationMessage ConversationMessage)
        {
            if (await ValidateId(ConversationMessage))
            {
            }
            return ConversationMessage.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ConversationMessage> ConversationMessages)
        {
            foreach (ConversationMessage ConversationMessage in ConversationMessages)
            {
                await Delete(ConversationMessage);
            }
            return ConversationMessages.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<ConversationMessage> ConversationMessages)
        {
            return true;
        }
    }
}
