using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.conversation_message
{
    public class ConversationMessage_ConversationDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long ConversationTypeId { get; set; }
        
        public string Name { get; set; }
        
        public string Avatar { get; set; }
        
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ConversationMessage_ConversationDTO() {}
        public ConversationMessage_ConversationDTO(Conversation Conversation)
        {
            
            this.Id = Conversation.Id;
            
            this.ConversationTypeId = Conversation.ConversationTypeId;
            
            this.Name = Conversation.Name;
            
            this.Avatar = Conversation.Avatar;
            
            this.RowId = Conversation.RowId;
            this.CreatedAt = Conversation.CreatedAt;
            this.UpdatedAt = Conversation.UpdatedAt;
            this.Informations = Conversation.Informations;
            this.Warnings = Conversation.Warnings;
            this.Errors = Conversation.Errors;
        }
    }

    public class ConversationMessage_ConversationFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter ConversationTypeId { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Avatar { get; set; }
        
        public ConversationOrder OrderBy { get; set; }
    }
}