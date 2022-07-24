using TrueSight.Common;
using TrueCareer.Entities;
namespace TrueCareer.Rpc.mentor
{
    public class Mentor_MentorDTO : DataDTO
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public string JobRole { get; set; }
        public long LikeCount { get; set; }
        public long MenteeCount { get; set; }
        public string CompanyName { get; set; }

        public Mentor_MentorDTO(AppUser AppUser)
        {

            this.Id = AppUser.Id;
            this.DisplayName = AppUser.DisplayName;
            this.Avatar = AppUser.Avatar;
            this.JobRole = AppUser.JobRole;
            this.LikeCount = AppUser.LikeCount;
            this.MenteeCount = AppUser.MenteeCount;
            this.CompanyName = AppUser.CompanyName;
        }
    }

    public class Mentor_MentorFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter DisplayName { get; set; }

        public IdFilter MajorId { get; set; }

        public StringFilter Address { get; set; }

        public StringFilter JobRole { get; set; }

        public StringFilter CompanyName { get; set; }
        public IdFilter RoleId { get; set; }
    }
}