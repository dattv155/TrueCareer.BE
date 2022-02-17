using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class MbtiPersonalTypeMajorMapping : DataEntity,  IEquatable<MbtiPersonalTypeMajorMapping>
    {
        public long MbtiPersonalTypeId { get; set; }
        public long MajorId { get; set; }
        public Major Major { get; set; }
        public MbtiPersonalType MbtiPersonalType { get; set; }
        
        public bool Equals(MbtiPersonalTypeMajorMapping other)
        {
            if (other == null) return false;
            if (this.MbtiPersonalTypeId != other.MbtiPersonalTypeId) return false;
            if (this.MajorId != other.MajorId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MbtiPersonalTypeMajorMappingFilter : FilterEntity
    {
        public IdFilter MbtiPersonalTypeId { get; set; }
        public IdFilter MajorId { get; set; }
        public List<MbtiPersonalTypeMajorMappingFilter> OrFilter { get; set; }
        public MbtiPersonalTypeMajorMappingOrder OrderBy {get; set;}
        public MbtiPersonalTypeMajorMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MbtiPersonalTypeMajorMappingOrder
    {
        MbtiPersonalType = 0,
        Major = 1,
    }

    [Flags]
    public enum MbtiPersonalTypeMajorMappingSelect:long
    {
        ALL = E.ALL,
        MbtiPersonalType = E._0,
        Major = E._1,
    }
}
