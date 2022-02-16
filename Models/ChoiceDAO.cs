using System;
using System.Collections.Generic;

namespace TrueCareer.Models
{
    public partial class ChoiceDAO
    {
        public long Id { get; set; }
        public string ChoiceContent { get; set; }
        public string Description { get; set; }
        public long QuestionId { get; set; }
        public long MbtiSingleTypeId { get; set; }

        public virtual MbtiSingleTypeDAO MbtiSingleType { get; set; }
        public virtual QuestionDAO Question { get; set; }
    }
}
