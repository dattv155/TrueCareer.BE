using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class MentorRegisterRequestDAO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long MentorApprovalStatusId { get; set; }
        public long TopicId { get; set; }

        public virtual MentorApprovalStatusDAO MentorApprovalStatus { get; set; }
        public virtual TopicDAO Topic { get; set; }
        public virtual AppUserDAO User { get; set; }
    }
}
