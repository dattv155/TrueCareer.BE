using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class School : DataEntity,  IEquatable<School>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid RowId { get; set; }
        
        public bool Equals(School other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Name != other.Name) return false;
            if (this.Description != other.Description) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class SchoolFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public List<SchoolFilter> OrFilter { get; set; }
        public SchoolOrder OrderBy {get; set;}
        public SchoolSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SchoolOrder
    {
        Id = 0,
        Name = 1,
        Description = 2,
    }

    [Flags]
    public enum SchoolSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Description = E._2,
    }
}
