using TrueSight.Common;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class ConversationReadHistory : DataEntity,  IEquatable<ConversationReadHistory>
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long GlobalUserId { get; set; }
        public DateTime ReadAt { get; set; }
        public long CountUnread { get; set; }
        public Conversation Conversation { get; set; }
        public GlobalUser GlobalUser { get; set; }
        
        public bool Equals(ConversationReadHistory other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.ConversationId != other.ConversationId) return false;
            if (this.GlobalUserId != other.GlobalUserId) return false;
            if (this.ReadAt != other.ReadAt) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ConversationReadHistoryFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter ConversationId { get; set; }
        public IdFilter GlobalUserId { get; set; }
        public DateFilter ReadAt { get; set; }
        public List<ConversationReadHistoryFilter> OrFilter { get; set; }
        public ConversationReadHistoryOrder OrderBy {get; set;}
        public ConversationReadHistorySelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversationReadHistoryOrder
    {
        Id = 0,
        Conversation = 1,
        GlobalUser = 2,
        ReadAt = 3,
    }

    [Flags]
    public enum ConversationReadHistorySelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Conversation = E._1,
        GlobalUser = E._2,
        ReadAt = E._3,
    }
}
