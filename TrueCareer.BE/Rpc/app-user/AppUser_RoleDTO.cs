using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_RoleDTO:DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public AppUser_RoleDTO() { }
        public AppUser_RoleDTO(Role Role)
        {

            this.Id = Role.Id;

            this.Code = Role.Code;

            this.Name = Role.Name;
        }
    }
    public class AppUser_RoleFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public RoleOrder OrderBy { get; set; }
    }
}
