using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.BE.Entities;
using TrueSight.Common;

namespace TrueCareer.Entities
{
    public class MentorRegisterRequest : DataEntity, IEquatable<MentorRegisterRequest>
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long MentorApprovalStatusId { get; set; }
        public MentorApprovalStatus MentorApprovalStatus { get; set; }
        public AppUser User { get; set; }
        public MentorInfo MentorInfo {get; set;}
        public bool Equals(MentorRegisterRequest other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MentorRegisterRequestFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter UserId { get; set; }
        public IdFilter MentorApprovalStatusId { get; set; }
        public IdFilter TopicId { get; set; }
        public List<MentorRegisterRequestFilter> OrFilter { get; set; }
        public MentorRegisterRequestOrder OrderBy { get; set; }
        public MentorRegisterRequestSelect Selects { get; set; }
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MentorRegisterRequestOrder
    {
        Id = 0,
        User = 1,
        MentorApprovalStatus = 2,
        Topic = 3,
    }

    [Flags]
    public enum MentorRegisterRequestSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        User = E._1,
        MentorApprovalStatus = E._2,
        Topic = E._3,
    }
}
