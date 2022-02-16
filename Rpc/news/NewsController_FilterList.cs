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
using TrueCareer.Services.MNews;
using TrueCareer.Services.MAppUser;
using TrueCareer.Services.MNewsStatus;

namespace TrueCareer.Rpc.news
{
    public partial class NewsController 
    {
        [Route(NewsRoute.FilterListAppUser), HttpPost]
        public async Task<List<News_AppUserDTO>> FilterListAppUser([FromBody] News_AppUserFilterDTO News_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = News_AppUserFilterDTO.Id;
            AppUserFilter.Username = News_AppUserFilterDTO.Username;
            AppUserFilter.Email = News_AppUserFilterDTO.Email;
            AppUserFilter.Phone = News_AppUserFilterDTO.Phone;
            AppUserFilter.Password = News_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = News_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = News_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = News_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = News_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = News_AppUserFilterDTO.CoverImage;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<News_AppUserDTO> News_AppUserDTOs = AppUsers
                .Select(x => new News_AppUserDTO(x)).ToList();
            return News_AppUserDTOs;
        }
        [Route(NewsRoute.FilterListNewsStatus), HttpPost]
        public async Task<List<News_NewsStatusDTO>> FilterListNewsStatus([FromBody] News_NewsStatusFilterDTO News_NewsStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NewsStatusFilter NewsStatusFilter = new NewsStatusFilter();
            NewsStatusFilter.Skip = 0;
            NewsStatusFilter.Take = int.MaxValue;
            NewsStatusFilter.Take = 20;
            NewsStatusFilter.OrderBy = NewsStatusOrder.Id;
            NewsStatusFilter.OrderType = OrderType.ASC;
            NewsStatusFilter.Selects = NewsStatusSelect.ALL;

            List<NewsStatus> NewsStatuses = await NewsStatusService.List(NewsStatusFilter);
            List<News_NewsStatusDTO> News_NewsStatusDTOs = NewsStatuses
                .Select(x => new News_NewsStatusDTO(x)).ToList();
            return News_NewsStatusDTOs;
        }
    }
}

