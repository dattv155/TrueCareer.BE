using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class Conversation1DAO
    {
        public Conversation1DAO()
        {
            Messages = new HashSet<MessageDAO>();
        }

        public long Id { get; set; }
        public string LatestContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? LatestUserId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public string Hash { get; set; }

        public virtual ICollection<MessageDAO> Messages { get; set; }
    }
}
