using TrueSight.Common;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class ConversationAttachment : DataEntity,  IEquatable<ConversationAttachment>
    {
        public long Id { get; set; }
        public long ConversationMessageId { get; set; }
        public long ConversationAttachmentTypeId { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public string Size { get; set; }
        public string Name { get; set; }
        public string Checksum { get; set; }
        public string Type { get; set; }
        public ConversationAttachmentType ConversationAttachmentType { get; set; }
        public ConversationMessage ConversationMessage { get; set; }
        
        public bool Equals(ConversationAttachment other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.ConversationMessageId != other.ConversationMessageId) return false;
            if (this.ConversationAttachmentTypeId != other.ConversationAttachmentTypeId) return false;
            if (this.Url != other.Url) return false;
            if (this.Thumbnail != other.Thumbnail) return false;
            if (this.Size != other.Size) return false;
            if (this.Name != other.Name) return false;
            if (this.Checksum != other.Checksum) return false;
            if (this.Type != other.Type) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ConversationAttachmentFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter ConversationMessageId { get; set; }
        public IdFilter ConversationAttachmentTypeId { get; set; }
        public StringFilter Url { get; set; }
        public StringFilter Thumbnail { get; set; }
        public StringFilter Size { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Checksum { get; set; }
        public StringFilter Type { get; set; }
        public List<ConversationAttachmentFilter> OrFilter { get; set; }
        public ConversationAttachmentOrder OrderBy {get; set;}
        public ConversationAttachmentSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversationAttachmentOrder
    {
        Id = 0,
        ConversationMessage = 1,
        ConversationAttachmentType = 2,
        Url = 3,
        Thumbnail = 4,
        Size = 5,
        Name = 6,
        Checksum = 7,
        Type = 8,
    }

    [Flags]
    public enum ConversationAttachmentSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        ConversationMessage = E._1,
        ConversationAttachmentType = E._2,
        Url = E._3,
        Thumbnail = E._4,
        Size = E._5,
        Name = E._6,
        Checksum = E._7,
        Type = E._8,
    }
}
