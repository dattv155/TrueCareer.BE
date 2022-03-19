using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class ConnectionTypeEnum
    {
        public static GenericEnum GOOGLE_MEET = new GenericEnum { Id = 1, Code = "GOOGLE_MEET", Name = "Google Meet" };
        public static GenericEnum ZOOM = new GenericEnum { Id = 2, Code = "ZOOM", Name = "Zoom" };
        public static GenericEnum OTHER = new GenericEnum { Id = 3, Code = "OTHER", Name = "Other" };
        public static List<GenericEnum> ConnectionTypeEnumList = new List<GenericEnum>
        {
            GOOGLE_MEET, ZOOM, OTHER
        };
    }
}
