using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class NewsStatusDAO
    {
        public NewsStatusDAO()
        {
            News = new HashSet<NewsDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<NewsDAO> News { get; set; }
    }
}
