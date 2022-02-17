using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class ConnectionTypeEnum
    {
        public static GenericEnum A1 = new GenericEnum { Id = 1, Code = "", Name = "" };
        public static List<GenericEnum> ConnectionTypeEnumList = new List<GenericEnum>
        {
            A1,
        };
    }
}
