using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MConversation
{
    public class ConversationMessage
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
            LatestContentEmpty,
            LatestContentOverLength,
            HashEmpty,
            HashOverLength,
            Message_ContentEmpty,
            Message_ContentOverLength,
        }
    }
}
