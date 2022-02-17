using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.mentor_review
{
    public class MentorReview_MentorReviewDTO : DataDTO
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string ContentReview { get; set; }
        public long Star { get; set; }
        public long MentorId { get; set; }
        public long CreatorId { get; set; }
        public DateTime Time { get; set; }
        public MentorReview_AppUserDTO Creator { get; set; }
        public MentorReview_AppUserDTO Mentor { get; set; }
        public MentorReview_MentorReviewDTO() {}
        public MentorReview_MentorReviewDTO(MentorReview MentorReview)
        {
            this.Id = MentorReview.Id;
            this.Description = MentorReview.Description;
            this.ContentReview = MentorReview.ContentReview;
            this.Star = MentorReview.Star;
            this.MentorId = MentorReview.MentorId;
            this.CreatorId = MentorReview.CreatorId;
            this.Time = MentorReview.Time;
            this.Creator = MentorReview.Creator == null ? null : new MentorReview_AppUserDTO(MentorReview.Creator);
            this.Mentor = MentorReview.Mentor == null ? null : new MentorReview_AppUserDTO(MentorReview.Mentor);
            this.Informations = MentorReview.Informations;
            this.Warnings = MentorReview.Warnings;
            this.Errors = MentorReview.Errors;
        }
    }

    public class MentorReview_MentorReviewFilterDTO : FilterDTO
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
