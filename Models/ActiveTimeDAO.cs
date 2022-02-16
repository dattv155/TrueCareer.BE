using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class ActiveTimeDAO
    {
        public long Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public long MentorId { get; set; }

        public virtual AppUserDAO Mentor { get; set; }
    }
}
