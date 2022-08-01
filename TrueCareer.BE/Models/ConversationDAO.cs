using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ConversationDAO
    {
        public ConversationDAO()
        {
            ConversationMessages = new HashSet<ConversationMessageDAO>();
            ConversationParticipants = new HashSet<ConversationParticipantDAO>();
            ConversationReadHistories = new HashSet<ConversationReadHistoryDAO>();
            MentorMenteeConnections = new HashSet<MentorMenteeConnectionDAO>();
        }

        public long Id { get; set; }
        public long ConversationTypeId { get; set; }
        public long? ConversationConfigurationId { get; set; }
        public long? FacebookConfigurationId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public string Hash { get; set; }
        public long? LatestGlobalUserId { get; set; }
        public string LatestContent { get; set; }
        public long StatusId { get; set; }

        public virtual ConversationConfigurationDAO ConversationConfiguration { get; set; }
        public virtual ConversationTypeDAO ConversationType { get; set; }
        public virtual GlobalUserDAO LatestGlobalUser { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<ConversationMessageDAO> ConversationMessages { get; set; }
        public virtual ICollection<ConversationParticipantDAO> ConversationParticipants { get; set; }
        public virtual ICollection<ConversationReadHistoryDAO> ConversationReadHistories { get; set; }
        public virtual ICollection<MentorMenteeConnectionDAO> MentorMenteeConnections { get; set; }
    }
}
