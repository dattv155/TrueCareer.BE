using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Services.MAppUser;
using TrueCareer.Services.MInformation;
using TrueCareer.Services.MMentorReview;
using TrueCareer.Services.MNews;
using TrueCareer.Services.MTopic;
using TrueSight.Common;

namespace TrueCareer.Rpc.user_information
{
    public class UserInformationController : RpcController
    {
        private ITopicService TopicService;
        private IInformationService InformationService;
        private INewsService NewsService;
        private IMentorReviewService MentorReviewService;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;

        public UserInformationController(
            ITopicService TopicService, 
            IInformationService InformationService, 
            INewsService NewsService,
            IMentorReviewService MentorReviewService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext
        ) {
            this.TopicService = TopicService;
            this.InformationService = InformationService;
            this.NewsService = NewsService;
            this.MentorReviewService = MentorReviewService;
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
        }

    }
}
