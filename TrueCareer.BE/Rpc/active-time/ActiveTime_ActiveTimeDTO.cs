using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.active_time
{
    public class ActiveTime_ActiveTimeDTO : DataDTO
    {
        public long Id { get; set; }
        public long UnitOfTimeId { get; set; }
        public DateTime ActiveDate { get; set; }
        public long MentorId { get; set; }
        public ActiveTime_AppUserDTO Mentor { get; set; }
        public ActiveTime_UnitOfTimeDTO UnitOfTime { get; set; }
        public ActiveTime_ActiveTimeDTO() {}
        public ActiveTime_ActiveTimeDTO(ActiveTime ActiveTime)
        {
            this.Id = ActiveTime.Id;
            this.ActiveDate = ActiveTime.ActiveDate;
            this.UnitOfTimeId = ActiveTime.UnitOfTimeId;
            this.UnitOfTime = ActiveTime.UnitOfTime == null ? null : new ActiveTime_UnitOfTimeDTO(ActiveTime.UnitOfTime);
            this.MentorId = ActiveTime.MentorId;
            this.Mentor = ActiveTime.Mentor == null ? null : new ActiveTime_AppUserDTO(ActiveTime.Mentor);
            this.Informations = ActiveTime.Informations;
            this.Warnings = ActiveTime.Warnings;
            this.Errors = ActiveTime.Errors;
        }
    }

    public class ActiveTime_ActiveTimeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public DateFilter ActiveDate { get; set; }
        public IdFilter UnitOfTimeId { get; set; }
        public IdFilter MentorId { get; set; }
        public ActiveTimeOrder OrderBy { get; set; }
    }
}
