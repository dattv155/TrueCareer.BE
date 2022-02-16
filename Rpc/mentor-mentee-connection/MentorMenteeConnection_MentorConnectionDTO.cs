using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.mentor_mentee_connection
{
    public class MentorMenteeConnection_MentorConnectionDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long MentorId { get; set; }
        
        public string Url { get; set; }
        
        public long ConnectionTypeId { get; set; }
        
        public MentorMenteeConnection_MentorConnectionDTO() {}
        public MentorMenteeConnection_MentorConnectionDTO(MentorConnection MentorConnection)
        {
            
            this.Id = MentorConnection.Id;
            
            this.MentorId = MentorConnection.MentorId;
            
            this.Url = MentorConnection.Url;
            
            this.ConnectionTypeId = MentorConnection.ConnectionTypeId;
            
            this.Informations = MentorConnection.Informations;
            this.Warnings = MentorConnection.Warnings;
            this.Errors = MentorConnection.Errors;
        }
    }

    public class MentorMenteeConnection_MentorConnectionFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter MentorId { get; set; }
        
        public StringFilter Url { get; set; }
        
        public IdFilter ConnectionTypeId { get; set; }
        
        public MentorConnectionOrder OrderBy { get; set; }
    }
}