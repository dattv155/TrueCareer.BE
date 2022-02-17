using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MMbtiResult
{
    public class MbtiResultMessage
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
            MbtiPersonalTypeEmpty,
            MbtiPersonalTypeNotExisted,
            UserEmpty,
            UserNotExisted,
        }
    }
}
