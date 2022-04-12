using TrueSight.Common;
using TrueCareer.Entities;
using System;

namespace TrueCareer.Rpc.mentor_register_request {
     public class MentorRegisterRequest_ActiveTimeDTO : DataDTO
    {
        public long Id { get; set; }
        public long UnitOfTimeId { get; set; }
        public DateTime ActiveDate { get; set; }
        public long MentorId { get; set; }
        public MentorRegisterRequest_AppUserDTO Mentor { get; set; }
        public MentorRegisterRequest_UnitOfTimeDTO UnitOfTime { get; set; }
        public MentorRegisterRequest_ActiveTimeDTO() {}
        public MentorRegisterRequest_ActiveTimeDTO(ActiveTime ActiveTime)
        {
            this.Id = ActiveTime.Id;
            this.ActiveDate = ActiveTime.ActiveDate;
            this.UnitOfTimeId = ActiveTime.UnitOfTimeId;
            this.UnitOfTime = ActiveTime.UnitOfTime == null ? null : new MentorRegisterRequest_UnitOfTimeDTO(ActiveTime.UnitOfTime);
            this.MentorId = ActiveTime.MentorId;
            this.Mentor = ActiveTime.Mentor == null ? null : new MentorRegisterRequest_AppUserDTO(ActiveTime.Mentor);
            this.Informations = ActiveTime.Informations;
            this.Warnings = ActiveTime.Warnings;
            this.Errors = ActiveTime.Errors;
        }
    }

    public class MentorRegisterRequest_ActiveTimeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public DateFilter ActiveDate { get; set; }
        public IdFilter UnitOfTimeId { get; set; }
        public IdFilter MentorId { get; set; }
        public ActiveTimeOrder OrderBy { get; set; }
    }
}