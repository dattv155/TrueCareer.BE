using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class InformationTypeDAO
    {
        public InformationTypeDAO()
        {
            Information = new HashSet<InformationDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<InformationDAO> Information { get; set; }
    }
}
