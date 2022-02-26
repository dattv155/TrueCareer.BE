using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class MbtiSingleTypeDAO
    {
        public MbtiSingleTypeDAO()
        {
            Choices = new HashSet<ChoiceDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ChoiceDAO> Choices { get; set; }
    }
}
