using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.question
{
    public class Question_QuestionDTO : DataDTO
    {
        public long Id { get; set; }
        public string QuestionContent { get; set; }
        public string Description { get; set; }
        public List<Question_ChoiceDTO> Choices { get; set; }
        public Question_QuestionDTO() {}
        public Question_QuestionDTO(Question Question)
        {
            this.Id = Question.Id;
            this.QuestionContent = Question.QuestionContent;
            this.Description = Question.Description;
            this.Choices = Question.Choices?.Select(x => new Question_ChoiceDTO(x)).ToList();
            this.Informations = Question.Informations;
            this.Warnings = Question.Warnings;
            this.Errors = Question.Errors;
        }
    }

    public class Question_QuestionFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter QuestionContent { get; set; }
        public StringFilter Description { get; set; }
        public QuestionOrder OrderBy { get; set; }
    }
}
