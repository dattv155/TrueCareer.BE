using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MMentorReview
{
    public class MentorReviewMessage
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
            DescriptionEmpty,
            DescriptionOverLength,
            ContentReviewEmpty,
            ContentReviewOverLength,
            TimeEmpty,
            CreatorEmpty,
            CreatorNotExisted,
            MentorEmpty,
            MentorNotExisted,
        }
    }
}
