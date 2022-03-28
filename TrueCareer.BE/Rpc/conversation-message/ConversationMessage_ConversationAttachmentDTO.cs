using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.conversation_message
{
    public class ConversationMessage_ConversationAttachmentDTO : DataDTO
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
        public ConversationMessage_ConversationAttachmentTypeDTO ConversationAttachmentType { get; set; }   
        public ConversationMessage_ConversationAttachmentDTO() {}
        public ConversationMessage_ConversationAttachmentDTO(ConversationAttachment ConversationAttachment)
        {
            this.Id = ConversationAttachment.Id;
            this.ConversationMessageId = ConversationAttachment.ConversationMessageId;
            this.ConversationAttachmentTypeId = ConversationAttachment.ConversationAttachmentTypeId;
            this.Url = ConversationAttachment.Url;
            this.Thumbnail = ConversationAttachment.Thumbnail;
            this.Size = ConversationAttachment.Size;
            this.Name = ConversationAttachment.Name;
            this.Checksum = ConversationAttachment.Checksum;
            this.Type = ConversationAttachment.Type;
            this.ConversationAttachmentType = ConversationAttachment.ConversationAttachmentType == null ? null : new ConversationMessage_ConversationAttachmentTypeDTO(ConversationAttachment.ConversationAttachmentType);
            this.Errors = ConversationAttachment.Errors;
        }
    }

    public class ConversationMessage_ConversationAttachmentFilterDTO : FilterDTO
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
        
        public ConversationAttachmentOrder OrderBy { get; set; }
    }
}