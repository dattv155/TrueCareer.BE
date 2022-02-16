using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MActiveTime
{
    public class ActiveTimeMessage
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
            StartAtEmpty,
            EndAtEmpty,
            MentorEmpty,
            MentorNotExisted,
        }
    }
}
