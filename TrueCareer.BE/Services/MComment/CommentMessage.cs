using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MComment
{
    public class CommentMessage
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
            CreatorEmpty,
            CreatorNotExisted,
        }
    }
}
