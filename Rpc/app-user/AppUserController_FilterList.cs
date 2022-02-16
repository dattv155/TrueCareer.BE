using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using TrueCareer.Entities;
using TrueCareer.Services.MAppUser;
using TrueCareer.Services.MSex;
using TrueCareer.Services.MFavouriteMentor;
using TrueCareer.Services.MFavouriteNews;
using TrueCareer.Services.MNews;
using TrueCareer.Services.MInformation;
using TrueCareer.Services.MInformationType;
using TrueCareer.Services.MTopic;

namespace TrueCareer.Rpc.app_user
{
    public partial class AppUserController 
    {
        [Route(AppUserRoute.FilterListSex), HttpPost]
        public async Task<List<AppUser_SexDTO>> FilterListSex([FromBody] AppUser_SexFilterDTO AppUser_SexFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SexFilter SexFilter = new SexFilter();
            SexFilter.Skip = 0;
            SexFilter.Take = int.MaxValue;
            SexFilter.Take = 20;
            SexFilter.OrderBy = SexOrder.Id;
            SexFilter.OrderType = OrderType.ASC;
            SexFilter.Selects = SexSelect.ALL;

            List<Sex> Sexes = await SexService.List(SexFilter);
            List<AppUser_SexDTO> AppUser_SexDTOs = Sexes
                .Select(x => new AppUser_SexDTO(x)).ToList();
            return AppUser_SexDTOs;
        }
        [Route(AppUserRoute.FilterListFavouriteMentor), HttpPost]
        public async Task<List<AppUser_FavouriteMentorDTO>> FilterListFavouriteMentor([FromBody] AppUser_FavouriteMentorFilterDTO AppUser_FavouriteMentorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FavouriteMentorFilter FavouriteMentorFilter = new FavouriteMentorFilter();
            FavouriteMentorFilter.Skip = 0;
            FavouriteMentorFilter.Take = 20;
            FavouriteMentorFilter.OrderBy = FavouriteMentorOrder.Id;
            FavouriteMentorFilter.OrderType = OrderType.ASC;
            FavouriteMentorFilter.Selects = FavouriteMentorSelect.ALL;
            FavouriteMentorFilter.Id = AppUser_FavouriteMentorFilterDTO.Id;
            FavouriteMentorFilter.UserId = AppUser_FavouriteMentorFilterDTO.UserId;
            FavouriteMentorFilter.MentorId = AppUser_FavouriteMentorFilterDTO.MentorId;

            List<FavouriteMentor> FavouriteMentors = await FavouriteMentorService.List(FavouriteMentorFilter);
            List<AppUser_FavouriteMentorDTO> AppUser_FavouriteMentorDTOs = FavouriteMentors
                .Select(x => new AppUser_FavouriteMentorDTO(x)).ToList();
            return AppUser_FavouriteMentorDTOs;
        }
        [Route(AppUserRoute.FilterListFavouriteNews), HttpPost]
        public async Task<List<AppUser_FavouriteNewsDTO>> FilterListFavouriteNews([FromBody] AppUser_FavouriteNewsFilterDTO AppUser_FavouriteNewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FavouriteNewsFilter FavouriteNewsFilter = new FavouriteNewsFilter();
            FavouriteNewsFilter.Skip = 0;
            FavouriteNewsFilter.Take = 20;
            FavouriteNewsFilter.OrderBy = FavouriteNewsOrder.Id;
            FavouriteNewsFilter.OrderType = OrderType.ASC;
            FavouriteNewsFilter.Selects = FavouriteNewsSelect.ALL;
            FavouriteNewsFilter.Id = AppUser_FavouriteNewsFilterDTO.Id;
            FavouriteNewsFilter.UserId = AppUser_FavouriteNewsFilterDTO.UserId;
            FavouriteNewsFilter.NewsId = AppUser_FavouriteNewsFilterDTO.NewsId;

            List<FavouriteNews> FavouriteNews = await FavouriteNewsService.List(FavouriteNewsFilter);
            List<AppUser_FavouriteNewsDTO> AppUser_FavouriteNewsDTOs = FavouriteNews
                .Select(x => new AppUser_FavouriteNewsDTO(x)).ToList();
            return AppUser_FavouriteNewsDTOs;
        }
        [Route(AppUserRoute.FilterListNews), HttpPost]
        public async Task<List<AppUser_NewsDTO>> FilterListNews([FromBody] AppUser_NewsFilterDTO AppUser_NewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NewsFilter NewsFilter = new NewsFilter();
            NewsFilter.Skip = 0;
            NewsFilter.Take = 20;
            NewsFilter.OrderBy = NewsOrder.Id;
            NewsFilter.OrderType = OrderType.ASC;
            NewsFilter.Selects = NewsSelect.ALL;
            NewsFilter.Id = AppUser_NewsFilterDTO.Id;
            NewsFilter.CreatorId = AppUser_NewsFilterDTO.CreatorId;
            NewsFilter.NewsContent = AppUser_NewsFilterDTO.NewsContent;
            NewsFilter.LikeCounting = AppUser_NewsFilterDTO.LikeCounting;
            NewsFilter.WatchCounting = AppUser_NewsFilterDTO.WatchCounting;
            NewsFilter.NewsStatusId = AppUser_NewsFilterDTO.NewsStatusId;

            List<News> News = await NewsService.List(NewsFilter);
            List<AppUser_NewsDTO> AppUser_NewsDTOs = News
                .Select(x => new AppUser_NewsDTO(x)).ToList();
            return AppUser_NewsDTOs;
        }
        [Route(AppUserRoute.FilterListInformation), HttpPost]
        public async Task<List<AppUser_InformationDTO>> FilterListInformation([FromBody] AppUser_InformationFilterDTO AppUser_InformationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            InformationFilter InformationFilter = new InformationFilter();
            InformationFilter.Skip = 0;
            InformationFilter.Take = 20;
            InformationFilter.OrderBy = InformationOrder.Id;
            InformationFilter.OrderType = OrderType.ASC;
            InformationFilter.Selects = InformationSelect.ALL;
            InformationFilter.Id = AppUser_InformationFilterDTO.Id;
            InformationFilter.InformationTypeId = AppUser_InformationFilterDTO.InformationTypeId;
            InformationFilter.Name = AppUser_InformationFilterDTO.Name;
            InformationFilter.Description = AppUser_InformationFilterDTO.Description;
            InformationFilter.StartAt = AppUser_InformationFilterDTO.StartAt;
            InformationFilter.Role = AppUser_InformationFilterDTO.Role;
            InformationFilter.Image = AppUser_InformationFilterDTO.Image;
            InformationFilter.TopicId = AppUser_InformationFilterDTO.TopicId;
            InformationFilter.UserId = AppUser_InformationFilterDTO.UserId;
            InformationFilter.EndAt = AppUser_InformationFilterDTO.EndAt;

            List<Information> Information = await InformationService.List(InformationFilter);
            List<AppUser_InformationDTO> AppUser_InformationDTOs = Information
                .Select(x => new AppUser_InformationDTO(x)).ToList();
            return AppUser_InformationDTOs;
        }
        [Route(AppUserRoute.FilterListInformationType), HttpPost]
        public async Task<List<AppUser_InformationTypeDTO>> FilterListInformationType([FromBody] AppUser_InformationTypeFilterDTO AppUser_InformationTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            InformationTypeFilter InformationTypeFilter = new InformationTypeFilter();
            InformationTypeFilter.Skip = 0;
            InformationTypeFilter.Take = int.MaxValue;
            InformationTypeFilter.Take = 20;
            InformationTypeFilter.OrderBy = InformationTypeOrder.Id;
            InformationTypeFilter.OrderType = OrderType.ASC;
            InformationTypeFilter.Selects = InformationTypeSelect.ALL;

            List<InformationType> InformationTypes = await InformationTypeService.List(InformationTypeFilter);
            List<AppUser_InformationTypeDTO> AppUser_InformationTypeDTOs = InformationTypes
                .Select(x => new AppUser_InformationTypeDTO(x)).ToList();
            return AppUser_InformationTypeDTOs;
        }
        [Route(AppUserRoute.FilterListTopic), HttpPost]
        public async Task<List<AppUser_TopicDTO>> FilterListTopic([FromBody] AppUser_TopicFilterDTO AppUser_TopicFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TopicFilter TopicFilter = new TopicFilter();
            TopicFilter.Skip = 0;
            TopicFilter.Take = 20;
            TopicFilter.OrderBy = TopicOrder.Id;
            TopicFilter.OrderType = OrderType.ASC;
            TopicFilter.Selects = TopicSelect.ALL;
            TopicFilter.Id = AppUser_TopicFilterDTO.Id;
            TopicFilter.Title = AppUser_TopicFilterDTO.Title;
            TopicFilter.Description = AppUser_TopicFilterDTO.Description;
            TopicFilter.Cost = AppUser_TopicFilterDTO.Cost;

            List<Topic> Topics = await TopicService.List(TopicFilter);
            List<AppUser_TopicDTO> AppUser_TopicDTOs = Topics
                .Select(x => new AppUser_TopicDTO(x)).ToList();
            return AppUser_TopicDTOs;
        }
    }
}

