using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.information
{
    public class Information_InformationTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Code { get; set; }
        
        public Information_InformationTypeDTO() {}
        public Information_InformationTypeDTO(InformationType InformationType)
        {
            
            this.Id = InformationType.Id;
            
            this.Name = InformationType.Name;
            
            this.Code = InformationType.Code;
            
            this.Informations = InformationType.Informations;
            this.Warnings = InformationType.Warnings;
            this.Errors = InformationType.Errors;
        }
    }

    public class Information_InformationTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Code { get; set; }
        
        public InformationTypeOrder OrderBy { get; set; }
    }
}