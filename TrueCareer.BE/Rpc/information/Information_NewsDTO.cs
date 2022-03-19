using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.information
{
    public class Information_NewsDTO : DataDTO
    {
        public long Id { get; set; }
        public long CreatorId { get; set; }
        public string NewsContent { get; set; }
        public long LikeCounting { get; set; }
        public long WatchCounting { get; set; }
        public Information_AppUserDTO Creator { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Information_NewsDTO() { }
        public Information_NewsDTO(News News)
        {
            this.Id = News.Id;
            this.CreatorId = News.CreatorId;
            this.NewsContent = News.NewsContent;
            this.LikeCounting = News.LikeCounting;
            this.WatchCounting = News.WatchCounting;
            this.Creator = News.Creator == null ? null : new Information_AppUserDTO(News.Creator);
            this.RowId = News.RowId;
            this.CreatedAt = News.CreatedAt;
            this.UpdatedAt = News.UpdatedAt;
            this.Informations = News.Informations;
            this.Warnings = News.Warnings;
            this.Errors = News.Errors;
        }
    }

    public class Information_NewsFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter CreatorId { get; set; }
        public StringFilter NewsContent { get; set; }
        public LongFilter LikeCounting { get; set; }
        public LongFilter WatchCounting { get; set; }
        public IdFilter NewsStatusId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public NewsOrder OrderBy { get; set; }
    }
}
