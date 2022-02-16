using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MImage
{
    public class ImageMessage
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
            UrlEmpty,
            UrlOverLength,
            ThumbnailUrlEmpty,
            ThumbnailUrlOverLength,
        }
    }
}
