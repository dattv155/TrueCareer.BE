using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MConversationConfiguration
{
    public class ConversationConfigurationMessage
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
            AppSecretEmpty,
            AppSecretOverLength,
            AppNameEmpty,
            AppNameOverLength,
            OASecretKeyEmpty,
            OASecretKeyOverLength,
            ExpiredAtEmpty,
            ConversationTypeEmpty,
            ConversationTypeNotExisted,
        }
    }
}
