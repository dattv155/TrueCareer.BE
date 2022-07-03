using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ConversationReadHistoryDAO
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long GlobalUserId { get; set; }
        public DateTime ReadAt { get; set; }
        public long CountUnread { get; set; }
        public long UserId { get; set; }

        public virtual ConversationDAO Conversation { get; set; }
        public virtual GlobalUserDAO GlobalUser { get; set; }
        public virtual AppUserDAO User { get; set; }
    }
}
