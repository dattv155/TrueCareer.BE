using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Image : DataEntity,  IEquatable<Image>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Image other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Name != other.Name) return false;
            if (this.Url != other.Url) return false;
            if (this.ThumbnailUrl != other.ThumbnailUrl) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ImageFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Url { get; set; }
        public StringFilter ThumbnailUrl { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ImageFilter> OrFilter { get; set; }
        public ImageOrder OrderBy {get; set;}
        public ImageSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImageOrder
    {
        Id = 0,
        Name = 1,
        Url = 2,
        ThumbnailUrl = 6,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ImageSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Url = E._2,
        ThumbnailUrl = E._6,
    }
}
