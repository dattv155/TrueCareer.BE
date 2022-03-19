using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class RoleEnum
    {
        public static GenericEnum ADMIN = new GenericEnum { Id = 1, Code = "ADMIN", Name = "Admin" };
        public static GenericEnum MENTOR = new GenericEnum { Id = 2, Code = "MENTOR", Name = "Mentor" };
        public static GenericEnum MENTEE = new GenericEnum { Id = 3, Code = "MENTEE", Name = "Mentee" };
        public static List<GenericEnum> RoleEnumList = new List<GenericEnum>
        {
            ADMIN, MENTOR, MENTEE
        };
    }
}
