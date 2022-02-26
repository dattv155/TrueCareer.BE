using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class MbtiResultDAO
    {
        public long UserId { get; set; }
        public long MbtiPersonalTypeId { get; set; }
        public long Id { get; set; }

        public virtual MbtiPersonalTypeDAO MbtiPersonalType { get; set; }
        public virtual AppUserDAO User { get; set; }
    }
}
