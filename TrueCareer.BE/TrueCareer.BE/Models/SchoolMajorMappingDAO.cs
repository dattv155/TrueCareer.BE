using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class SchoolMajorMappingDAO
    {
        public long SchoolId { get; set; }
        public long MajorId { get; set; }

        public virtual MajorDAO Major { get; set; }
        public virtual SchoolDAO School { get; set; }
    }
}
