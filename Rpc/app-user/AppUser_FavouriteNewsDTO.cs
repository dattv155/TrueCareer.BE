using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_FavouriteNewsDTO : DataDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long NewsId { get; set; }
        public AppUser_NewsDTO News { get; set; }   
        public AppUser_FavouriteNewsDTO() {}
        public AppUser_FavouriteNewsDTO(FavouriteNews FavouriteNews)
        {
            this.Id = FavouriteNews.Id;
            this.UserId = FavouriteNews.UserId;
            this.NewsId = FavouriteNews.NewsId;
            this.News = FavouriteNews.News == null ? null : new AppUser_NewsDTO(FavouriteNews.News);
            this.Errors = FavouriteNews.Errors;
        }
    }

    public class AppUser_FavouriteNewsFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter UserId { get; set; }
        
        public IdFilter NewsId { get; set; }
        
        public FavouriteNewsOrder OrderBy { get; set; }
    }
}