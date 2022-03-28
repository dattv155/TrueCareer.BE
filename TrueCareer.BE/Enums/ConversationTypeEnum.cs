using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;

namespace TrueCareer.Enums
{
    public class ConversationTypeEnum
    {
        public static GenericEnum LOCAL = new GenericEnum { Id = 1, Code = "LOCAL", Name = "Nội bộ" };
        public static GenericEnum ZALO = new GenericEnum { Id = 2, Code = "ZALO", Name = "Zalo" };
        public static GenericEnum FACEBOOK = new GenericEnum { Id = 3, Code = "FACEBOOK", Name = "Facebook" };

        public static List<GenericEnum> ConversationTypeEnumList = new List<GenericEnum>
        {
            LOCAL, ZALO, FACEBOOK
        };
    }
}
