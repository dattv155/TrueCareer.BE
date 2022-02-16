using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MQuestion
{
    public class QuestionMessage
    {
        public enum Information
        {

        }

        public enum Warning
        {

        }

        public enum Error
        {
            IdNotExisted,
            QuestionContentEmpty,
            QuestionContentOverLength,
            DescriptionEmpty,
            DescriptionOverLength,
            Choice_ChoiceContentEmpty,
            Choice_ChoiceContentOverLength,
            Choice_DescriptionEmpty,
            Choice_DescriptionOverLength,
            Choice_MbtiSingleTypeEmpty,
            Choice_MbtiSingleTypeNotExisted,
        }
    }
}
