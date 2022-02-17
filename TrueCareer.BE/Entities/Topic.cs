using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Topic : DataEntity,  IEquatable<Topic>
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        
        public bool Equals(Topic other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Title != other.Title) return false;
            if (this.Description != other.Description) return false;
            if (this.Cost != other.Cost) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class TopicFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Title { get; set; }
        public StringFilter Description { get; set; }
        public DecimalFilter Cost { get; set; }
        public List<TopicFilter> OrFilter { get; set; }
        public TopicOrder OrderBy {get; set;}
        public TopicSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TopicOrder
    {
        Id = 0,
        Title = 1,
        Description = 2,
        Cost = 3,
    }

    [Flags]
    public enum TopicSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Title = E._1,
        Description = E._2,
        Cost = E._3,
    }
}
