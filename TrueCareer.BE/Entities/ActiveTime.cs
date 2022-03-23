using TrueSight.Common;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class ActiveTime : DataEntity,  IEquatable<ActiveTime>
    {
        public long Id { get; set; }
        public DateTime ActiveDate { get; set; }
        public long MentorId { get; set; }
        public AppUser Mentor { get; set; }

        public long UnitOfTimeId { get; set; }

        public UnitOfTime UnitOfTime { get; set; }
        
        public bool Equals(ActiveTime other)
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

    public class ActiveTimeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public DateFilter ActiveDate { get; set; }
        public IdFilter MentorId { get; set; }
        public IdFilter UnitOfTimeId { get; set; }
        public List<ActiveTimeFilter> OrFilter { get; set; }
        public ActiveTimeOrder OrderBy {get; set;}
        public ActiveTimeSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActiveTimeOrder
    {
        Id = 0,
        ActiveDate = 1,
        UnitOfTime = 2,
        Mentor = 3,
    }

    [Flags]
    public enum ActiveTimeSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        ActiveDate = E._1,
        UnitOfTime = E._2,
        Mentor = E._3,
    }
}
