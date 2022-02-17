using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class AppUserDAO
    {
        public AppUserDAO()
        {
            ActiveTimes = new HashSet<ActiveTimeDAO>();
            AppUserRoleMappings = new HashSet<AppUserRoleMappingDAO>();
            Comments = new HashSet<CommentDAO>();
            FavouriteMentorMentors = new HashSet<FavouriteMentorDAO>();
            FavouriteMentorUsers = new HashSet<FavouriteMentorDAO>();
            FavouriteNews = new HashSet<FavouriteNewsDAO>();
            Information = new HashSet<InformationDAO>();
            MbtiResults = new HashSet<MbtiResultDAO>();
            MentorConnections = new HashSet<MentorConnectionDAO>();
            MentorMenteeConnectionMentees = new HashSet<MentorMenteeConnectionDAO>();
            MentorMenteeConnectionMentors = new HashSet<MentorMenteeConnectionDAO>();
            MentorReviewCreators = new HashSet<MentorReviewDAO>();
            MentorReviewMentors = new HashSet<MentorReviewDAO>();
            News = new HashSet<NewsDAO>();
            NotificationRecipients = new HashSet<NotificationDAO>();
            NotificationSenders = new HashSet<NotificationDAO>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public long SexId { get; set; }
        public DateTime? Birthday { get; set; }
        public string Avatar { get; set; }
        public string CoverImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual SexDAO Sex { get; set; }
        public virtual ICollection<ActiveTimeDAO> ActiveTimes { get; set; }
        public virtual ICollection<AppUserRoleMappingDAO> AppUserRoleMappings { get; set; }
        public virtual ICollection<CommentDAO> Comments { get; set; }
        public virtual ICollection<FavouriteMentorDAO> FavouriteMentorMentors { get; set; }
        public virtual ICollection<FavouriteMentorDAO> FavouriteMentorUsers { get; set; }
        public virtual ICollection<FavouriteNewsDAO> FavouriteNews { get; set; }
        public virtual ICollection<InformationDAO> Information { get; set; }
        public virtual ICollection<MbtiResultDAO> MbtiResults { get; set; }
        public virtual ICollection<MentorConnectionDAO> MentorConnections { get; set; }
        public virtual ICollection<MentorMenteeConnectionDAO> MentorMenteeConnectionMentees { get; set; }
        public virtual ICollection<MentorMenteeConnectionDAO> MentorMenteeConnectionMentors { get; set; }
        public virtual ICollection<MentorReviewDAO> MentorReviewCreators { get; set; }
        public virtual ICollection<MentorReviewDAO> MentorReviewMentors { get; set; }
        public virtual ICollection<NewsDAO> News { get; set; }
        public virtual ICollection<NotificationDAO> NotificationRecipients { get; set; }
        public virtual ICollection<NotificationDAO> NotificationSenders { get; set; }
    }
}
