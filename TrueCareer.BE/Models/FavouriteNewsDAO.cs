using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class FavouriteNewsDAO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long NewsId { get; set; }

        public virtual NewsDAO News { get; set; }
        public virtual AppUserDAO User { get; set; }
    }
}
