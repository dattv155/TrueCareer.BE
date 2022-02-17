using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Comment : DataEntity,  IEquatable<Comment>
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public long CreatorId { get; set; }
        public Guid DiscussionId { get; set; }
        public AppUser Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Comment other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Content != other.Content) return false;
            if (this.CreatorId != other.CreatorId) return false;
            if (this.DiscussionId != other.DiscussionId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class CommentFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter CreatorId { get; set; }
        public GuidFilter DiscussionId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<CommentFilter> OrFilter { get; set; }
        public CommentOrder OrderBy {get; set;}
        public CommentSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommentOrder
    {
        Id = 0,
        Content = 1,
        Creator = 2,
        Discussion = 6,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum CommentSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Content = E._1,
        Creator = E._2,
        Discussion = E._6,
    }
}
