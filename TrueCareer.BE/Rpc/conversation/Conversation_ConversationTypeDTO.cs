using TrueSight.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.conversation
{
    public class Conversation_ConversationTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Conversation_ConversationTypeDTO() {}
        public Conversation_ConversationTypeDTO(ConversationType ConversationType)
        {
            
            this.Id = ConversationType.Id;
            
            this.Code = ConversationType.Code;
            
            this.Name = ConversationType.Name;
            
            this.Errors = ConversationType.Errors;
        }
    }

    public class Conversation_ConversationTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public ConversationTypeOrder OrderBy { get; set; }
    }
}