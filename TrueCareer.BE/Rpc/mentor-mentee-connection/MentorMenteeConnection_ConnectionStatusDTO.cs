using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.mentor_mentee_connection
{
    public class MentorMenteeConnection_ConnectionStatusDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public MentorMenteeConnection_ConnectionStatusDTO() {}
        public MentorMenteeConnection_ConnectionStatusDTO(ConnectionStatus ConnectionStatus)
        {
            
            this.Id = ConnectionStatus.Id;
            
            this.Code = ConnectionStatus.Code;
            
            this.Name = ConnectionStatus.Name;
            
            this.Informations = ConnectionStatus.Informations;
            this.Warnings = ConnectionStatus.Warnings;
            this.Errors = ConnectionStatus.Errors;
        }
    }

    public class MentorMenteeConnection_ConnectionStatusFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public ConnectionStatusOrder OrderBy { get; set; }
    }
}