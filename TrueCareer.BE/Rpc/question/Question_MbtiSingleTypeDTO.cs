using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.question
{
    public class Question_MbtiSingleTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public Question_MbtiSingleTypeDTO() {}
        public Question_MbtiSingleTypeDTO(MbtiSingleType MbtiSingleType)
        {
            
            this.Id = MbtiSingleType.Id;
            
            this.Code = MbtiSingleType.Code;
            
            this.Name = MbtiSingleType.Name;
            
            this.Informations = MbtiSingleType.Informations;
            this.Warnings = MbtiSingleType.Warnings;
            this.Errors = MbtiSingleType.Errors;
        }
    }

    public class Question_MbtiSingleTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public MbtiSingleTypeOrder OrderBy { get; set; }
    }
}