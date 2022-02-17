using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.choice
{
    public class Choice_QuestionDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string QuestionContent { get; set; }
        
        public string Description { get; set; }
        
        public Choice_QuestionDTO() {}
        public Choice_QuestionDTO(Question Question)
        {
            
            this.Id = Question.Id;
            
            this.QuestionContent = Question.QuestionContent;
            
            this.Description = Question.Description;
            
            this.Informations = Question.Informations;
            this.Warnings = Question.Warnings;
            this.Errors = Question.Errors;
        }
    }

    public class Choice_QuestionFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter QuestionContent { get; set; }
        
        public StringFilter Description { get; set; }
        
        public QuestionOrder OrderBy { get; set; }
    }
}