using TrueSight.Common;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class ConversationParticipant : DataEntity,  IEquatable<ConversationParticipant>
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long GlobalUserId { get; set; }
        public Conversation Conversation { get; set; }
        public GlobalUser GlobalUser { get; set; }
        
        public bool Equals(ConversationParticipant other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.ConversationId != other.ConversationId) return false;
            if (this.GlobalUserId != other.GlobalUserId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ConversationParticipantFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter ConversationId { get; set; }
        public IdFilter GlobalUserId { get; set; }
        public List<ConversationParticipantFilter> OrFilter { get; set; }
        public ConversationParticipantOrder OrderBy {get; set;}
        public ConversationParticipantSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversationParticipantOrder
    {
        Id = 0,
        Conversation = 1,
        GlobalUser = 2,
    }

    [Flags]
    public enum ConversationParticipantSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Conversation = E._1,
        GlobalUser = E._2,
    }
}
