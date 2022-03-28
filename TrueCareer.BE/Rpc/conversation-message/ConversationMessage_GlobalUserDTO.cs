using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.conversation_message
{
    public class ConversationMessage_GlobalUserDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long GlobalUserTypeId { get; set; }
        
        public string Username { get; set; }
        
        public string DisplayName { get; set; }
        
        public string Avatar { get; set; }
        
        public Guid RowId { get; set; }
        public ConversationMessage_GlobalUserDTO() {}
        public ConversationMessage_GlobalUserDTO(GlobalUser GlobalUser)
        {
            
            this.Id = GlobalUser.Id;
            
            this.GlobalUserTypeId = GlobalUser.GlobalUserTypeId;
            
            this.Username = GlobalUser.Username;
            
            this.DisplayName = GlobalUser.DisplayName;
            
            this.Avatar = GlobalUser.Avatar;
            
            this.RowId = GlobalUser.RowId;
            this.Informations = GlobalUser.Informations;
            this.Warnings = GlobalUser.Warnings;
            this.Errors = GlobalUser.Errors;
        }
    }

    public class ConversationMessage_GlobalUserFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter GlobalUserTypeId { get; set; }
        
        public StringFilter Username { get; set; }
        
        public StringFilter DisplayName { get; set; }
        
        public StringFilter Avatar { get; set; }
        
        public GlobalUserOrder OrderBy { get; set; }
    }
}