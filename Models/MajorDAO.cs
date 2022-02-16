using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class MajorDAO
    {
        public MajorDAO()
        {
            MbtiPersonalTypeMajorMappings = new HashSet<MbtiPersonalTypeMajorMappingDAO>();
            SchoolMajorMappings = new HashSet<SchoolMajorMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<MbtiPersonalTypeMajorMappingDAO> MbtiPersonalTypeMajorMappings { get; set; }
        public virtual ICollection<SchoolMajorMappingDAO> SchoolMajorMappings { get; set; }
    }
}
