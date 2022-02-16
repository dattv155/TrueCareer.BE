using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class ActiveTime : DataEntity,  IEquatable<ActiveTime>
    {
        public long Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public long MentorId { get; set; }
        public AppUser Mentor { get; set; }
        
        public bool Equals(ActiveTime other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.StartAt != other.StartAt) return false;
            if (this.EndAt != other.EndAt) return false;
            if (this.MentorId != other.MentorId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ActiveTimeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public DateFilter StartAt { get; set; }
        public DateFilter EndAt { get; set; }
        public IdFilter MentorId { get; set; }
        public List<ActiveTimeFilter> OrFilter { get; set; }
        public ActiveTimeOrder OrderBy {get; set;}
        public ActiveTimeSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActiveTimeOrder
    {
        Id = 0,
        StartAt = 1,
        EndAt = 2,
        Mentor = 3,
    }

    [Flags]
    public enum ActiveTimeSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        StartAt = E._1,
        EndAt = E._2,
        Mentor = E._3,
    }
}
