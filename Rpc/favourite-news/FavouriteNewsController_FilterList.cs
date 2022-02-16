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
using TrueCareer.Services.MFavouriteNews;
using TrueCareer.Services.MNews;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.favourite_news
{
    public partial class FavouriteNewsController 
    {
        [Route(FavouriteNewsRoute.FilterListNews), HttpPost]
        public async Task<List<FavouriteNews_NewsDTO>> FilterListNews([FromBody] FavouriteNews_NewsFilterDTO FavouriteNews_NewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NewsFilter NewsFilter = new NewsFilter();
            NewsFilter.Skip = 0;
            NewsFilter.Take = 20;
            NewsFilter.OrderBy = NewsOrder.Id;
            NewsFilter.OrderType = OrderType.ASC;
            NewsFilter.Selects = NewsSelect.ALL;
            NewsFilter.Id = FavouriteNews_NewsFilterDTO.Id;
            NewsFilter.CreatorId = FavouriteNews_NewsFilterDTO.CreatorId;
            NewsFilter.NewsContent = FavouriteNews_NewsFilterDTO.NewsContent;
            NewsFilter.LikeCounting = FavouriteNews_NewsFilterDTO.LikeCounting;
            NewsFilter.WatchCounting = FavouriteNews_NewsFilterDTO.WatchCounting;
            NewsFilter.NewsStatusId = FavouriteNews_NewsFilterDTO.NewsStatusId;

            List<News> News = await NewsService.List(NewsFilter);
            List<FavouriteNews_NewsDTO> FavouriteNews_NewsDTOs = News
                .Select(x => new FavouriteNews_NewsDTO(x)).ToList();
            return FavouriteNews_NewsDTOs;
        }
        [Route(FavouriteNewsRoute.FilterListAppUser), HttpPost]
        public async Task<List<FavouriteNews_AppUserDTO>> FilterListAppUser([FromBody] FavouriteNews_AppUserFilterDTO FavouriteNews_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = FavouriteNews_AppUserFilterDTO.Id;
            AppUserFilter.Username = FavouriteNews_AppUserFilterDTO.Username;
            AppUserFilter.Email = FavouriteNews_AppUserFilterDTO.Email;
            AppUserFilter.Phone = FavouriteNews_AppUserFilterDTO.Phone;
            AppUserFilter.Password = FavouriteNews_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = FavouriteNews_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = FavouriteNews_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = FavouriteNews_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = FavouriteNews_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = FavouriteNews_AppUserFilterDTO.CoverImage;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<FavouriteNews_AppUserDTO> FavouriteNews_AppUserDTOs = AppUsers
                .Select(x => new FavouriteNews_AppUserDTO(x)).ToList();
            return FavouriteNews_AppUserDTOs;
        }
    }
}

