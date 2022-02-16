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
using TrueCareer.Services.MMentorConnection;
using TrueCareer.Services.MConnectionType;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.mentor_connection
{
    public partial class MentorConnectionController 
    {
        [Route(MentorConnectionRoute.FilterListConnectionType), HttpPost]
        public async Task<List<MentorConnection_ConnectionTypeDTO>> FilterListConnectionType([FromBody] MentorConnection_ConnectionTypeFilterDTO MentorConnection_ConnectionTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConnectionTypeFilter ConnectionTypeFilter = new ConnectionTypeFilter();
            ConnectionTypeFilter.Skip = 0;
            ConnectionTypeFilter.Take = int.MaxValue;
            ConnectionTypeFilter.Take = 20;
            ConnectionTypeFilter.OrderBy = ConnectionTypeOrder.Id;
            ConnectionTypeFilter.OrderType = OrderType.ASC;
            ConnectionTypeFilter.Selects = ConnectionTypeSelect.ALL;

            List<ConnectionType> ConnectionTypes = await ConnectionTypeService.List(ConnectionTypeFilter);
            List<MentorConnection_ConnectionTypeDTO> MentorConnection_ConnectionTypeDTOs = ConnectionTypes
                .Select(x => new MentorConnection_ConnectionTypeDTO(x)).ToList();
            return MentorConnection_ConnectionTypeDTOs;
        }
        [Route(MentorConnectionRoute.FilterListAppUser), HttpPost]
        public async Task<List<MentorConnection_AppUserDTO>> FilterListAppUser([FromBody] MentorConnection_AppUserFilterDTO MentorConnection_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = MentorConnection_AppUserFilterDTO.Id;
            AppUserFilter.Username = MentorConnection_AppUserFilterDTO.Username;
            AppUserFilter.Email = MentorConnection_AppUserFilterDTO.Email;
            AppUserFilter.Phone = MentorConnection_AppUserFilterDTO.Phone;
            AppUserFilter.Password = MentorConnection_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = MentorConnection_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = MentorConnection_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = MentorConnection_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = MentorConnection_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = MentorConnection_AppUserFilterDTO.CoverImage;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MentorConnection_AppUserDTO> MentorConnection_AppUserDTOs = AppUsers
                .Select(x => new MentorConnection_AppUserDTO(x)).ToList();
            return MentorConnection_AppUserDTOs;
        }
    }
}

