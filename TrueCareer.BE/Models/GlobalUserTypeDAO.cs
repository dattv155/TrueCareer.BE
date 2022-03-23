using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class GlobalUserTypeDAO
    {
        public GlobalUserTypeDAO()
        {
            GlobalUsers = new HashSet<GlobalUserDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<GlobalUserDAO> GlobalUsers { get; set; }
    }
}
