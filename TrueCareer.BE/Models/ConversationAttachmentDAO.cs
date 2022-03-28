using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ConversationAttachmentDAO
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

        public virtual ConversationAttachmentTypeDAO ConversationAttachmentType { get; set; }
        public virtual ConversationMessageDAO ConversationMessage { get; set; }
    }
}
