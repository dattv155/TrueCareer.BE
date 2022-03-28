using TrueSight.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.conversation
{
    public class Conversation_ConversationDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public long CountUnread { get; set; }
        public List<Conversation_ConversationParticipantDTO> ConversationParticipants { get; set; }
        public string LatestContent { get; set; }
        public long? LatestGlobalUserId { get; set; }
        public Conversation_GlobalUserDTO LatestGlobalUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Conversation_ConversationDTO() {}
        public Conversation_ConversationDTO(Conversation Conversation)
        {
            this.Id = Conversation.Id;
            this.Name = Conversation.Name;
            this.Avatar = Conversation.Avatar;
            this.CountUnread = Conversation.CountUnread;
            this.LatestContent = Conversation.LatestContent;
            this.LatestGlobalUserId = Conversation.LatestGlobalUserId;
            this.LatestGlobalUser = Conversation.LatestGlobalUser == null ? null : new Conversation_GlobalUserDTO(Conversation.LatestGlobalUser);
            this.ConversationParticipants = Conversation.ConversationParticipants?.Select(x => new Conversation_ConversationParticipantDTO(x)).ToList();
            this.CreatedAt = Conversation.CreatedAt;
            this.UpdatedAt = Conversation.UpdatedAt;
            this.Errors = Conversation.Errors;
        }
    }

    public class Conversation_ConversationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Avatar { get; set; }
        public IdFilter ConversationTypeId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ConversationOrder OrderBy { get; set; }
    }
}
