using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class MentorReviewDAO
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string ContentReview { get; set; }
        public long Star { get; set; }
        public long MentorId { get; set; }
        public long CreatorId { get; set; }
        public DateTime Time { get; set; }

        public virtual AppUserDAO Creator { get; set; }
        public virtual AppUserDAO Mentor { get; set; }
    }
}
