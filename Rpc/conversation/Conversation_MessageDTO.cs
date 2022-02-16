using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.conversation
{
    public class Conversation_MessageDTO : DataDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; }
        public long ConversationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Conversation_MessageDTO() {}
        public Conversation_MessageDTO(Message Message)
        {
            this.Id = Message.Id;
            this.UserId = Message.UserId;
            this.Content = Message.Content;
            this.ConversationId = Message.ConversationId;
            this.Errors = Message.Errors;
        }
    }

    public class Conversation_MessageFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter UserId { get; set; }
        
        public StringFilter Content { get; set; }
        
        public IdFilter ConversationId { get; set; }
        
        public MessageOrder OrderBy { get; set; }
    }
}