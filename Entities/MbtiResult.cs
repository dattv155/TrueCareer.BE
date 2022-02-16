using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class MbtiResult : DataEntity,  IEquatable<MbtiResult>
    {
        public long UserId { get; set; }
        public long MbtiPersonalTypeId { get; set; }
        public long Id { get; set; }
        public MbtiPersonalType MbtiPersonalType { get; set; }
        public AppUser User { get; set; }
        
        public bool Equals(MbtiResult other)
        {
            if (other == null) return false;
            if (this.UserId != other.UserId) return false;
            if (this.MbtiPersonalTypeId != other.MbtiPersonalTypeId) return false;
            if (this.Id != other.Id) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MbtiResultFilter : FilterEntity
    {
        public IdFilter UserId { get; set; }
        public IdFilter MbtiPersonalTypeId { get; set; }
        public IdFilter Id { get; set; }
        public List<MbtiResultFilter> OrFilter { get; set; }
        public MbtiResultOrder OrderBy {get; set;}
        public MbtiResultSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MbtiResultOrder
    {
        User = 0,
        MbtiPersonalType = 1,
        Id = 2,
    }

    [Flags]
    public enum MbtiResultSelect:long
    {
        ALL = E.ALL,
        User = E._0,
        MbtiPersonalType = E._1,
        Id = E._2,
    }
}
