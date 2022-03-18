using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class MbtiSingleTypeEnum
    {
        public static GenericEnum Extroverts = new GenericEnum { Id = 1, Code = "E", Name = "Người hướng ngoại" };
        public static GenericEnum Sensors = new GenericEnum { Id = 2, Code = "S", Name = "Người thực tế" };
        public static GenericEnum Thinkers = new GenericEnum { Id = 3, Code = "T", Name = "Người suy tính" };
        public static GenericEnum Judgers = new GenericEnum { Id = 4, Code = "J", Name = "Người cứng nhắc" };
        public static GenericEnum Introverts = new GenericEnum { Id = 5, Code = "I", Name = "Người hướng nội" };
        public static GenericEnum Intuitives = new GenericEnum { Id = 6, Code = "N", Name = "Người trực quan" };
        public static GenericEnum Feelers = new GenericEnum { Id = 7, Code = "F", Name = "Người cảm tính" };
        public static GenericEnum Perceivers = new GenericEnum { Id = 8, Code = "P", Name = "Người linh hoạt" };
        public static List<GenericEnum> MbtiSingleTypeEnumList = new List<GenericEnum>
        {
            Extroverts, Sensors, Thinkers, Judgers, Introverts, Intuitives, Feelers, Perceivers
        };
    }
}
