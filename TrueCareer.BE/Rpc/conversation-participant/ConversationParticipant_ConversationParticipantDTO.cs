using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.conversation_participant
{
    public class ConversationParticipant_ConversationParticipantDTO : DataDTO
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long UserId { get; set; }
        public ConversationParticipant_ConversationDTO Conversation { get; set; }
        public ConversationParticipant_AppUserDTO User { get; set; }
        public ConversationParticipant_ConversationParticipantDTO() {}
        public ConversationParticipant_ConversationParticipantDTO(ConversationParticipant ConversationParticipant)
        {
            this.Id = ConversationParticipant.Id;
            this.ConversationId = ConversationParticipant.ConversationId;
            this.UserId = ConversationParticipant.UserId;
            this.Conversation = ConversationParticipant.Conversation == null ? null : new ConversationParticipant_ConversationDTO(ConversationParticipant.Conversation);
            this.User = ConversationParticipant.User == null ? null : new ConversationParticipant_AppUserDTO(ConversationParticipant.User);
            this.Informations = ConversationParticipant.Informations;
            this.Warnings = ConversationParticipant.Warnings;
            this.Errors = ConversationParticipant.Errors;
        }
    }

    public class ConversationParticipant_ConversationParticipantFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter ConversationId { get; set; }
        public IdFilter UserId { get; set; }
        public ConversationParticipantOrder OrderBy { get; set; }
    }
}
