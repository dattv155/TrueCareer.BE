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
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public long MentorId { get; set; }
        public ActiveTime_AppUserDTO Mentor { get; set; }
        public ActiveTime_ActiveTimeDTO() {}
        public ActiveTime_ActiveTimeDTO(ActiveTime ActiveTime)
        {
            this.Id = ActiveTime.Id;
            this.StartAt = ActiveTime.StartAt;
            this.EndAt = ActiveTime.EndAt;
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
        public DateFilter StartAt { get; set; }
        public DateFilter EndAt { get; set; }
        public IdFilter MentorId { get; set; }
        public ActiveTimeOrder OrderBy { get; set; }
    }
}
