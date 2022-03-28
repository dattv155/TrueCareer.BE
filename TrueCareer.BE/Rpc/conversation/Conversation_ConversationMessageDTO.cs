using TrueSight.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.conversation
{
    public class Conversation_ConversationMessageDTO : DataDTO
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long GlobalUserId { get; set; }
        public string Content { get; set; }
        public Conversation_GlobalUserDTO GlobalUser { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Conversation_ConversationMessageDTO() {}
        public Conversation_ConversationMessageDTO(ConversationMessage ConversationMessage)
        {
            this.Id = ConversationMessage.Id;
            this.ConversationId = ConversationMessage.ConversationId;
            this.GlobalUserId = ConversationMessage.GlobalUserId;
            this.Content = ConversationMessage.Content;
            this.GlobalUser = ConversationMessage.GlobalUser == null ? null : new Conversation_GlobalUserDTO(ConversationMessage.GlobalUser);
            this.CreatedAt = ConversationMessage.CreatedAt;
            this.UpdatedAt = ConversationMessage.UpdatedAt;
            this.Errors = ConversationMessage.Errors;
        }
    }

    public class Conversation_ConversationMessageFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter ConversationId { get; set; }
        
        public IdFilter ConversationTypeId { get; set; }
        
        public IdFilter GlobalUserId { get; set; }
        
        public StringFilter Content { get; set; }
        
        public ConversationMessageOrder OrderBy { get; set; }
    }
}