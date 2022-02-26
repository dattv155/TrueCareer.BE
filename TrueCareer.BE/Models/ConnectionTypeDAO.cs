using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ConnectionTypeDAO
    {
        public ConnectionTypeDAO()
        {
            MentorConnections = new HashSet<MentorConnectionDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<MentorConnectionDAO> MentorConnections { get; set; }
    }
}
