using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequest_MajorDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public MentorRegisterRequest_MajorDTO() {}

        public MentorRegisterRequest_MajorDTO(Major Major)
        {
            this.Id = Major.Id;
            this.Name = Major.Name;
            this.Description = Major.Description;
            this.Informations = Major.Informations;
            this.Warnings = Major.Warnings;
            this.Errors = Major.Errors;
        }
    }

    public class MentorRegisterRequest_MajorFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }

        public MajorOrder OrderBy { get; set; }
    }
}
