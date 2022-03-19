using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.information
{
    public class Information_MentorReviewDTO:DataDTO
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string ContentReview { get; set; }
        public long Star { get; set; }
        public long MentorId { get; set; }
        public long CreatorId { get; set; }
        public DateTime Time { get; set; }
        public Information_AppUserDTO Creator { get; set; }
        public Information_AppUserDTO Mentor { get; set; }
        public Information_MentorReviewDTO() { }
        public Information_MentorReviewDTO(MentorReview MentorReview)
        {
            this.Id = MentorReview.Id;
            this.Description = MentorReview.Description;
            this.ContentReview = MentorReview.ContentReview;
            this.Star = MentorReview.Star;
            this.MentorId = MentorReview.MentorId;
            this.CreatorId = MentorReview.CreatorId;
            this.Time = MentorReview.Time;
            this.Creator = MentorReview.Creator == null ? null : new Information_AppUserDTO(MentorReview.Creator);
            this.Mentor = MentorReview.Mentor == null ? null : new Information_AppUserDTO(MentorReview.Mentor);
            this.Informations = MentorReview.Informations;
            this.Warnings = MentorReview.Warnings;
            this.Errors = MentorReview.Errors;
        }
    }

    public class Information_MentorReviewFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Description { get; set; }
        public StringFilter ContentReview { get; set; }
        public LongFilter Star { get; set; }
        public IdFilter MentorId { get; set; }
        public IdFilter CreatorId { get; set; }
        public DateFilter Time { get; set; }
        public MentorReviewOrder OrderBy { get; set; }
    }
}
