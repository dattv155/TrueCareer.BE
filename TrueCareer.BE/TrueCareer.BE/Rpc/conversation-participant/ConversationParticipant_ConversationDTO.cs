using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.conversation_participant
{
    public class ConversationParticipant_ConversationDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string LatestContent { get; set; }
        
        public long? LatestUserId { get; set; }
        
        public string Hash { get; set; }
        
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ConversationParticipant_ConversationDTO() {}
        public ConversationParticipant_ConversationDTO(Conversation Conversation)
        {
            
            this.Id = Conversation.Id;
            
            this.LatestContent = Conversation.LatestContent;
            
            this.LatestUserId = Conversation.LatestUserId;
            
            this.Hash = Conversation.Hash;
            
            this.RowId = Conversation.RowId;
            this.CreatedAt = Conversation.CreatedAt;
            this.UpdatedAt = Conversation.UpdatedAt;
            this.Informations = Conversation.Informations;
            this.Warnings = Conversation.Warnings;
            this.Errors = Conversation.Errors;
        }
    }

    public class ConversationParticipant_ConversationFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter LatestContent { get; set; }
        
        public IdFilter LatestUserId { get; set; }
        
        public StringFilter Hash { get; set; }
        
        public ConversationOrder OrderBy { get; set; }
    }
}