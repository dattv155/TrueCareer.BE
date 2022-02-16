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
using TrueCareer.Services.MInformation;
using TrueCareer.Services.MInformationType;
using TrueCareer.Services.MTopic;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.information
{
    public partial class InformationController 
    {
        [Route(InformationRoute.FilterListInformationType), HttpPost]
        public async Task<List<Information_InformationTypeDTO>> FilterListInformationType([FromBody] Information_InformationTypeFilterDTO Information_InformationTypeFilterDTO)
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
            List<Information_InformationTypeDTO> Information_InformationTypeDTOs = InformationTypes
                .Select(x => new Information_InformationTypeDTO(x)).ToList();
            return Information_InformationTypeDTOs;
        }
        [Route(InformationRoute.FilterListTopic), HttpPost]
        public async Task<List<Information_TopicDTO>> FilterListTopic([FromBody] Information_TopicFilterDTO Information_TopicFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TopicFilter TopicFilter = new TopicFilter();
            TopicFilter.Skip = 0;
            TopicFilter.Take = 20;
            TopicFilter.OrderBy = TopicOrder.Id;
            TopicFilter.OrderType = OrderType.ASC;
            TopicFilter.Selects = TopicSelect.ALL;
            TopicFilter.Id = Information_TopicFilterDTO.Id;
            TopicFilter.Title = Information_TopicFilterDTO.Title;
            TopicFilter.Description = Information_TopicFilterDTO.Description;
            TopicFilter.Cost = Information_TopicFilterDTO.Cost;

            List<Topic> Topics = await TopicService.List(TopicFilter);
            List<Information_TopicDTO> Information_TopicDTOs = Topics
                .Select(x => new Information_TopicDTO(x)).ToList();
            return Information_TopicDTOs;
        }
        [Route(InformationRoute.FilterListAppUser), HttpPost]
        public async Task<List<Information_AppUserDTO>> FilterListAppUser([FromBody] Information_AppUserFilterDTO Information_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Information_AppUserFilterDTO.Id;
            AppUserFilter.Username = Information_AppUserFilterDTO.Username;
            AppUserFilter.Email = Information_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Information_AppUserFilterDTO.Phone;
            AppUserFilter.Password = Information_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = Information_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = Information_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = Information_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = Information_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = Information_AppUserFilterDTO.CoverImage;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Information_AppUserDTO> Information_AppUserDTOs = AppUsers
                .Select(x => new Information_AppUserDTO(x)).ToList();
            return Information_AppUserDTOs;
        }
    }
}

