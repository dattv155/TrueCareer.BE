using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class MessageDAO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long ConversationId { get; set; }

        public virtual ConversationDAO Conversation { get; set; }
    }
}
