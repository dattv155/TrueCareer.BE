using TrueSight.Common;
using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using TrueCareer.Repositories;
using TrueCareer.Entities;
using TrueCareer.Enums;
using TrueCareer.Services.MInformation;
using TrueCareer.Services.MFavouriteMentor;
using TrueCareer.Services.MMentorMenteeConnection;

namespace TrueCareer.Services.MMentor
{
    public interface IMentorService : IServiceScoped
    {
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<int> Count(AppUserFilter AppUserFilter);

    }

    public class MentorService : IMentorService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IInformationService InformationService;
        private IMentorMenteeConnectionService MentorMenteeConnectionService;
        private IFavouriteMentorService FavouriteMentorService;
        public MentorService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            ILogging Logging,
            IInformationService InformationService,
            IMentorMenteeConnectionService MentorMenteeConnectionService,
            IFavouriteMentorService FavouriteMentorService
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
            this.InformationService = InformationService;
            this.MentorMenteeConnectionService = MentorMenteeConnectionService;
            this.FavouriteMentorService = FavouriteMentorService;
        }
        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                IdFilter RoleIdFilter = new IdFilter();
                RoleIdFilter.Equal = 2;
                AppUserFilter.RoleId = RoleIdFilter;
                List<AppUser> Mentors = await UOW.AppUserRepository.List(AppUserFilter);
                foreach (var Mentor in Mentors)
                {
                    IdFilter MentorIdFilter = new IdFilter();
                    MentorIdFilter.Equal = Mentor.Id;
                    IdFilter InformationTypeIdFilter = new IdFilter();
                    InformationTypeIdFilter.Equal = InformationTypeEnum.EXPERIENCE.Id;
                    MentorMenteeConnectionFilter MentorMenteeConnectionFilter = new MentorMenteeConnectionFilter()
                    {
                        MentorId = MentorIdFilter
                    };
                    FavouriteMentorFilter FavouriteMentorFilter = new FavouriteMentorFilter()
                    {
                        MentorId = MentorIdFilter
                    };
                    InformationFilter InformationFilter = new InformationFilter()
                    {
                        UserId = MentorIdFilter,
                        InformationTypeId = InformationTypeIdFilter,
                        Skip = 0,
                        Take = 1,
                        Selects = InformationSelect.ALL
                    };
                    List<Information> Informations = await InformationService.List(InformationFilter);

                    Mentor.MenteeCount = await MentorMenteeConnectionService.Count(MentorMenteeConnectionFilter);
                    Mentor.LikeCount = await FavouriteMentorService.Count(FavouriteMentorFilter);
                    if (Informations != null && Informations.Count() > 0)
                    {
                        Mentor.JobRole = Informations[0].Role;
                        Mentor.CompanyName = Informations[0].Name;
                    }
                }
                return Mentors;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUser));
            }
            return null;
        }

        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                IdFilter RoleIdFilter = new IdFilter();
                RoleIdFilter.Equal = 2;
                AppUserFilter.RoleId = RoleIdFilter;
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUser));
            }
            return 0;
        }
    }

}