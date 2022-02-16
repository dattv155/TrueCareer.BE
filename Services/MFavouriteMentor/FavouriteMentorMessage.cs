using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MFavouriteMentor
{
    public class FavouriteMentorMessage
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
            MentorEmpty,
            MentorNotExisted,
            UserEmpty,
            UserNotExisted,
        }
    }
}
