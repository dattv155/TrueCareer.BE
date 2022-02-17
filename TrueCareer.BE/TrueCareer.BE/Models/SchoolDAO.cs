using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class SchoolDAO
    {
        public SchoolDAO()
        {
            SchoolMajorMappings = new HashSet<SchoolMajorMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid RowId { get; set; }

        public virtual ICollection<SchoolMajorMappingDAO> SchoolMajorMappings { get; set; }
    }
}
