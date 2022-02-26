using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ImageDAO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
