using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Major : DataEntity, IEquatable<Major>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string MajorImage { get; set; }

        public bool Equals(Major other)
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

    public class MajorFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public List<MajorFilter> OrFilter { get; set; }
        public MajorOrder OrderBy { get; set; }
        public MajorSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MajorOrder
    {
        Id = 0,
        Name = 1,
        Description = 2,
    }

    [Flags]
    public enum MajorSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Description = E._2,
    }
}
