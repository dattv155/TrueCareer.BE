using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class ConnectionStatusEnum
    {
        public static GenericEnum A1 = new GenericEnum { Id = 1, Code = "", Name = "" };
        public static List<GenericEnum> ConnectionStatusEnumList = new List<GenericEnum>
        {
            A1,
        };
    }
}
