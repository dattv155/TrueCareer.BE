using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ConnectionStatusDAO
    {
        public ConnectionStatusDAO()
        {
            MentorMenteeConnections = new HashSet<MentorMenteeConnectionDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MentorMenteeConnectionDAO> MentorMenteeConnections { get; set; }
    }
}
