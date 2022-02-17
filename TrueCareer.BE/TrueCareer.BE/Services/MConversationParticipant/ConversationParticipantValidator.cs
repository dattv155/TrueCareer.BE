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

namespace TrueCareer.Services.MConversationParticipant
{
    public interface IConversationParticipantValidator : IServiceScoped
    {
        Task Get(ConversationParticipant ConversationParticipant);
        Task<bool> Create(ConversationParticipant ConversationParticipant);
        Task<bool> Update(ConversationParticipant ConversationParticipant);
        Task<bool> Delete(ConversationParticipant ConversationParticipant);
        Task<bool> BulkDelete(List<ConversationParticipant> ConversationParticipants);
        Task<bool> Import(List<ConversationParticipant> ConversationParticipants);
    }

    public class ConversationParticipantValidator : IConversationParticipantValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private ConversationParticipantMessage ConversationParticipantMessage;

        public ConversationParticipantValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ConversationParticipantMessage = new ConversationParticipantMessage();
        }

        public async Task Get(ConversationParticipant ConversationParticipant)
        {
        }

        public async Task<bool> Create(ConversationParticipant ConversationParticipant)
        {
            await ValidateConversation(ConversationParticipant);
            await ValidateUser(ConversationParticipant);
            return ConversationParticipant.IsValidated;
        }

        public async Task<bool> Update(ConversationParticipant ConversationParticipant)
        {
            if (await ValidateId(ConversationParticipant))
            {
                await ValidateConversation(ConversationParticipant);
                await ValidateUser(ConversationParticipant);
            }
            return ConversationParticipant.IsValidated;
        }

        public async Task<bool> Delete(ConversationParticipant ConversationParticipant)
        {
            if (await ValidateId(ConversationParticipant))
            {
            }
            return ConversationParticipant.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ConversationParticipant> ConversationParticipants)
        {
            foreach (ConversationParticipant ConversationParticipant in ConversationParticipants)
            {
                await Delete(ConversationParticipant);
            }
            return ConversationParticipants.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<ConversationParticipant> ConversationParticipants)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(ConversationParticipant ConversationParticipant)
        {
            ConversationParticipantFilter ConversationParticipantFilter = new ConversationParticipantFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ConversationParticipant.Id },
                Selects = ConversationParticipantSelect.Id
            };

            int count = await UOW.ConversationParticipantRepository.CountAll(ConversationParticipantFilter);
            if (count == 0)
                ConversationParticipant.AddError(nameof(ConversationParticipantValidator), nameof(ConversationParticipant.Id), ConversationParticipantMessage.Error.IdNotExisted, ConversationParticipantMessage);
            return ConversationParticipant.IsValidated;
        }

        private async Task<bool> ValidateConversation(ConversationParticipant ConversationParticipant)
        {       
            if(ConversationParticipant.ConversationId == 0)
            {
                ConversationParticipant.AddError(nameof(ConversationParticipantValidator), nameof(ConversationParticipant.Conversation), ConversationParticipantMessage.Error.ConversationEmpty, ConversationParticipantMessage);
            }
            else
            {
                int count = await UOW.ConversationRepository.CountAll(new ConversationFilter
                {
                    Id = new IdFilter{ Equal =  ConversationParticipant.ConversationId },
                });
                if(count == 0)
                {
                    ConversationParticipant.AddError(nameof(ConversationParticipantValidator), nameof(ConversationParticipant.Conversation), ConversationParticipantMessage.Error.ConversationNotExisted, ConversationParticipantMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateUser(ConversationParticipant ConversationParticipant)
        {       
            if(ConversationParticipant.UserId == 0)
            {
                ConversationParticipant.AddError(nameof(ConversationParticipantValidator), nameof(ConversationParticipant.User), ConversationParticipantMessage.Error.UserEmpty, ConversationParticipantMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  ConversationParticipant.UserId },
                });
                if(count == 0)
                {
                    ConversationParticipant.AddError(nameof(ConversationParticipantValidator), nameof(ConversationParticipant.User), ConversationParticipantMessage.Error.UserNotExisted, ConversationParticipantMessage);
                }
            }
            return true;
        }
    }
}
