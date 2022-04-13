using TrueSight.Common;
using TrueCareer.Entities;
using System;

namespace TrueCareer.Rpc.mentor_mentee_connection {
    public class MentorMenteeConnection_MentorReviewDTO : DataDTO {
        public long Id { get; set; }
        public string Description { get; set; }
        public string ContentReview { get; set; }
        public long Star { get; set; }
        public long MentorId { get; set; }
        public long CreatorId { get; set; }
        public DateTime Time { get; set; }
        public MentorMenteeConnection_AppUserDTO Creator { get; set; }
        public MentorMenteeConnection_AppUserDTO Mentor { get; set; }
        public MentorMenteeConnection_MentorReviewDTO() {}
        public MentorMenteeConnection_MentorReviewDTO(MentorReview MentorReview)
        {
            this.Id = MentorReview.Id;
            this.Description = MentorReview.Description;
            this.ContentReview = MentorReview.ContentReview;
            this.Star = MentorReview.Star;
            this.MentorId = MentorReview.MentorId;
            this.CreatorId = MentorReview.CreatorId;
            this.Time = MentorReview.Time;
            this.Creator = MentorReview.Creator == null ? null : new MentorMenteeConnection_AppUserDTO(MentorReview.Creator);
            this.Mentor = MentorReview.Mentor == null ? null : new MentorMenteeConnection_AppUserDTO(MentorReview.Mentor);
            this.Informations = MentorReview.Informations;
            this.Warnings = MentorReview.Warnings;
            this.Errors = MentorReview.Errors;
        }
    }
}