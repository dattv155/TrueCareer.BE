using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Conversation : DataEntity,  IEquatable<Conversation>
    {
        public long Id { get; set; }
        public string LatestContent { get; set; }
        public long? LatestUserId { get; set; }
        public string Hash { get; set; }
        public List<Message> Messages { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Conversation other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.LatestContent != other.LatestContent) return false;
            if (this.LatestUserId != other.LatestUserId) return false;
            if (this.Hash != other.Hash) return false;
            if (this.Messages?.Count != other.Messages?.Count) return false;
            else if (this.Messages != null && other.Messages != null)
            {
                for (int i = 0; i < Messages.Count; i++)
                {
                    Message Message = Messages[i];
                    Message otherMessage = other.Messages[i];
                    if (Message == null && otherMessage != null)
                        return false;
                    if (Message != null && otherMessage == null)
                        return false;
                    if (Message.Equals(otherMessage) == false)
                        return false;
                }
            }
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
        public StringFilter LatestContent { get; set; }
        public IdFilter LatestUserId { get; set; }
        public StringFilter Hash { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ConversationFilter> OrFilter { get; set; }
        public ConversationOrder OrderBy {get; set;}
        public ConversationSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversationOrder
    {
        Id = 0,
        LatestContent = 1,
        LatestUser = 3,
        Hash = 7,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ConversationSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        LatestContent = E._1,
        LatestUser = E._3,
        Hash = E._7,
    }
}
