using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class MentorMenteeConnection : DataEntity,  IEquatable<MentorMenteeConnection>
    {
        public long MentorId { get; set; }
        public long MenteeId { get; set; }
        public long ConnectionId { get; set; }
        public string FirstMessage { get; set; }
        public long ConnectionStatusId { get; set; }
        public long ActiveTimeId { get; set; }
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public MentorConnection Connection { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }
        public AppUser Mentee { get; set; }
        public AppUser Mentor { get; set; }
        
        public bool Equals(MentorMenteeConnection other)
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

    public class MentorMenteeConnectionFilter : FilterEntity
    {
        public IdFilter MentorId { get; set; }
        public IdFilter MenteeId { get; set; }
        public IdFilter ConnectionId { get; set; }
        public IdFilter ConversationId { get; set; }
        public StringFilter FirstMessage { get; set; }
        public IdFilter ConnectionStatusId { get; set; }
        public IdFilter ActiveTimeId { get; set; }
        public IdFilter Id { get; set; }
        public List<MentorMenteeConnectionFilter> OrFilter { get; set; }
        public MentorMenteeConnectionOrder OrderBy {get; set;}
        public MentorMenteeConnectionSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MentorMenteeConnectionOrder
    {
        Mentor = 0,
        Mentee = 1,
        Connection = 2,
        FirstMessage = 3,
        ConnectionStatus = 4,
        ActiveTime = 5,
        Id = 6,
    }

    [Flags]
    public enum MentorMenteeConnectionSelect:long
    {
        ALL = E.ALL,
        Mentor = E._0,
        Mentee = E._1,
        Connection = E._2,
        FirstMessage = E._3,
        ConnectionStatus = E._4,
        ActiveTime = E._5,
        Id = E._6,
    }
}
