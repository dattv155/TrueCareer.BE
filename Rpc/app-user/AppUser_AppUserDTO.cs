using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_AppUserDTO : DataDTO
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
        public AppUser_SexDTO Sex { get; set; }
        public List<AppUser_FavouriteMentorDTO> FavouriteMentorMentors { get; set; }
        public List<AppUser_FavouriteMentorDTO> FavouriteMentorUsers { get; set; }
        public List<AppUser_FavouriteNewsDTO> FavouriteNews { get; set; }
        public List<AppUser_InformationDTO> Information { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AppUser_AppUserDTO() {}
        public AppUser_AppUserDTO(AppUser AppUser)
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
            this.Sex = AppUser.Sex == null ? null : new AppUser_SexDTO(AppUser.Sex);
            this.FavouriteMentorMentors = AppUser.FavouriteMentorMentors?.Select(x => new AppUser_FavouriteMentorDTO(x)).ToList();
            this.FavouriteMentorUsers = AppUser.FavouriteMentorUsers?.Select(x => new AppUser_FavouriteMentorDTO(x)).ToList();
            this.FavouriteNews = AppUser.FavouriteNews?.Select(x => new AppUser_FavouriteNewsDTO(x)).ToList();
            this.Information = AppUser.Information?.Select(x => new AppUser_InformationDTO(x)).ToList();
            this.RowId = AppUser.RowId;
            this.CreatedAt = AppUser.CreatedAt;
            this.UpdatedAt = AppUser.UpdatedAt;
            this.Informations = AppUser.Informations;
            this.Warnings = AppUser.Warnings;
            this.Errors = AppUser.Errors;
        }
    }

    public class AppUser_AppUserFilterDTO : FilterDTO
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
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public AppUserOrder OrderBy { get; set; }
    }
}
