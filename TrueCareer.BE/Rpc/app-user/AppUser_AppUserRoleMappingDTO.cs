using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_AppUserRoleMappingDTO:DataDTO
    {
        public long AppUserId { get; set; }
        public long RoleId { get; set; }
        public AppUser_RoleDTO Role { get; set; }

        public AppUser_AppUserRoleMappingDTO() { }
        public AppUser_AppUserRoleMappingDTO(AppUserRoleMapping AppUserRoleMapping)
        {
            this.AppUserId = AppUserRoleMapping.AppUserId;
            this.RoleId = AppUserRoleMapping.RoleId;
            this.Role = AppUserRoleMapping.Role == null ? null : new AppUser_RoleDTO(AppUserRoleMapping.Role);
        }
    }

    public class AppUser_AppUserRoleMappingFilterDTO : FilterDTO
    {

        public IdFilter AppUserId { get; set; }

        public IdFilter RoleId { get; set; }

        public AppUserRoleMappingOrder OrderBy { get; set; }
    }
}
