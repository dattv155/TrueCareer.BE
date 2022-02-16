using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MTopic
{
    public class TopicMessage
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
            TitleEmpty,
            TitleOverLength,
            DescriptionEmpty,
            DescriptionOverLength,
        }
    }
}
