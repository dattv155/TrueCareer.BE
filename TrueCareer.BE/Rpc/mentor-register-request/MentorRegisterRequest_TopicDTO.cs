using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequest_TopicDTO:DataDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }

        public MentorRegisterRequest_TopicDTO() { }
        public MentorRegisterRequest_TopicDTO(Topic Topic)
        {
            this.Id = Topic.Id;
            this.Title = Topic.Title;
            this.Description = Topic.Description;
            this.Cost = Topic.Cost;
            this.Informations = Topic.Informations;
            this.Warnings = Topic.Warnings;
            this.Errors = Topic.Errors;
        }
    }

    public class MentorRegisterRequest_TopicFilterDTO:FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Title { get; set; }
        public StringFilter Description { get; set; }
        public DecimalFilter Cost { get; set; }
        public TopicOrder OrderBy { get; set; }

    }
}
