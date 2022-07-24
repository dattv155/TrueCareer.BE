using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ActiveTimeDAO
    {
        public ActiveTimeDAO()
        {
            MentorMenteeConnections = new HashSet<MentorMenteeConnectionDAO>();
        }

        public long Id { get; set; }
        public DateTime ActiveDate { get; set; }
        public long MentorId { get; set; }
        public long UnitOfTimeId { get; set; }

        public virtual AppUserDAO Mentor { get; set; }
        public virtual UnitOfTimeDAO UnitOfTime { get; set; }
        public virtual ICollection<MentorMenteeConnectionDAO> MentorMenteeConnections { get; set; }
    }
}
