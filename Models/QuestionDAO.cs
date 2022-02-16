using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class QuestionDAO
    {
        public QuestionDAO()
        {
            Choices = new HashSet<ChoiceDAO>();
        }

        public long Id { get; set; }
        public string QuestionContent { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ChoiceDAO> Choices { get; set; }
    }
}
