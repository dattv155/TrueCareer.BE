using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class FavouriteMentorDAO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long MentorId { get; set; }

        public virtual AppUserDAO Mentor { get; set; }
        public virtual AppUserDAO User { get; set; }
    }
}
