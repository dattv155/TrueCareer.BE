using TrueSight.Common;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class ConversationMessage : DataEntity,  IEquatable<ConversationMessage>
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long GlobalUserId { get; set; }
        public string Content { get; set; }
        public Conversation Conversation { get; set; }
        public GlobalUser GlobalUser { get; set; }
        public List<ConversationAttachment> ConversationAttachments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(ConversationMessage other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.ConversationId != other.ConversationId) return false;
            if (this.GlobalUserId != other.GlobalUserId) return false;
            if (this.Content != other.Content) return false;
            if (this.ConversationAttachments?.Count != other.ConversationAttachments?.Count) return false;
            else if (this.ConversationAttachments != null && other.ConversationAttachments != null)
            {
                for (int i = 0; i < ConversationAttachments.Count; i++)
                {
                    ConversationAttachment ConversationAttachment = ConversationAttachments[i];
                    ConversationAttachment otherConversationAttachment = other.ConversationAttachments[i];
                    if (ConversationAttachment == null && otherConversationAttachment != null)
                        return false;
                    if (ConversationAttachment != null && otherConversationAttachment == null)
                        return false;
                    if (ConversationAttachment.Equals(otherConversationAttachment) == false)
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

    public class ConversationMessageFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public long ConversationId { get; set; }
        public IdFilter GlobalUserId { get; set; }
        public StringFilter Content { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ConversationMessageFilter> OrFilter { get; set; }
        public ConversationMessageOrder OrderBy {get; set;}
        public ConversationMessageSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversationMessageOrder
    {
        Id = 0,
        Conversation = 1,
        GlobalUser = 2,
        Content = 3,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ConversationMessageSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Conversation = E._1,
        GlobalUser = E._2,
        Content = E._3,
    }
}
