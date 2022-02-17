using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class TopicDAO
    {
        public TopicDAO()
        {
            Information = new HashSet<InformationDAO>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }

        public virtual ICollection<InformationDAO> Information { get; set; }
    }
}
