using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.image
{
    public class Image_ImageDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Image_ImageDTO() {}
        public Image_ImageDTO(Image Image)
        {
            this.Id = Image.Id;
            this.Name = Image.Name;
            this.Url = Image.Url;
            this.ThumbnailUrl = Image.ThumbnailUrl;
            this.CreatedAt = Image.CreatedAt;
            this.UpdatedAt = Image.UpdatedAt;
            this.Informations = Image.Informations;
            this.Warnings = Image.Warnings;
            this.Errors = Image.Errors;
        }
    }

    public class Image_ImageFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Url { get; set; }
        public StringFilter ThumbnailUrl { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ImageOrder OrderBy { get; set; }
    }
}
