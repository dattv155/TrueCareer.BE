using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequest_MentorRegisterRequestDTO:DataDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long MentorApprovalStatusId { get; set; }
        public long TopicId { get; set; }
        public MentorRegisterRequest_AppUserDTO User { get; set; }
        public MentorRegisterRequest_TopicDTO Topic { get; set; }
        public MentorRegisterRequest_MentorApprovalStatusDTO MentorApprovalStatus { get; set; }
        public MentorRegisterRequest_MentorRegisterRequestDTO() { }
        public MentorRegisterRequest_MentorRegisterRequestDTO(MentorRegisterRequest MentorRegisterRequest)
        {
            this.Id = MentorRegisterRequest.Id;
            this.UserId = MentorRegisterRequest.UserId;
            this.MentorApprovalStatusId = MentorRegisterRequest.MentorApprovalStatusId;
            this.TopicId = MentorRegisterRequest.TopicId;
        }
    }

    public class MentorRegisterRequest_MentorRegisterRequestFilterDTO:FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter TopicId { get; set; }
        public IdFilter UserId { get; set; }
        public IdFilter MentorApprovalStatusId { get; set; }
        public MentorRegisterRequestOrder OrderBy { get; set; }
    }
}
