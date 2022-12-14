using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class MentorMenteeConnectionDAO
    {
        public long MentorId { get; set; }
        public long MenteeId { get; set; }
        public long? ConnectionId { get; set; }
        public string FirstMessage { get; set; }
        public long ConnectionStatusId { get; set; }
        public long ActiveTimeId { get; set; }
        public long Id { get; set; }
        public long? ConversationId { get; set; }

        public virtual ActiveTimeDAO ActiveTime { get; set; }
        public virtual MentorConnectionDAO Connection { get; set; }
        public virtual ConnectionStatusDAO ConnectionStatus { get; set; }
        public virtual ConversationDAO Conversation { get; set; }
        public virtual AppUserDAO Mentee { get; set; }
        public virtual AppUserDAO Mentor { get; set; }
    }
}
