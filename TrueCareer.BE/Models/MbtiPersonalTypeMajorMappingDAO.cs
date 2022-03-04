using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class MbtiPersonalTypeMajorMappingDAO
    {
        public long MbtiPersonalTypeId { get; set; }
        public long MajorId { get; set; }

        public virtual MajorDAO Major { get; set; }
        public virtual MbtiPersonalTypeDAO MbtiPersonalType { get; set; }
    }
}
