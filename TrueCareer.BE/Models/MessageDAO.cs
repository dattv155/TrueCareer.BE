using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
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
        public Guid RowId { get; set; }

        public virtual Conversation1DAO Conversation { get; set; }
    }
}
