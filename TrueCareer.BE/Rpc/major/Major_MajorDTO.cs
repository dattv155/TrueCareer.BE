using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.major
{
    public class Major_MajorDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string MajorImage { get; set; }
        public Major_MajorDTO() { }
        public Major_MajorDTO(Major Major)
        {
            this.Id = Major.Id;
            this.Name = Major.Name;
            this.Description = Major.Description;
            this.MajorImage = Major.MajorImage;
            this.Informations = Major.Informations;
            this.Warnings = Major.Warnings;
            this.Errors = Major.Errors;
        }
    }

    public class Major_MajorFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public MajorOrder OrderBy { get; set; }
    }
}
