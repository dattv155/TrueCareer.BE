using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MMessage
{
    public class MessageMessage
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
            ContentEmpty,
            ContentOverLength,
            ConversationEmpty,
            ConversationNotExisted,
        }
    }
}
