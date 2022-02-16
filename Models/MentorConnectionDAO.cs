using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class MentorConnectionDAO
    {
        public MentorConnectionDAO()
        {
            MentorMenteeConnections = new HashSet<MentorMenteeConnectionDAO>();
        }

        public long Id { get; set; }
        public long MentorId { get; set; }
        public string Url { get; set; }
        public long ConnectionTypeId { get; set; }

        public virtual ConnectionTypeDAO ConnectionType { get; set; }
        public virtual AppUserDAO Mentor { get; set; }
        public virtual ICollection<MentorMenteeConnectionDAO> MentorMenteeConnections { get; set; }
    }
}
