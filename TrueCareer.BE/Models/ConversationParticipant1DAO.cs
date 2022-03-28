using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ConversationParticipant1DAO
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long UserId { get; set; }

        public virtual Conversation1DAO Conversation { get; set; }
        public virtual AppUserDAO User { get; set; }
    }
}
