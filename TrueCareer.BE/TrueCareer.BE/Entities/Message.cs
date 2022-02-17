using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Message : DataEntity,  IEquatable<Message>
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; }
        public long ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Message other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.UserId != other.UserId) return false;
            if (this.Content != other.Content) return false;
            if (this.ConversationId != other.ConversationId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MessageFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter UserId { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter ConversationId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<MessageFilter> OrFilter { get; set; }
        public MessageOrder OrderBy {get; set;}
        public MessageSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageOrder
    {
        Id = 0,
        User = 1,
        Content = 2,
        Conversation = 6,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum MessageSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        User = E._1,
        Content = E._2,
        Conversation = E._6,
    }
}
