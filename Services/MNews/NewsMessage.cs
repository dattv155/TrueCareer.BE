using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MNews
{
    public class NewsMessage
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
            NewsContentEmpty,
            NewsContentOverLength,
            CreatorEmpty,
            CreatorNotExisted,
            NewsStatusEmpty,
            NewsStatusNotExisted,
        }
    }
}
