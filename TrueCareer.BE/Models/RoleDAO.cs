using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class RoleDAO
    {
        public RoleDAO()
        {
            AppUserRoleMappings = new HashSet<AppUserRoleMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<AppUserRoleMappingDAO> AppUserRoleMappings { get; set; }
    }
}
