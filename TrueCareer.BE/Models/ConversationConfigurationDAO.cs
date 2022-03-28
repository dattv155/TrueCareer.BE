using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ConversationConfigurationDAO
    {
        public ConversationConfigurationDAO()
        {
            Conversations = new HashSet<ConversationDAO>();
        }

        public long Id { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string AppName { get; set; }
        public string OaId { get; set; }
        public string OaToken { get; set; }
        public string OaSecretKey { get; set; }
        public long ConversationTypeId { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ConversationTypeDAO ConversationType { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<ConversationDAO> Conversations { get; set; }
    }
}
