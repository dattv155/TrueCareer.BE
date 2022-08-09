using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class MbtiPersonalType : DataEntity,  IEquatable<MbtiPersonalType>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public List<MbtiPersonalTypeMajorMapping> MbtiPersonalTypeMajorMappings { get; set; }
        
        public bool Equals(MbtiPersonalType other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MbtiPersonalTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Value { get; set; }
        public List<MbtiPersonalTypeFilter> OrFilter { get; set; }
        public MbtiPersonalTypeOrder OrderBy {get; set;}
        public MbtiPersonalTypeSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MbtiPersonalTypeOrder
    {
        Id = 0,
        Name = 1,
        Code = 2,
    }

    [Flags]
    public enum MbtiPersonalTypeSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Code = E._2,
        Value = E._3,
    }
}
