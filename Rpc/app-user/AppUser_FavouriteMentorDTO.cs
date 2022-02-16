using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_FavouriteMentorDTO : DataDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long MentorId { get; set; }
        public AppUser_FavouriteMentorDTO() {}
        public AppUser_FavouriteMentorDTO(FavouriteMentor FavouriteMentor)
        {
            this.Id = FavouriteMentor.Id;
            this.UserId = FavouriteMentor.UserId;
            this.MentorId = FavouriteMentor.MentorId;
            this.Errors = FavouriteMentor.Errors;
        }
    }

    public class AppUser_FavouriteMentorFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter UserId { get; set; }
        
        public IdFilter MentorId { get; set; }
        
        public FavouriteMentorOrder OrderBy { get; set; }
    }
}