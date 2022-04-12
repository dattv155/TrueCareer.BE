using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class MentorConnection : DataEntity, IEquatable<MentorConnection>
    {
        public long Id { get; set; }
        public long MentorId { get; set; }
        public string Url { get; set; }
        public long ConnectionTypeId { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public AppUser Mentor { get; set; }

        public bool Equals(MentorConnection other)
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

    public class MentorConnectionFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter MentorId { get; set; }
        public StringFilter Url { get; set; }
        public IdFilter ConnectionTypeId { get; set; }
        public List<MentorConnectionFilter> OrFilter { get; set; }
        public MentorConnectionOrder OrderBy { get; set; }
        public MentorConnectionSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MentorConnectionOrder
    {
        Id = 0,
        Mentor = 1,
        Url = 2,
        ConnectionType = 3,
    }

    [Flags]
    public enum MentorConnectionSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Mentor = E._1,
        Url = E._2,
        ConnectionType = E._3,
    }
}
