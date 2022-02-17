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
using TrueCareer.Services.MMentorMenteeConnection;
using TrueCareer.Services.MMentorConnection;
using TrueCareer.Services.MConnectionStatus;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.mentor_mentee_connection
{
    public partial class MentorMenteeConnectionController 
    {
        [Route(MentorMenteeConnectionRoute.SingleListMentorConnection), HttpPost]
        public async Task<List<MentorMenteeConnection_MentorConnectionDTO>> SingleListMentorConnection([FromBody] MentorMenteeConnection_MentorConnectionFilterDTO MentorMenteeConnection_MentorConnectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorConnectionFilter MentorConnectionFilter = new MentorConnectionFilter();
            MentorConnectionFilter.Skip = 0;
            MentorConnectionFilter.Take = 20;
            MentorConnectionFilter.OrderBy = MentorConnectionOrder.Id;
            MentorConnectionFilter.OrderType = OrderType.ASC;
            MentorConnectionFilter.Selects = MentorConnectionSelect.ALL;
            MentorConnectionFilter.Id = MentorMenteeConnection_MentorConnectionFilterDTO.Id;
            MentorConnectionFilter.MentorId = MentorMenteeConnection_MentorConnectionFilterDTO.MentorId;
            MentorConnectionFilter.Url = MentorMenteeConnection_MentorConnectionFilterDTO.Url;
            MentorConnectionFilter.ConnectionTypeId = MentorMenteeConnection_MentorConnectionFilterDTO.ConnectionTypeId;
            List<MentorConnection> MentorConnections = await MentorConnectionService.List(MentorConnectionFilter);
            List<MentorMenteeConnection_MentorConnectionDTO> MentorMenteeConnection_MentorConnectionDTOs = MentorConnections
                .Select(x => new MentorMenteeConnection_MentorConnectionDTO(x)).ToList();
            return MentorMenteeConnection_MentorConnectionDTOs;
        }
        [Route(MentorMenteeConnectionRoute.SingleListConnectionStatus), HttpPost]
        public async Task<List<MentorMenteeConnection_ConnectionStatusDTO>> SingleListConnectionStatus([FromBody] MentorMenteeConnection_ConnectionStatusFilterDTO MentorMenteeConnection_ConnectionStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConnectionStatusFilter ConnectionStatusFilter = new ConnectionStatusFilter();
            ConnectionStatusFilter.Skip = 0;
            ConnectionStatusFilter.Take = int.MaxValue;
            ConnectionStatusFilter.Take = 20;
            ConnectionStatusFilter.OrderBy = ConnectionStatusOrder.Id;
            ConnectionStatusFilter.OrderType = OrderType.ASC;
            ConnectionStatusFilter.Selects = ConnectionStatusSelect.ALL;
            List<ConnectionStatus> ConnectionStatuses = await ConnectionStatusService.List(ConnectionStatusFilter);
            List<MentorMenteeConnection_ConnectionStatusDTO> MentorMenteeConnection_ConnectionStatusDTOs = ConnectionStatuses
                .Select(x => new MentorMenteeConnection_ConnectionStatusDTO(x)).ToList();
            return MentorMenteeConnection_ConnectionStatusDTOs;
        }
        [Route(MentorMenteeConnectionRoute.SingleListAppUser), HttpPost]
        public async Task<List<MentorMenteeConnection_AppUserDTO>> SingleListAppUser([FromBody] MentorMenteeConnection_AppUserFilterDTO MentorMenteeConnection_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = MentorMenteeConnection_AppUserFilterDTO.Id;
            AppUserFilter.Username = MentorMenteeConnection_AppUserFilterDTO.Username;
            AppUserFilter.Email = MentorMenteeConnection_AppUserFilterDTO.Email;
            AppUserFilter.Phone = MentorMenteeConnection_AppUserFilterDTO.Phone;
            AppUserFilter.Password = MentorMenteeConnection_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = MentorMenteeConnection_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = MentorMenteeConnection_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = MentorMenteeConnection_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = MentorMenteeConnection_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = MentorMenteeConnection_AppUserFilterDTO.CoverImage;
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MentorMenteeConnection_AppUserDTO> MentorMenteeConnection_AppUserDTOs = AppUsers
                .Select(x => new MentorMenteeConnection_AppUserDTO(x)).ToList();
            return MentorMenteeConnection_AppUserDTOs;
        }
    }
}

