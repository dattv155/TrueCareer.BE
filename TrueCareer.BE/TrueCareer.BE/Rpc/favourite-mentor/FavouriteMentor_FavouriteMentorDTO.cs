using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.favourite_mentor
{
    public class FavouriteMentor_FavouriteMentorDTO : DataDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long MentorId { get; set; }
        public FavouriteMentor_AppUserDTO Mentor { get; set; }
        public FavouriteMentor_AppUserDTO User { get; set; }
        public FavouriteMentor_FavouriteMentorDTO() {}
        public FavouriteMentor_FavouriteMentorDTO(FavouriteMentor FavouriteMentor)
        {
            this.Id = FavouriteMentor.Id;
            this.UserId = FavouriteMentor.UserId;
            this.MentorId = FavouriteMentor.MentorId;
            this.Mentor = FavouriteMentor.Mentor == null ? null : new FavouriteMentor_AppUserDTO(FavouriteMentor.Mentor);
            this.User = FavouriteMentor.User == null ? null : new FavouriteMentor_AppUserDTO(FavouriteMentor.User);
            this.Informations = FavouriteMentor.Informations;
            this.Warnings = FavouriteMentor.Warnings;
            this.Errors = FavouriteMentor.Errors;
        }
    }

    public class FavouriteMentor_FavouriteMentorFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter UserId { get; set; }
        public IdFilter MentorId { get; set; }
        public FavouriteMentorOrder OrderBy { get; set; }
    }
}
