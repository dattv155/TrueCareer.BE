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
using TrueCareer.Services.MNotification;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.notification
{
    public partial class NotificationController 
    {
        [Route(NotificationRoute.SingleListAppUser), HttpPost]
        public async Task<List<Notification_AppUserDTO>> SingleListAppUser([FromBody] Notification_AppUserFilterDTO Notification_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Notification_AppUserFilterDTO.Id;
            AppUserFilter.Username = Notification_AppUserFilterDTO.Username;
            AppUserFilter.Email = Notification_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Notification_AppUserFilterDTO.Phone;
            AppUserFilter.Password = Notification_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = Notification_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = Notification_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = Notification_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = Notification_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = Notification_AppUserFilterDTO.CoverImage;
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Notification_AppUserDTO> Notification_AppUserDTOs = AppUsers
                .Select(x => new Notification_AppUserDTO(x)).ToList();
            return Notification_AppUserDTOs;
        }
    }
}

