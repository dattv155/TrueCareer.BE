using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;

namespace TrueCareer.Enums
{
    public class GlobalUserTypeEnum
    {
        public static GenericEnum LOCAL = new GenericEnum { Id = 1, Code = "LOCAL", Name = "Nội bộ" };

        public static List<GenericEnum> GlobalUserTypeEnumList = new List<GenericEnum>
        {
            LOCAL
        };
    }
}
