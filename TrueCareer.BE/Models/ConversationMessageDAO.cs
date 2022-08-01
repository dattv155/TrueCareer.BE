using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ConversationMessageDAO
    {
        public ConversationMessageDAO()
        {
            ConversationAttachments = new HashSet<ConversationAttachmentDAO>();
        }

        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long GlobalUserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual ConversationDAO Conversation { get; set; }
        public virtual GlobalUserDAO GlobalUser { get; set; }
        public virtual ICollection<ConversationAttachmentDAO> ConversationAttachments { get; set; }
    }
}
