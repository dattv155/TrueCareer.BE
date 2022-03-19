using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class MbtiPersonalTypeDAO
    {
        public MbtiPersonalTypeDAO()
        {
            MbtiPersonalTypeMajorMappings = new HashSet<MbtiPersonalTypeMajorMappingDAO>();
            MbtiResults = new HashSet<MbtiResultDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }

        public virtual ICollection<MbtiPersonalTypeMajorMappingDAO> MbtiPersonalTypeMajorMappings { get; set; }
        public virtual ICollection<MbtiResultDAO> MbtiResults { get; set; }
    }
}
