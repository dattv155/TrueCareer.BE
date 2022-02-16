using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.favourite_news
{
    public class FavouriteNews_NewsDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long CreatorId { get; set; }
        
        public string NewsContent { get; set; }
        
        public long LikeCounting { get; set; }
        
        public long WatchCounting { get; set; }
        
        public long NewsStatusId { get; set; }
        
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public FavouriteNews_NewsDTO() {}
        public FavouriteNews_NewsDTO(News News)
        {
            
            this.Id = News.Id;
            
            this.CreatorId = News.CreatorId;
            
            this.NewsContent = News.NewsContent;
            
            this.LikeCounting = News.LikeCounting;
            
            this.WatchCounting = News.WatchCounting;
            
            this.NewsStatusId = News.NewsStatusId;
            
            this.RowId = News.RowId;
            this.CreatedAt = News.CreatedAt;
            this.UpdatedAt = News.UpdatedAt;
            this.Informations = News.Informations;
            this.Warnings = News.Warnings;
            this.Errors = News.Errors;
        }
    }

    public class FavouriteNews_NewsFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter CreatorId { get; set; }
        
        public StringFilter NewsContent { get; set; }
        
        public LongFilter LikeCounting { get; set; }
        
        public LongFilter WatchCounting { get; set; }
        
        public IdFilter NewsStatusId { get; set; }
        
        public NewsOrder OrderBy { get; set; }
    }
}