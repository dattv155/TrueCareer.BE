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
using TrueCareer.Services.MFavouriteMentor;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.favourite_mentor
{
    public partial class FavouriteMentorController 
    {
        [Route(FavouriteMentorRoute.FilterListAppUser), HttpPost]
        public async Task<List<FavouriteMentor_AppUserDTO>> FilterListAppUser([FromBody] FavouriteMentor_AppUserFilterDTO FavouriteMentor_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = FavouriteMentor_AppUserFilterDTO.Id;
            AppUserFilter.Username = FavouriteMentor_AppUserFilterDTO.Username;
            AppUserFilter.Email = FavouriteMentor_AppUserFilterDTO.Email;
            AppUserFilter.Phone = FavouriteMentor_AppUserFilterDTO.Phone;
            AppUserFilter.Password = FavouriteMentor_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = FavouriteMentor_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = FavouriteMentor_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = FavouriteMentor_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = FavouriteMentor_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = FavouriteMentor_AppUserFilterDTO.CoverImage;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<FavouriteMentor_AppUserDTO> FavouriteMentor_AppUserDTOs = AppUsers
                .Select(x => new FavouriteMentor_AppUserDTO(x)).ToList();
            return FavouriteMentor_AppUserDTOs;
        }
    }
}

