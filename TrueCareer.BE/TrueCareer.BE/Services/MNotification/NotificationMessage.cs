using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MNotification
{
    public class NotificationMessage
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
            TitleWebEmpty,
            TitleWebOverLength,
            ContentWebEmpty,
            ContentWebOverLength,
            TimeEmpty,
            LinkWebsiteEmpty,
            LinkWebsiteOverLength,
            RecipientEmpty,
            RecipientNotExisted,
            SenderEmpty,
            SenderNotExisted,
        }
    }
}
