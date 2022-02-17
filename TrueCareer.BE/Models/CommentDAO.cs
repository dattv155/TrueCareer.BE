using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class CommentDAO
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid DiscussionId { get; set; }

        public virtual AppUserDAO Creator { get; set; }
    }
}
