using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class GlobalUserDAO
    {
        public GlobalUserDAO()
        {
            ConversationMessages = new HashSet<ConversationMessageDAO>();
            ConversationParticipants = new HashSet<ConversationParticipantDAO>();
            ConversationReadHistories = new HashSet<ConversationReadHistoryDAO>();
            Conversations = new HashSet<ConversationDAO>();
            FirebaseTokens = new HashSet<FirebaseTokenDAO>();
        }

        public long Id { get; set; }
        public long GlobalUserTypeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual GlobalUserTypeDAO GlobalUserType { get; set; }
        public virtual ICollection<ConversationMessageDAO> ConversationMessages { get; set; }
        public virtual ICollection<ConversationParticipantDAO> ConversationParticipants { get; set; }
        public virtual ICollection<ConversationReadHistoryDAO> ConversationReadHistories { get; set; }
        public virtual ICollection<ConversationDAO> Conversations { get; set; }
        public virtual ICollection<FirebaseTokenDAO> FirebaseTokens { get; set; }
    }
}
