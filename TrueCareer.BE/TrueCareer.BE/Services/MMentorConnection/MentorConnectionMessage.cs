using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MMentorConnection
{
    public class MentorConnectionMessage
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
            UrlEmpty,
            UrlOverLength,
            ConnectionTypeEmpty,
            ConnectionTypeNotExisted,
            MentorEmpty,
            MentorNotExisted,
        }
    }
}
