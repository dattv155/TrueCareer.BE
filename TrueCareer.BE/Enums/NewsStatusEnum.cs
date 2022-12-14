using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class NewsStatusEnum
    {
        public static GenericEnum PENDING = new GenericEnum { Id = 1, Code = "PENDING", Name = "Chờ duyệt" };
        public static GenericEnum APPROVE = new GenericEnum { Id = 2, Code = "APPROVE", Name = "Đã duyệt" };
        public static GenericEnum REJECT = new GenericEnum { Id = 3, Code = "REJECT", Name = "Từ chối" };
        public static List<GenericEnum> NewsStatusEnumList = new List<GenericEnum>
        {
            PENDING, APPROVE, REJECT
        };
    }
}
