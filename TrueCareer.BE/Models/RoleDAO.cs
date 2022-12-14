using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class RoleDAO
    {
        public RoleDAO()
        {
            AppUsers = new HashSet<AppUserDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
    }
}
