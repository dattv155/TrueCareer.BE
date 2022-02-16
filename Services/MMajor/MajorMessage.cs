using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MMajor
{
    public class MajorMessage
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
        }
    }
}
