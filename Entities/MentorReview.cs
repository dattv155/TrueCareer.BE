using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class MentorReview : DataEntity,  IEquatable<MentorReview>
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string ContentReview { get; set; }
        public long Star { get; set; }
        public long MentorId { get; set; }
        public long CreatorId { get; set; }
        public DateTime Time { get; set; }
        public AppUser Creator { get; set; }
        public AppUser Mentor { get; set; }
        
        public bool Equals(MentorReview other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Description != other.Description) return false;
            if (this.ContentReview != other.ContentReview) return false;
            if (this.Star != other.Star) return false;
            if (this.MentorId != other.MentorId) return false;
            if (this.CreatorId != other.CreatorId) return false;
            if (this.Time != other.Time) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MentorReviewFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Description { get; set; }
        public StringFilter ContentReview { get; set; }
        public LongFilter Star { get; set; }
        public IdFilter MentorId { get; set; }
        public IdFilter CreatorId { get; set; }
        public DateFilter Time { get; set; }
        public List<MentorReviewFilter> OrFilter { get; set; }
        public MentorReviewOrder OrderBy {get; set;}
        public MentorReviewSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MentorReviewOrder
    {
        Id = 0,
        Description = 1,
        ContentReview = 2,
        Star = 3,
        Mentor = 4,
        Creator = 5,
        Time = 6,
    }

    [Flags]
    public enum MentorReviewSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Description = E._1,
        ContentReview = E._2,
        Star = E._3,
        Mentor = E._4,
        Creator = E._5,
        Time = E._6,
    }
}
