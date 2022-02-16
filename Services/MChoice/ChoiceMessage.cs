using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MChoice
{
    public class ChoiceMessage
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
            ChoiceContentEmpty,
            ChoiceContentOverLength,
            DescriptionEmpty,
            DescriptionOverLength,
            MbtiSingleTypeEmpty,
            MbtiSingleTypeNotExisted,
            QuestionEmpty,
            QuestionNotExisted,
        }
    }
}
