using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MMail
{
    public class MailMessage
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
            UsernameEmpty,
            UsernameOverLength,
            PasswordEmpty,
            PasswordOverLength,
            RecipientsEmpty,
            RecipientsOverLength,
            BccRecipientsEmpty,
            BccRecipientsOverLength,
            CcRecipientsEmpty,
            CcRecipientsOverLength,
            SubjectEmpty,
            SubjectOverLength,
            BodyEmpty,
            BodyOverLength,
            ErrorEmpty,
            ErrorOverLength,
        }
    }
}
