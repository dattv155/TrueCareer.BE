using TrueSight.Common;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Conversation : DataEntity,  IEquatable<Conversation>
    {
        public long Id { get; set; }
        public long ConversationTypeId { get; set; }
        public long? ConversationConfigurationId { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Avatar { get; set; }
        public long CountUnread { get; set; }
        public string LatestContent { get; set; }
        public long? LatestGlobalUserId { get; set; }
        public long? StatusId { get; set; }
        public GlobalUser LatestGlobalUser { get; set; }
        public ConversationConfiguration ConversationConfiguration { get; set; }
        public ConversationType ConversationType { get; set; }
        public List<ConversationMessage> ConversationMessages { get; set; }
        public List<ConversationParticipant> ConversationParticipants { get; set; }
        public List<ConversationReadHistory> ConversationReadHistories { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Conversation other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.ConversationTypeId != other.ConversationTypeId) return false;
            if (this.Name != other.Name) return false;
            if (this.Avatar != other.Avatar) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ConversationFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter ConversationTypeId { get; set; }
        public IdFilter StatusId { get; set; }
        public StringFilter Hash { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Avatar { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ConversationFilter> OrFilter { get; set; }
        public ConversationOrder OrderBy {get; set;}
        public ConversationSelect Selects {get; set;}
        public long? GlobalUserId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversationOrder
    {
        Id = 0,
        ConversationType = 1,
        Name = 2,
        Avatar = 3,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ConversationSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        ConversationType = E._1,
        Name = E._2,
        Avatar = E._3,
    }
}
