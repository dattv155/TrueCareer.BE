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
using TrueCareer.Services.MActiveTime;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.active_time
{
    public partial class ActiveTimeController 
    {
        [Route(ActiveTimeRoute.SingleListAppUser), HttpPost]
        public async Task<List<ActiveTime_AppUserDTO>> SingleListAppUser([FromBody] ActiveTime_AppUserFilterDTO ActiveTime_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ActiveTime_AppUserFilterDTO.Id;
            AppUserFilter.Username = ActiveTime_AppUserFilterDTO.Username;
            AppUserFilter.Email = ActiveTime_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ActiveTime_AppUserFilterDTO.Phone;
            AppUserFilter.Password = ActiveTime_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = ActiveTime_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = ActiveTime_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = ActiveTime_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = ActiveTime_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = ActiveTime_AppUserFilterDTO.CoverImage;
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ActiveTime_AppUserDTO> ActiveTime_AppUserDTOs = AppUsers
                .Select(x => new ActiveTime_AppUserDTO(x)).ToList();
            return ActiveTime_AppUserDTOs;
        }
    }
}

