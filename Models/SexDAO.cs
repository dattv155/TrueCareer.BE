using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class SexDAO
    {
        public SexDAO()
        {
            AppUsers = new HashSet<AppUserDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
    }
}
