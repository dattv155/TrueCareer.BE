using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequest_AppUserDTO : DataDTO
    {

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

        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public MentorRegisterRequest_AppUserDTO() { }
        public MentorRegisterRequest_AppUserDTO(AppUser AppUser)
        {

            this.Id = AppUser.Id;

            this.Username = AppUser.Username;

            this.Email = AppUser.Email;

            this.Phone = AppUser.Phone;

            this.Password = AppUser.Password;

            this.DisplayName = AppUser.DisplayName;

            this.SexId = AppUser.SexId;

            this.Birthday = AppUser.Birthday;

            this.Avatar = AppUser.Avatar;

            this.CoverImage = AppUser.CoverImage;

            this.RowId = AppUser.RowId;
            this.CreatedAt = AppUser.CreatedAt;
            this.UpdatedAt = AppUser.UpdatedAt;
            this.Informations = AppUser.Informations;
            this.Warnings = AppUser.Warnings;
            this.Errors = AppUser.Errors;
        }
    }

    public class MentorRegisterRequest_AppUserFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Username { get; set; }

        public StringFilter Email { get; set; }

        public StringFilter Phone { get; set; }

        public StringFilter Password { get; set; }

        public StringFilter DisplayName { get; set; }

        public IdFilter SexId { get; set; }

        public DateFilter Birthday { get; set; }

        public StringFilter Avatar { get; set; }

        public StringFilter CoverImage { get; set; }

        public AppUserOrder OrderBy { get; set; }
    }
}
