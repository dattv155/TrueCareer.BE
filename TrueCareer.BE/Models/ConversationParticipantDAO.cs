using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class ConversationParticipantDAO
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long UserId { get; set; }

        public virtual ConversationDAO Conversation { get; set; }
        public virtual AppUserDAO User { get; set; }
    }
}
