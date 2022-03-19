using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrueSight.Common;

namespace TrueCareer.Entities
{
    public class MentorApprovalStatus : DataEntity, IEquatable<MentorApprovalStatus>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public bool Equals(MentorApprovalStatus other)
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

    public class MentorApprovalStatusFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<ConnectionStatusFilter> OrFilter { get; set; }
        public ConnectionStatusOrder OrderBy { get; set; }
        public ConnectionStatusSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MentorApprovalStatusOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum MentorApprovalStatusSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
