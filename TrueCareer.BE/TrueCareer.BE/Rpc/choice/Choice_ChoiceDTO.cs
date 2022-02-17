using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.choice
{
    public class Choice_ChoiceDTO : DataDTO
    {
        public long Id { get; set; }
        public string ChoiceContent { get; set; }
        public string Description { get; set; }
        public long QuestionId { get; set; }
        public long MbtiSingleTypeId { get; set; }
        public Choice_MbtiSingleTypeDTO MbtiSingleType { get; set; }
        public Choice_QuestionDTO Question { get; set; }
        public Choice_ChoiceDTO() {}
        public Choice_ChoiceDTO(Choice Choice)
        {
            this.Id = Choice.Id;
            this.ChoiceContent = Choice.ChoiceContent;
            this.Description = Choice.Description;
            this.QuestionId = Choice.QuestionId;
            this.MbtiSingleTypeId = Choice.MbtiSingleTypeId;
            this.MbtiSingleType = Choice.MbtiSingleType == null ? null : new Choice_MbtiSingleTypeDTO(Choice.MbtiSingleType);
            this.Question = Choice.Question == null ? null : new Choice_QuestionDTO(Choice.Question);
            this.Informations = Choice.Informations;
            this.Warnings = Choice.Warnings;
            this.Errors = Choice.Errors;
        }
    }

    public class Choice_ChoiceFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter ChoiceContent { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter QuestionId { get; set; }
        public IdFilter MbtiSingleTypeId { get; set; }
        public ChoiceOrder OrderBy { get; set; }
    }
}
