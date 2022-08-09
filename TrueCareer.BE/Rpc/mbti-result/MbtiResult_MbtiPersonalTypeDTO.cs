using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.mbti_result
{
    public class MbtiResult_MbtiPersonalTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Code { get; set; }
        
        public string Value { get; set; }
        public List<MbtiPersonalTypeMajorMapping> MbtiPersonalTypeMajorMappings { get; set; }
        
        
        public MbtiResult_MbtiPersonalTypeDTO() {}
        public MbtiResult_MbtiPersonalTypeDTO(MbtiPersonalType MbtiPersonalType)
        {
            
            this.Id = MbtiPersonalType.Id;
            
            this.Name = MbtiPersonalType.Name;
            
            this.Code = MbtiPersonalType.Code;
            
            this.Value = MbtiPersonalType.Value;
            this.MbtiPersonalTypeMajorMappings = MbtiPersonalType.MbtiPersonalTypeMajorMappings;
            
            this.Informations = MbtiPersonalType.Informations;
            this.Warnings = MbtiPersonalType.Warnings;
            this.Errors = MbtiPersonalType.Errors;
        }
    }

    public class MbtiResult_MbtiPersonalTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Value { get; set; }
        
        public MbtiPersonalTypeOrder OrderBy { get; set; }
    }
}