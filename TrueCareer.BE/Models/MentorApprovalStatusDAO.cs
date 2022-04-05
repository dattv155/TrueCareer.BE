using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class MentorApprovalStatusDAO
    {
        public MentorApprovalStatusDAO()
        {
            MentorRegisterRequests = new HashSet<MentorRegisterRequestDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MentorRegisterRequestDAO> MentorRegisterRequests { get; set; }
    }
}
