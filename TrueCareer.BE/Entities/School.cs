using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class School : DataEntity,  IEquatable<School>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Rating { get; set; }
        public string CompleteTime { get; set; }
        public long? StudentCount { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string SchoolImage { get; set; }
        public Guid RowId { get; set; }
        
        public bool Equals(School other)
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

    public class SchoolFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public DecimalFilter Rating { get; set; }
        public StringFilter CompleteTime { get; set; }
        public LongFilter StudentCount { get; set; }
        public StringFilter PhoneNumber { get; set; }
        public StringFilter Address { get; set; }
        public StringFilter SchoolImage { get; set; }
        public List<SchoolFilter> OrFilter { get; set; }
        public SchoolOrder OrderBy {get; set;}
        public SchoolSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SchoolOrder
    {
        Id = 0,
        Name = 1,
        Description = 2,
        Rating = 4,
        CompleteTime = 5,
        StudentCount = 6,
        PhoneNumber = 7,
        Address = 8,
        SchoolImage = 9,
    }

    [Flags]
    public enum SchoolSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Description = E._2,
        Rating = E._4,
        CompleteTime = E._5,
        StudentCount = E._6,
        PhoneNumber = E._7,
        Address = E._8,
        SchoolImage = E._9,
    }
}
