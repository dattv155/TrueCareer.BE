using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MMentorMenteeConnection
{
    public class MentorMenteeConnectionMessage
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
            FirstMessageEmpty,
            FirstMessageOverLength,
            ConnectionEmpty,
            ConnectionNotExisted,
            ConnectionStatusEmpty,
            ConnectionStatusNotExisted,
            MenteeEmpty,
            MenteeNotExisted,
            MentorEmpty,
            MentorNotExisted,
        }
    }
}
