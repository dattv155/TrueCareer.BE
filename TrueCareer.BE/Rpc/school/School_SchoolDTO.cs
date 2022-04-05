using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.school
{
    public class School_SchoolDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Rating { get; set; }
        public string CompleteTime { get; set; }
        public long? StudentCount { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string SchoolImage { get; set; }
        public Guid RowId { get; set; }
        public School_SchoolDTO() {}
        public School_SchoolDTO(School School)
        {
            this.Id = School.Id;
            this.Name = School.Name;
            this.Description = School.Description;
            this.Rating = School.Rating;
            this.CompleteTime = School.CompleteTime;
            this.StudentCount = School.StudentCount;
            this.PhoneNumber = School.PhoneNumber;
            this.Address = School.Address;
            this.SchoolImage = School.SchoolImage;
            this.RowId = School.RowId;
            this.Informations = School.Informations;
            this.Warnings = School.Warnings;
            this.Errors = School.Errors;
        }
    }

    public class School_SchoolFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public DecimalFilter Rating { get; set; }
        public StringFilter CompleteTime { get; set; }
        public LongFilter StudentCount { get; set; }
        public StringFilter PhoneNumber { get; set; }
        public StringFilter Address { get; set; }
        public StringFilter SchoolImage { get; set; }
        public SchoolOrder OrderBy { get; set; }
    }
}
