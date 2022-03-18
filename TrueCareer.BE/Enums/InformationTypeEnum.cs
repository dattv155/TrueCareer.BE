using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class InformationTypeEnum
    {
        public static GenericEnum DESCRIPTION = new GenericEnum { Id = 1, Code = "DESCRIPTION", Name = "Giới thiệu bản thân" };
        public static GenericEnum EXPERIENCE = new GenericEnum { Id = 2, Code = "EXPERIENCE", Name = "Kinh nghiệm làm việc" };
        public static GenericEnum STUDY_PROGRESS = new GenericEnum { Id = 3, Code = "STUDY_PROGRESS", Name = "Qúa trình học tập" };
        public static GenericEnum EXTRACURRICULAR_ACTIVITY = new GenericEnum { Id = 4, Code = "EXTRACURRICULAR_ACTIVITY", Name = "Hoạt động ngoại khóa" };
        public static GenericEnum REWARD = new GenericEnum { Id = 5, Code = "REWARD", Name = "Giải thưởng" };
        public static GenericEnum SKILL_CERTIFICATE = new GenericEnum { Id = 6, Code = "SKILL_CERTIFICATE", Name = "Kỹ năng và chứng chỉ" };
        public static List<GenericEnum> InformationTypeEnumList = new List<GenericEnum>
        {
            DESCRIPTION, EXPERIENCE, STUDY_PROGRESS, EXTRACURRICULAR_ACTIVITY, REWARD, SKILL_CERTIFICATE, 
        };
    }
}
