using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.mentor_connection
{
    public class MentorConnection_ConnectionTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Code { get; set; }
        
        public MentorConnection_ConnectionTypeDTO() {}
        public MentorConnection_ConnectionTypeDTO(ConnectionType ConnectionType)
        {
            
            this.Id = ConnectionType.Id;
            
            this.Name = ConnectionType.Name;
            
            this.Code = ConnectionType.Code;
            
            this.Informations = ConnectionType.Informations;
            this.Warnings = ConnectionType.Warnings;
            this.Errors = ConnectionType.Errors;
        }
    }

    public class MentorConnection_ConnectionTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Code { get; set; }
        
        public ConnectionTypeOrder OrderBy { get; set; }
    }
}