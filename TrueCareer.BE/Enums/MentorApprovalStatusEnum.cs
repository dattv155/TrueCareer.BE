using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;

namespace TrueCareer.Enums
{
    public class MentorApprovalStatusEnum
    {
        public static GenericEnum PENDING = new GenericEnum { Id = 1, Code = "PENDING", Name = "Chờ duyệt" };
        public static GenericEnum APPROVE = new GenericEnum { Id = 2, Code = "APPROVE", Name = "Đã duyệt" };
        public static GenericEnum REJECT = new GenericEnum { Id = 3, Code = "REJECT", Name = "Từ chối" };
        public static List<GenericEnum> MentorApprovalStatusEnumList = new List<GenericEnum>
        {
            PENDING, APPROVE, REJECT
        };
    }
}
