using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.choice
{
    public class Choice_MbtiSingleTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public Choice_MbtiSingleTypeDTO() {}
        public Choice_MbtiSingleTypeDTO(MbtiSingleType MbtiSingleType)
        {
            
            this.Id = MbtiSingleType.Id;
            
            this.Code = MbtiSingleType.Code;
            
            this.Name = MbtiSingleType.Name;
            
            this.Informations = MbtiSingleType.Informations;
            this.Warnings = MbtiSingleType.Warnings;
            this.Errors = MbtiSingleType.Errors;
        }
    }

    public class Choice_MbtiSingleTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public MbtiSingleTypeOrder OrderBy { get; set; }
    }
}