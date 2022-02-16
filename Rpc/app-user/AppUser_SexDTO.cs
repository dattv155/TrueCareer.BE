using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_SexDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public AppUser_SexDTO() {}
        public AppUser_SexDTO(Sex Sex)
        {
            
            this.Id = Sex.Id;
            
            this.Code = Sex.Code;
            
            this.Name = Sex.Name;
            
            this.Informations = Sex.Informations;
            this.Warnings = Sex.Warnings;
            this.Errors = Sex.Errors;
        }
    }

    public class AppUser_SexFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public SexOrder OrderBy { get; set; }
    }
}