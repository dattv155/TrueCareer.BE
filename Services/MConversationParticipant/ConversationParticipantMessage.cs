using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MConversationParticipant
{
    public class ConversationParticipantMessage
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
            ConversationEmpty,
            ConversationNotExisted,
            UserEmpty,
            UserNotExisted,
        }
    }
}
