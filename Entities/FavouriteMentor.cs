using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class FavouriteMentor : DataEntity,  IEquatable<FavouriteMentor>
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long MentorId { get; set; }
        public AppUser Mentor { get; set; }
        public AppUser User { get; set; }
        
        public bool Equals(FavouriteMentor other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.UserId != other.UserId) return false;
            if (this.MentorId != other.MentorId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class FavouriteMentorFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter UserId { get; set; }
        public IdFilter MentorId { get; set; }
        public List<FavouriteMentorFilter> OrFilter { get; set; }
        public FavouriteMentorOrder OrderBy {get; set;}
        public FavouriteMentorSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FavouriteMentorOrder
    {
        Id = 0,
        User = 1,
        Mentor = 2,
    }

    [Flags]
    public enum FavouriteMentorSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        User = E._1,
        Mentor = E._2,
    }
}
