using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class UnitOfTimeDAO
    {
        public UnitOfTimeDAO()
        {
            ActiveTimes = new HashSet<ActiveTimeDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? StartAt { get; set; }
        public long? EndAt { get; set; }

        public virtual ICollection<ActiveTimeDAO> ActiveTimes { get; set; }
    }
}
