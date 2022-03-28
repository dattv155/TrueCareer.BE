using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class News : DataEntity,  IEquatable<News>
    {
        public long Id { get; set; }
        public long CreatorId { get; set; }
        public string NewsContent { get; set; }
        public long LikeCounting { get; set; }
        public long WatchCounting { get; set; }
        public long NewsStatusId { get; set; }
        public AppUser Creator { get; set; }
        public NewsStatus NewsStatus { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(News other)
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

    public class NewsFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter CreatorId { get; set; }
        public StringFilter NewsContent { get; set; }
        public LongFilter LikeCounting { get; set; }
        public LongFilter WatchCounting { get; set; }
        public IdFilter NewsStatusId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<NewsFilter> OrFilter { get; set; }
        public NewsOrder OrderBy {get; set;}
        public NewsSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NewsOrder
    {
        Id = 0,
        Creator = 2,
        NewsContent = 3,
        LikeCounting = 4,
        WatchCounting = 5,
        NewsStatus = 6,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum NewsSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Creator = E._2,
        NewsContent = E._3,
        LikeCounting = E._4,
        WatchCounting = E._5,
        NewsStatus = E._6,
    }
}
