using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.BE.Entities;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequest_MentorInfoDTO : DataDTO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long ConnectionId { get; set; }
        public string ConnectionUrl { get; set; }
        public long MajorId { get; set; }
        public string TopicDescription { get; set; }
        public List<MentorRegisterRequest_ActiveTimeDTO> ActiveTimes { get; set; }
        public MentorRegisterRequest_AppUserDTO User { get; set; }
        public MentorRegisterRequest_MajorDTO Topic { get; set; }
        public MentorRegisterRequest_ConnectionTypeDTO Connection { get; set; }
        public MentorRegisterRequest_MentorApprovalStatusDTO MentorApprovalStatus { get; set; }
        public MentorRegisterRequest_MentorInfoDTO() { }
        public MentorRegisterRequest_MentorInfoDTO(MentorInfo MentorInfo)
        {
            this.Id = MentorInfo.Id;
            this.AppUserId = MentorInfo.AppUserId;
        }
    }

    public class MentorRegisterRequest_MentorInfoFilterDTO : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter TopicId { get; set; }
        public IdFilter UserId { get; set; }
        public IdFilter MentorApprovalStatusId { get; set; }
        public MentorRegisterRequestOrder OrderBy { get; set; }
    }
}

