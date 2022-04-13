using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class ConnectionStatusEnum
    {
        public static GenericEnum PENDING = new GenericEnum { Id = 1, Code = "PENDING", Name = "Chờ duyệt" };
        public static GenericEnum COMING_SOON = new GenericEnum { Id = 2, Code = "COMING_SOON", Name = "Sắp diễn ra" };
        public static GenericEnum REJECTED = new GenericEnum { Id = 3, Code = "REJECTED", Name = "Từ chối" };
        public static GenericEnum COMPLETED = new GenericEnum { Id = 4, Code = "COMPLETED", Name = "Hoàn thành" };
        public static GenericEnum IN_PROGRESS = new GenericEnum { Id = 5, Code = "IN_PROGESS", Name = "Đang diễn ra" };
        public static GenericEnum CANCEL = new GenericEnum { Id = 6, Code = "CANCEL", Name = "Đã hủy" };

        public static List<GenericEnum> ConnectionStatusEnumList = new List<GenericEnum>
        {
            PENDING, COMING_SOON, REJECTED, COMPLETED, IN_PROGRESS, CANCEL
        };
    }
}
