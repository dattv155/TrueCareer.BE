using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class SchoolMajorMapping : DataEntity,  IEquatable<SchoolMajorMapping>
    {
        public long SchoolId { get; set; }
        public long MajorId { get; set; }
        public Major Major { get; set; }
        public School School { get; set; }
        
        public bool Equals(SchoolMajorMapping other)
        {
            if (other == null) return false;
            if (this.SchoolId != other.SchoolId) return false;
            if (this.MajorId != other.MajorId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class SchoolMajorMappingFilter : FilterEntity
    {
        public IdFilter SchoolId { get; set; }
        public IdFilter MajorId { get; set; }
        public List<SchoolMajorMappingFilter> OrFilter { get; set; }
        public SchoolMajorMappingOrder OrderBy {get; set;}
        public SchoolMajorMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SchoolMajorMappingOrder
    {
        School = 0,
        Major = 1,
    }

    [Flags]
    public enum SchoolMajorMappingSelect:long
    {
        ALL = E.ALL,
        School = E._0,
        Major = E._1,
    }
}
