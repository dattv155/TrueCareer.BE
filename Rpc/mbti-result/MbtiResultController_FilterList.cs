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
using TrueCareer.Services.MMbtiResult;
using TrueCareer.Services.MMbtiPersonalType;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.mbti_result
{
    public partial class MbtiResultController 
    {
        [Route(MbtiResultRoute.FilterListMbtiPersonalType), HttpPost]
        public async Task<List<MbtiResult_MbtiPersonalTypeDTO>> FilterListMbtiPersonalType([FromBody] MbtiResult_MbtiPersonalTypeFilterDTO MbtiResult_MbtiPersonalTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MbtiPersonalTypeFilter MbtiPersonalTypeFilter = new MbtiPersonalTypeFilter();
            MbtiPersonalTypeFilter.Skip = 0;
            MbtiPersonalTypeFilter.Take = int.MaxValue;
            MbtiPersonalTypeFilter.Take = 20;
            MbtiPersonalTypeFilter.OrderBy = MbtiPersonalTypeOrder.Id;
            MbtiPersonalTypeFilter.OrderType = OrderType.ASC;
            MbtiPersonalTypeFilter.Selects = MbtiPersonalTypeSelect.ALL;

            List<MbtiPersonalType> MbtiPersonalTypes = await MbtiPersonalTypeService.List(MbtiPersonalTypeFilter);
            List<MbtiResult_MbtiPersonalTypeDTO> MbtiResult_MbtiPersonalTypeDTOs = MbtiPersonalTypes
                .Select(x => new MbtiResult_MbtiPersonalTypeDTO(x)).ToList();
            return MbtiResult_MbtiPersonalTypeDTOs;
        }
        [Route(MbtiResultRoute.FilterListAppUser), HttpPost]
        public async Task<List<MbtiResult_AppUserDTO>> FilterListAppUser([FromBody] MbtiResult_AppUserFilterDTO MbtiResult_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = MbtiResult_AppUserFilterDTO.Id;
            AppUserFilter.Username = MbtiResult_AppUserFilterDTO.Username;
            AppUserFilter.Email = MbtiResult_AppUserFilterDTO.Email;
            AppUserFilter.Phone = MbtiResult_AppUserFilterDTO.Phone;
            AppUserFilter.Password = MbtiResult_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = MbtiResult_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = MbtiResult_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = MbtiResult_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = MbtiResult_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = MbtiResult_AppUserFilterDTO.CoverImage;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MbtiResult_AppUserDTO> MbtiResult_AppUserDTOs = AppUsers
                .Select(x => new MbtiResult_AppUserDTO(x)).ToList();
            return MbtiResult_AppUserDTOs;
        }
    }
}

