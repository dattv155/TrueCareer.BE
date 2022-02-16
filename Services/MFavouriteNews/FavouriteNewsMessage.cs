using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MFavouriteNews
{
    public class FavouriteNewsMessage
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
            NewsEmpty,
            NewsNotExisted,
            UserEmpty,
            UserNotExisted,
        }
    }
}
