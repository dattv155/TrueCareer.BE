using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class MbtiPersonalTypeEnum
    {
        public static GenericEnum ISTJ = new GenericEnum { Id = 1, Code = "ISTJ", Name = "Người trách nhiệm", Value = "" };
        public static GenericEnum ISFJ = new GenericEnum { Id = 2, Code = "ISFJ", Name = "Người nuôi dưỡng", Value = "" };
        public static GenericEnum ISFP = new GenericEnum { Id = 3, Code = "ISFP", Name = "Người nghệ sĩ", Value = "" };
        public static GenericEnum ISTP = new GenericEnum { Id = 4, Code = "ISTP", Name = "Nhà kỹ thuật", Value = "" };
        public static GenericEnum INFP = new GenericEnum { Id = 5, Code = "INFP", Name = "Người lý tưởng hóa", Value = "" };
        public static GenericEnum INFJ = new GenericEnum { Id = 6, Code = "INFJ", Name = "Người che chở", Value = "" };
        public static GenericEnum INTJ = new GenericEnum { Id = 7, Code = "INTJ", Name = "Nhà khoa học", Value = "" };
        public static GenericEnum INTP = new GenericEnum { Id = 8, Code = "INTP", Name = "Nhà tư duy", Value = "" };
        public static GenericEnum ENFJ = new GenericEnum { Id = 9, Code = "ENFJ", Name = "Người cho đi", Value = "" };
        public static GenericEnum ENFP = new GenericEnum { Id = 10, Code = "ENFP", Name = "Người truyền cảm hứng", Value = "" };
        public static GenericEnum ENTJ = new GenericEnum { Id = 11, Code = "ENTJ", Name = "Nhà điều hành", Value = "" };
        public static GenericEnum ENTP = new GenericEnum { Id = 12, Code = "ENTP", Name = "Người nhìn xa", Value = "" };
        public static GenericEnum ESFJ = new GenericEnum { Id = 13, Code = "ESFJ", Name = "Người quan tâm", Value = "" };
        public static GenericEnum ESFP = new GenericEnum { Id = 14, Code = "ESFP", Name = "Người trình diễn", Value = "" };
        public static GenericEnum ESTJ = new GenericEnum { Id = 15, Code = "ESTJ", Name = "Người giám hộ", Value = "" };
        public static GenericEnum ESTP = new GenericEnum { Id = 16, Code = "ESTP", Name = "Người thực thi", Value = "" };
        public static List<GenericEnum> MbtiPersonalTypeEnumList = new List<GenericEnum>
        {
            ISTJ, ISFJ, ISFP, ISTP, INFP, INFJ, INTJ, INTP, ENFJ, ENFP, ENTJ, ENTP, ESFJ, ESFP, ESTJ, ESTP
        };
    }
}
