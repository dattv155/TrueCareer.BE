using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class InformationType : DataEntity,  IEquatable<InformationType>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        
        public bool Equals(InformationType other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Name != other.Name) return false;
            if (this.Code != other.Code) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class InformationTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Code { get; set; }
        public List<InformationTypeFilter> OrFilter { get; set; }
        public InformationTypeOrder OrderBy {get; set;}
        public InformationTypeSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InformationTypeOrder
    {
        Id = 0,
        Name = 1,
        Code = 2,
    }

    [Flags]
    public enum InformationTypeSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Code = E._2,
    }
}
