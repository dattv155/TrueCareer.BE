using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class AppUserRoleMapping : DataEntity,  IEquatable<AppUserRoleMapping>
    {
        public long AppUserId { get; set; }
        public long RoleId { get; set; }
        public AppUser AppUser { get; set; }
        public Role Role { get; set; }
        
        public bool Equals(AppUserRoleMapping other)
        {
            if (other == null) return false;
            if (this.AppUserId != other.AppUserId) return false;
            if (this.RoleId != other.RoleId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class AppUserRoleMappingFilter : FilterEntity
    {
        public IdFilter AppUserId { get; set; }
        public IdFilter RoleId { get; set; }
        public List<AppUserRoleMappingFilter> OrFilter { get; set; }
        public AppUserRoleMappingOrder OrderBy {get; set;}
        public AppUserRoleMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AppUserRoleMappingOrder
    {
        AppUser = 0,
        Role = 1,
    }

    [Flags]
    public enum AppUserRoleMappingSelect:long
    {
        ALL = E.ALL,
        AppUser = E._0,
        Role = E._1,
    }
}
