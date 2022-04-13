using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MUnitOfTime
{
    public class UnitOfTimeMessage
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
            CodeEmpty,
            CodeOverLength,
            NameEmpty,
            NameOverLength,
        }
    }
}
