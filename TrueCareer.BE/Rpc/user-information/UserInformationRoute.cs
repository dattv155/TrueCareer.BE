using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Rpc.user_information
{
    public class UserInformationRoute : Root
    {
        private const string Default = Rpc + Module + "/user-information";
        public const string CountReview = Default + "/count-review";
        public const string ListReview = Default + "/list-review";
        public const string CountNews = Default + "/count-news";
        public const string ListNews = Default + "/list-news";
        public const string ListGeneralInformation = Default + "/list-general-information";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string ListRecommendMentor = Default + "/list-recommend-mentor";
        public const string GetTopic = Default + "/get-topic";
        public const string UploadFile = Default + "/upload-file";
        public const string UploadImage = Default + "/upload-image";
    }
}
