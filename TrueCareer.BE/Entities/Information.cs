using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Information : DataEntity,  IEquatable<Information>
    {
        public long Id { get; set; }
        public long InformationTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartAt { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }
        public long? TopicId { get; set; }
        public long UserId { get; set; }
        public DateTime EndAt { get; set; }
        public InformationType InformationType { get; set; }
        public Topic? Topic { get; set; }
        public AppUser User { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Information other)
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

    public class InformationFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter InformationTypeId { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public DateFilter StartAt { get; set; }
        public StringFilter Role { get; set; }
        public StringFilter Image { get; set; }
        public IdFilter TopicId { get; set; }
        public IdFilter UserId { get; set; }
        public DateFilter EndAt { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<InformationFilter> OrFilter { get; set; }
        public InformationOrder OrderBy {get; set;}
        public InformationSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InformationOrder
    {
        Id = 0,
        InformationType = 1,
        Name = 2,
        Description = 3,
        StartAt = 4,
        Role = 5,
        Image = 6,
        Topic = 7,
        User = 8,
        EndAt = 9,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum InformationSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        InformationType = E._1,
        Name = E._2,
        Description = E._3,
        StartAt = E._4,
        Role = E._5,
        Image = E._6,
        Topic = E._7,
        User = E._8,
        EndAt = E._9,
    }
}
