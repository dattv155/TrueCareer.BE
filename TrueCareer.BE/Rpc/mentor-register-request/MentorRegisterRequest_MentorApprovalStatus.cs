using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequest_MentorApprovalStatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public MentorRegisterRequest_MentorApprovalStatusDTO() { }
        public MentorRegisterRequest_MentorApprovalStatusDTO(MentorApprovalStatus MentorApprovalStatus)
        {

            this.Id = MentorApprovalStatus.Id;

            this.Code = MentorApprovalStatus.Code;

            this.Name = MentorApprovalStatus.Name;

            this.Informations = MentorApprovalStatus.Informations;
            this.Warnings = MentorApprovalStatus.Warnings;
            this.Errors = MentorApprovalStatus.Errors;
        }
    }

    public class MentorRegisterRequest_MentorApprovalStatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public MentorApprovalStatusOrder OrderBy { get; set; }
    }
}
