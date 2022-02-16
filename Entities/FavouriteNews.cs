using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class FavouriteNews : DataEntity,  IEquatable<FavouriteNews>
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long NewsId { get; set; }
        public News News { get; set; }
        public AppUser User { get; set; }
        
        public bool Equals(FavouriteNews other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.UserId != other.UserId) return false;
            if (this.NewsId != other.NewsId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class FavouriteNewsFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter UserId { get; set; }
        public IdFilter NewsId { get; set; }
        public List<FavouriteNewsFilter> OrFilter { get; set; }
        public FavouriteNewsOrder OrderBy {get; set;}
        public FavouriteNewsSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FavouriteNewsOrder
    {
        Id = 0,
        User = 1,
        News = 2,
    }

    [Flags]
    public enum FavouriteNewsSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        User = E._1,
        News = E._2,
    }
}
