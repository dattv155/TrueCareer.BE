using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MInformation
{
    public class InformationMessage
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
            NameEmpty,
            NameOverLength,
            DescriptionEmpty,
            DescriptionOverLength,
            StartAtEmpty,
            RoleEmpty,
            RoleOverLength,
            ImageEmpty,
            ImageOverLength,
            EndAtEmpty,
            InformationTypeEmpty,
            InformationTypeNotExisted,
            TopicEmpty,
            TopicNotExisted,
            UserEmpty,
            UserNotExisted,
        }
    }
}
