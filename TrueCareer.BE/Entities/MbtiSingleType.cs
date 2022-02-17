using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class MbtiSingleType : DataEntity,  IEquatable<MbtiSingleType>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public bool Equals(MbtiSingleType other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MbtiSingleTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<MbtiSingleTypeFilter> OrFilter { get; set; }
        public MbtiSingleTypeOrder OrderBy {get; set;}
        public MbtiSingleTypeSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MbtiSingleTypeOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum MbtiSingleTypeSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
