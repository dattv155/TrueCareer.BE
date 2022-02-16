using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.favourite_news
{
    public class FavouriteNews_FavouriteNewsDTO : DataDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long NewsId { get; set; }
        public FavouriteNews_NewsDTO News { get; set; }
        public FavouriteNews_AppUserDTO User { get; set; }
        public FavouriteNews_FavouriteNewsDTO() {}
        public FavouriteNews_FavouriteNewsDTO(FavouriteNews FavouriteNews)
        {
            this.Id = FavouriteNews.Id;
            this.UserId = FavouriteNews.UserId;
            this.NewsId = FavouriteNews.NewsId;
            this.News = FavouriteNews.News == null ? null : new FavouriteNews_NewsDTO(FavouriteNews.News);
            this.User = FavouriteNews.User == null ? null : new FavouriteNews_AppUserDTO(FavouriteNews.User);
            this.Informations = FavouriteNews.Informations;
            this.Warnings = FavouriteNews.Warnings;
            this.Errors = FavouriteNews.Errors;
        }
    }

    public class FavouriteNews_FavouriteNewsFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter UserId { get; set; }
        public IdFilter NewsId { get; set; }
        public FavouriteNewsOrder OrderBy { get; set; }
    }
}
