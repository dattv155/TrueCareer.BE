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
using System.Dynamic;
using TrueCareer.Entities;
using TrueCareer.Services.MInformation;
using TrueCareer.Services.MInformationType;
using TrueCareer.Services.MTopic;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.information
{
    public partial class InformationController : RpcController
    {
        private IInformationTypeService InformationTypeService;
        private ITopicService TopicService;
        private IAppUserService AppUserService;
        private IInformationService InformationService;
        private ICurrentContext CurrentContext;
        public InformationController(
            IInformationTypeService InformationTypeService,
            ITopicService TopicService,
            IAppUserService AppUserService,
            IInformationService InformationService,
            ICurrentContext CurrentContext
        )
        {
            this.InformationTypeService = InformationTypeService;
            this.TopicService = TopicService;
            this.AppUserService = AppUserService;
            this.InformationService = InformationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(InformationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Information_InformationFilterDTO Information_InformationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            InformationFilter InformationFilter = ConvertFilterDTOToFilterEntity(Information_InformationFilterDTO);
            InformationFilter = await InformationService.ToFilter(InformationFilter);
            int count = await InformationService.Count(InformationFilter);
            return count;
        }

        [Route(InformationRoute.List), HttpPost]
        public async Task<ActionResult<List<Information_InformationDTO>>> List([FromBody] Information_InformationFilterDTO Information_InformationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            InformationFilter InformationFilter = ConvertFilterDTOToFilterEntity(Information_InformationFilterDTO);
            InformationFilter = await InformationService.ToFilter(InformationFilter);
            List<Information> Information = await InformationService.List(InformationFilter);
            List<Information_InformationDTO> Information_InformationDTOs = Information
                .Select(c => new Information_InformationDTO(c)).ToList();
            return Information_InformationDTOs;
        }

        [Route(InformationRoute.Get), HttpPost]
        public async Task<ActionResult<Information_InformationDTO>> Get([FromBody]Information_InformationDTO Information_InformationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Information_InformationDTO.Id))
                return Forbid();

            Information Information = await InformationService.Get(Information_InformationDTO.Id);
            return new Information_InformationDTO(Information);
        }

        [Route(InformationRoute.Create), HttpPost]
        public async Task<ActionResult<Information_InformationDTO>> Create([FromBody] Information_InformationDTO Information_InformationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Information_InformationDTO.Id))
                return Forbid();

            Information Information = ConvertDTOToEntity(Information_InformationDTO);
            Information = await InformationService.Create(Information);
            Information_InformationDTO = new Information_InformationDTO(Information);
            if (Information.IsValidated)
                return Information_InformationDTO;
            else
                return BadRequest(Information_InformationDTO);
        }

        [Route(InformationRoute.Update), HttpPost]
        public async Task<ActionResult<Information_InformationDTO>> Update([FromBody] Information_InformationDTO Information_InformationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Information_InformationDTO.Id))
                return Forbid();

            Information Information = ConvertDTOToEntity(Information_InformationDTO);
            Information = await InformationService.Update(Information);
            Information_InformationDTO = new Information_InformationDTO(Information);
            if (Information.IsValidated)
                return Information_InformationDTO;
            else
                return BadRequest(Information_InformationDTO);
        }

        [Route(InformationRoute.Delete), HttpPost]
        public async Task<ActionResult<Information_InformationDTO>> Delete([FromBody] Information_InformationDTO Information_InformationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Information_InformationDTO.Id))
                return Forbid();

            Information Information = ConvertDTOToEntity(Information_InformationDTO);
            Information = await InformationService.Delete(Information);
            Information_InformationDTO = new Information_InformationDTO(Information);
            if (Information.IsValidated)
                return Information_InformationDTO;
            else
                return BadRequest(Information_InformationDTO);
        }
        
        [Route(InformationRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            InformationFilter InformationFilter = new InformationFilter();
            InformationFilter = await InformationService.ToFilter(InformationFilter);
            InformationFilter.Id = new IdFilter { In = Ids };
            InformationFilter.Selects = InformationSelect.Id;
            InformationFilter.Skip = 0;
            InformationFilter.Take = int.MaxValue;

            List<Information> Information = await InformationService.List(InformationFilter);
            Information = await InformationService.BulkDelete(Information);
            if (Information.Any(x => !x.IsValidated))
                return BadRequest(Information.Where(x => !x.IsValidated));
            return true;
        }
        
      

        [Route(InformationRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Information_InformationFilterDTO Information_InformationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Information_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Information.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            InformationFilter InformationFilter = new InformationFilter();
            InformationFilter = await InformationService.ToFilter(InformationFilter);
            if (Id == 0)
            {

            }
            else
            {
                InformationFilter.Id = new IdFilter { Equal = Id };
                int count = await InformationService.Count(InformationFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Information ConvertDTOToEntity(Information_InformationDTO Information_InformationDTO)
        {
            Information_InformationDTO.TrimString();
            Information Information = new Information();
            Information.Id = Information_InformationDTO.Id;
            Information.InformationTypeId = Information_InformationDTO.InformationTypeId;
            Information.Name = Information_InformationDTO.Name;
            Information.Description = Information_InformationDTO.Description;
            Information.StartAt = Information_InformationDTO.StartAt;
            Information.Role = Information_InformationDTO.Role;
            Information.Image = Information_InformationDTO.Image;
            Information.TopicId = Information_InformationDTO.TopicId;
            Information.UserId = Information_InformationDTO.UserId;
            Information.EndAt = Information_InformationDTO.EndAt;
            Information.InformationType = Information_InformationDTO.InformationType == null ? null : new InformationType
            {
                Id = Information_InformationDTO.InformationType.Id,
                Name = Information_InformationDTO.InformationType.Name,
                Code = Information_InformationDTO.InformationType.Code,
            };
            Information.Topic = Information_InformationDTO.Topic == null ? null : new Topic
            {
                Id = Information_InformationDTO.Topic.Id,
                Title = Information_InformationDTO.Topic.Title,
                Description = Information_InformationDTO.Topic.Description,
                Cost = Information_InformationDTO.Topic.Cost,
            };
            Information.User = Information_InformationDTO.User == null ? null : new AppUser
            {
                Id = Information_InformationDTO.User.Id,
                Username = Information_InformationDTO.User.Username,
                Email = Information_InformationDTO.User.Email,
                Phone = Information_InformationDTO.User.Phone,
                Password = Information_InformationDTO.User.Password,
                DisplayName = Information_InformationDTO.User.DisplayName,
                SexId = Information_InformationDTO.User.SexId,
                Birthday = Information_InformationDTO.User.Birthday,
                Avatar = Information_InformationDTO.User.Avatar,
                CoverImage = Information_InformationDTO.User.CoverImage,
            };
            Information.BaseLanguage = CurrentContext.Language;
            return Information;
        }

        private InformationFilter ConvertFilterDTOToFilterEntity(Information_InformationFilterDTO Information_InformationFilterDTO)
        {
            InformationFilter InformationFilter = new InformationFilter();
            InformationFilter.Selects = InformationSelect.ALL;
            InformationFilter.Skip = Information_InformationFilterDTO.Skip;
            InformationFilter.Take = Information_InformationFilterDTO.Take;
            InformationFilter.OrderBy = Information_InformationFilterDTO.OrderBy;
            InformationFilter.OrderType = Information_InformationFilterDTO.OrderType;

            InformationFilter.Id = Information_InformationFilterDTO.Id;
            InformationFilter.InformationTypeId = Information_InformationFilterDTO.InformationTypeId;
            InformationFilter.Name = Information_InformationFilterDTO.Name;
            InformationFilter.Description = Information_InformationFilterDTO.Description;
            InformationFilter.StartAt = Information_InformationFilterDTO.StartAt;
            InformationFilter.Role = Information_InformationFilterDTO.Role;
            InformationFilter.Image = Information_InformationFilterDTO.Image;
            InformationFilter.TopicId = Information_InformationFilterDTO.TopicId;
            InformationFilter.UserId = Information_InformationFilterDTO.UserId;
            InformationFilter.EndAt = Information_InformationFilterDTO.EndAt;
            InformationFilter.CreatedAt = Information_InformationFilterDTO.CreatedAt;
            InformationFilter.UpdatedAt = Information_InformationFilterDTO.UpdatedAt;
            return InformationFilter;
        }
    }
}

