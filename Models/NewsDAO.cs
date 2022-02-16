using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class NewsDAO
    {
        public NewsDAO()
        {
            FavouriteNews = new HashSet<FavouriteNewsDAO>();
        }

        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatorId { get; set; }
        public string NewsContent { get; set; }
        public long LikeCounting { get; set; }
        public long WatchCounting { get; set; }
        public long NewsStatusId { get; set; }
        public Guid RowId { get; set; }

        public virtual AppUserDAO Creator { get; set; }
        public virtual NewsStatusDAO NewsStatus { get; set; }
        public virtual ICollection<FavouriteNewsDAO> FavouriteNews { get; set; }
    }
}
