using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class SexEnum
    {
        public static GenericEnum FEMALE = new GenericEnum { Id = 1, Code = "FEMALE", Name = "Nữ" };
        public static GenericEnum MALE = new GenericEnum { Id = 2, Code = "MALE", Name = "Nam" };
        public static GenericEnum OTHER = new GenericEnum { Id = 3, Code = "OTHER", Name = "Khác" };
        public static List<GenericEnum> SexEnumList = new List<GenericEnum>
        {
            FEMALE, MALE, OTHER
        };
    }
}
