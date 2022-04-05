using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
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
        public decimal? Rating { get; set; }
        /// <summary>
        /// Số năm học
        /// </summary>
        public string CompleteTime { get; set; }
        public long? StudentCount { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string SchoolImage { get; set; }

        public virtual ICollection<SchoolMajorMappingDAO> SchoolMajorMappings { get; set; }
    }
}
