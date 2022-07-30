using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Services.MConnectionType;
using TrueCareer.Services.MMajor;
using Microsoft.AspNetCore.Mvc;
using TrueSight.Common;
using TrueCareer.Entities;
using TrueCareer.Services.MMentorRegisterRequest;
using TrueCareer.Service;
using TrueCareer.Services.MTopic;
using TrueCareer.Services.MMentorConnection;
using TrueCareer.Services.MActiveTime;
using TrueCareer.Services.MAppUser;
using TrueCareer.Enums;
using TrueCareer.BE.Entities;

namespace TrueCareer.Rpc.mentor_register_request
{
    public partial class MentorRegisterRequestController : RpcController
    {
        private IMajorService MajorService;
        private IConnectionTypeService ConnectionTypeService;
        private ICurrentContext CurrentContext;
        private IMentorRegisterRequestService MentorRegisterRequestService;
        private IFileService FileService;
        private ITopicService TopicService;
        private IMentorConnectionService MentorConnectionService;
        private IActiveTimeService ActiveTimeService;
        private IAppUserService AppUserService;
        public MentorRegisterRequestController(
            IMajorService MajorService,
            IConnectionTypeService ConnectionTypeService,
            ICurrentContext CurrentContext,
            IMentorRegisterRequestService MentorRegisterRequestService,
            IFileService FileService,
            ITopicService TopicService,
            IMentorConnectionService MentorConnectionService,
            IActiveTimeService ActiveTimeService,
            IAppUserService AppUserService
        )
        {
            this.MajorService = MajorService;
            this.ConnectionTypeService = ConnectionTypeService;
            this.CurrentContext = CurrentContext;
            this.MentorRegisterRequestService = MentorRegisterRequestService;
            this.FileService = FileService;
            this.TopicService = TopicService;
            this.MentorConnectionService = MentorConnectionService;
            this.ActiveTimeService = ActiveTimeService;
            this.AppUserService = AppUserService;
        }

        [Route(MentorRegisterRequestRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] MentorRegisterRequest_MentorRegisterRequestFilterDTO MentorRegisterRequest_MentorRegisterRequestFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorRegisterRequestFilter MentorRegisterRequestFilter = ConvertFilterDTOToFilterEntity(MentorRegisterRequest_MentorRegisterRequestFilterDTO);
            MentorRegisterRequestFilter = await MentorRegisterRequestService.ToFilter(MentorRegisterRequestFilter);
            int count = await MentorRegisterRequestService.Count(MentorRegisterRequestFilter);
            return count;
        }

        [Route(MentorRegisterRequestRoute.List), HttpPost]
        public async Task<ActionResult<List<MentorRegisterRequest_MentorRegisterRequestDTO>>> List([FromBody] MentorRegisterRequest_MentorRegisterRequestFilterDTO MentorRegisterRequest_MentorRegisterRequestFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorRegisterRequestFilter MentorRegisterRequestFilter = ConvertFilterDTOToFilterEntity(MentorRegisterRequest_MentorRegisterRequestFilterDTO);
            MentorRegisterRequestFilter = await MentorRegisterRequestService.ToFilter(MentorRegisterRequestFilter);
            List<MentorRegisterRequest> MentorRegisterRequests = await MentorRegisterRequestService.List(MentorRegisterRequestFilter);
            List<MentorRegisterRequest_MentorRegisterRequestDTO> MentorRegisterRequest_MentorRegisterRequestDTOs = MentorRegisterRequests
                .Select(c => new MentorRegisterRequest_MentorRegisterRequestDTO(c)).ToList();
            return MentorRegisterRequest_MentorRegisterRequestDTOs;
        }

        [Route(MentorRegisterRequestRoute.Get), HttpPost]
        public async Task<ActionResult<MentorRegisterRequest_MentorRegisterRequestDTO>> Get([FromBody] MentorRegisterRequest_MentorRegisterRequestDTO MentorRegisterRequest_MentorRegisterRequestDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorRegisterRequest_MentorRegisterRequestDTO.Id))
                return Forbid();

            MentorRegisterRequest MentorRegisterRequest = await MentorRegisterRequestService.Get(MentorRegisterRequest_MentorRegisterRequestDTO.Id);
            return new MentorRegisterRequest_MentorRegisterRequestDTO(MentorRegisterRequest);
        }

        [Route(MentorRegisterRequestRoute.Create), HttpPost]
        public async Task<ActionResult<MentorRegisterRequest_MentorRegisterRequestDTO>> Create([FromBody] MentorRegisterRequest_MentorRegisterRequestDTO MentorRegisterRequest_MentorRegisterRequestDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorRegisterRequest_MentorRegisterRequestDTO.Id))
                return Forbid();

            MentorRegisterRequest MentorRegisterRequest = ConvertDTOToEntity(MentorRegisterRequest_MentorRegisterRequestDTO);
            MentorRegisterRequest.MentorApprovalStatus = new MentorApprovalStatus()
            {
                Id = MentorApprovalStatusEnum.PENDING.Id,
                Code = MentorApprovalStatusEnum.PENDING.Code,
                Name = MentorApprovalStatusEnum.PENDING.Name
            };
            MentorRegisterRequest.MentorApprovalStatusId = MentorApprovalStatusEnum.PENDING.Id;
            MentorRegisterRequest = await MentorRegisterRequestService.Create(MentorRegisterRequest);
            MentorRegisterRequest_MentorRegisterRequestDTO = new MentorRegisterRequest_MentorRegisterRequestDTO(MentorRegisterRequest);
            if (MentorRegisterRequest.IsValidated)

                return MentorRegisterRequest_MentorRegisterRequestDTO;
            else
                return BadRequest(MentorRegisterRequest_MentorRegisterRequestDTO);
        }

        [Route(MentorRegisterRequestRoute.Update), HttpPost]
        public async Task<ActionResult<MentorRegisterRequest_MentorRegisterRequestDTO>> Update([FromBody] MentorRegisterRequest_MentorRegisterRequestDTO MentorRegisterRequest_MentorRegisterRequestDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorRegisterRequest_MentorRegisterRequestDTO.Id))
                return Forbid();

            MentorRegisterRequest MentorRegisterRequest = ConvertDTOToEntity(MentorRegisterRequest_MentorRegisterRequestDTO);
            MentorRegisterRequest = await MentorRegisterRequestService.Update(MentorRegisterRequest);
            MentorRegisterRequest_MentorRegisterRequestDTO = new MentorRegisterRequest_MentorRegisterRequestDTO(MentorRegisterRequest);
            if (MentorRegisterRequest.IsValidated)
                return MentorRegisterRequest_MentorRegisterRequestDTO;
            else
                return BadRequest(MentorRegisterRequest_MentorRegisterRequestDTO);
        }

        [Route(MentorRegisterRequestRoute.Delete), HttpPost]
        public async Task<ActionResult<MentorRegisterRequest_MentorRegisterRequestDTO>> Delete([FromBody] MentorRegisterRequest_MentorRegisterRequestDTO MentorRegisterRequest_MentorRegisterRequestDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorRegisterRequest_MentorRegisterRequestDTO.Id))
                return Forbid();

            MentorRegisterRequest MentorRegisterRequest = ConvertDTOToEntity(MentorRegisterRequest_MentorRegisterRequestDTO);
            MentorRegisterRequest = await MentorRegisterRequestService.Delete(MentorRegisterRequest);
            MentorRegisterRequest_MentorRegisterRequestDTO = new MentorRegisterRequest_MentorRegisterRequestDTO(MentorRegisterRequest);
            if (MentorRegisterRequest.IsValidated)
                return MentorRegisterRequest_MentorRegisterRequestDTO;
            else
                return BadRequest(MentorRegisterRequest_MentorRegisterRequestDTO);
        }

        [Route(MentorRegisterRequestRoute.Approve), HttpPost]
        public async Task<ActionResult<MentorRegisterRequest_MentorRegisterRequestDTO>> Approve([FromBody] MentorRegisterRequest_MentorRegisterRequestDTO MentorRegisterRequest_MentorRegisterRequestDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorRegisterRequest_MentorRegisterRequestDTO.Id))
                return Forbid();

            MentorRegisterRequest MentorRegisterRequest = ConvertDTOToEntity(MentorRegisterRequest_MentorRegisterRequestDTO);
            MentorRegisterRequest = await MentorRegisterRequestService.Approve(MentorRegisterRequest);
            MentorRegisterRequest_MentorRegisterRequestDTO = new MentorRegisterRequest_MentorRegisterRequestDTO(MentorRegisterRequest);
            if (MentorRegisterRequest.IsValidated)
                return MentorRegisterRequest_MentorRegisterRequestDTO;
            else
                return BadRequest(MentorRegisterRequest_MentorRegisterRequestDTO);
        }

        [Route(MentorRegisterRequestRoute.Reject), HttpPost]
        public async Task<ActionResult<MentorRegisterRequest_MentorRegisterRequestDTO>> Reject([FromBody] MentorRegisterRequest_MentorRegisterRequestDTO MentorRegisterRequest_MentorRegisterRequestDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorRegisterRequest_MentorRegisterRequestDTO.Id))
                return Forbid();

            MentorRegisterRequest MentorRegisterRequest = ConvertDTOToEntity(MentorRegisterRequest_MentorRegisterRequestDTO);
            MentorApprovalStatus MentorApprovalStatus = new MentorApprovalStatus()
            {
                Id = MentorApprovalStatusEnum.REJECT.Id,
                Code = MentorApprovalStatusEnum.REJECT.Code,
                Name = MentorApprovalStatusEnum.REJECT.Name
            };
            MentorRegisterRequest.MentorApprovalStatus = MentorApprovalStatus;
            MentorRegisterRequest.MentorApprovalStatusId = MentorApprovalStatusEnum.REJECT.Id;
            MentorRegisterRequest = await MentorRegisterRequestService.Update(MentorRegisterRequest);
            MentorRegisterRequest_MentorRegisterRequestDTO = new MentorRegisterRequest_MentorRegisterRequestDTO(MentorRegisterRequest);
            if (MentorRegisterRequest.IsValidated)
                return MentorRegisterRequest_MentorRegisterRequestDTO;
            else
                return BadRequest(MentorRegisterRequest_MentorRegisterRequestDTO);
        }


        private async Task<bool> HasPermission(long Id)
        {
            MentorRegisterRequestFilter MentorRegisterRequestFilter = new MentorRegisterRequestFilter();
            MentorRegisterRequestFilter = await MentorRegisterRequestService.ToFilter(MentorRegisterRequestFilter);
            if (Id == 0)
            {

            }
            else
            {
                MentorRegisterRequestFilter.Id = new IdFilter { Equal = Id };
                int count = await MentorRegisterRequestService.Count(MentorRegisterRequestFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private MentorRegisterRequest ConvertDTOToEntity(MentorRegisterRequest_MentorRegisterRequestDTO MentorRegisterRequest_MentorRegisterRequestDTO)
        {
            MentorRegisterRequest_MentorRegisterRequestDTO.TrimString();
            MentorRegisterRequest MentorRegisterRequest = new MentorRegisterRequest();
            MentorRegisterRequest.Id = MentorRegisterRequest_MentorRegisterRequestDTO.Id;
            MentorRegisterRequest.AppUserId = MentorRegisterRequest_MentorRegisterRequestDTO.UserId;
            MentorRegisterRequest.MentorApprovalStatusId = MentorRegisterRequest_MentorRegisterRequestDTO.MentorApprovalStatusId;
            MentorRegisterRequest.MentorInfo = MentorRegisterRequest_MentorRegisterRequestDTO.MentorInfo == null ? null : new MentorInfo
            {
                Id = MentorRegisterRequest_MentorRegisterRequestDTO.MentorInfo.Id,
                AppUserId = MentorRegisterRequest_MentorRegisterRequestDTO.UserId,
                ConnectionId = MentorRegisterRequest_MentorRegisterRequestDTO.MentorInfo.ConnectionId,
                ConnectionUrl = MentorRegisterRequest_MentorRegisterRequestDTO.MentorInfo.ConnectionUrl,
                MajorId = MentorRegisterRequest_MentorRegisterRequestDTO.MentorInfo.MajorId,
                TopicDescription = MentorRegisterRequest_MentorRegisterRequestDTO.MentorInfo.TopicDescription,
                ActiveTimes = MentorRegisterRequest_MentorRegisterRequestDTO.MentorInfo.ActiveTimes.Select(x => new ActiveTime
                {
                    MentorId = MentorRegisterRequest_MentorRegisterRequestDTO.UserId,
                    ActiveDate = x.ActiveDate,
                    UnitOfTimeId = x.UnitOfTimeId,
                }).ToList(),
            };
            MentorRegisterRequest.MentorApprovalStatus = MentorRegisterRequest_MentorRegisterRequestDTO.MentorApprovalStatus == null ? null : new MentorApprovalStatus
            {
                Id = MentorRegisterRequest_MentorRegisterRequestDTO.MentorApprovalStatus.Id,
                Code = MentorRegisterRequest_MentorRegisterRequestDTO.MentorApprovalStatus.Code,
                Name = MentorRegisterRequest_MentorRegisterRequestDTO.MentorApprovalStatus.Name,
            };
            MentorRegisterRequest.User = MentorRegisterRequest_MentorRegisterRequestDTO.User == null ? null : new AppUser
            {
                Id = MentorRegisterRequest_MentorRegisterRequestDTO.User.Id,
                Username = MentorRegisterRequest_MentorRegisterRequestDTO.User.Username,
                Email = MentorRegisterRequest_MentorRegisterRequestDTO.User.Email,
                Phone = MentorRegisterRequest_MentorRegisterRequestDTO.User.Phone,
                Password = MentorRegisterRequest_MentorRegisterRequestDTO.User.Password,
                DisplayName = MentorRegisterRequest_MentorRegisterRequestDTO.User.DisplayName,
                SexId = MentorRegisterRequest_MentorRegisterRequestDTO.User.SexId,
                Birthday = MentorRegisterRequest_MentorRegisterRequestDTO.User.Birthday,
                Avatar = MentorRegisterRequest_MentorRegisterRequestDTO.User.Avatar,
                CoverImage = MentorRegisterRequest_MentorRegisterRequestDTO.User.CoverImage,
            };

            MentorRegisterRequest.BaseLanguage = CurrentContext.Language;
            return MentorRegisterRequest;
        }

        private MentorRegisterRequestFilter ConvertFilterDTOToFilterEntity(MentorRegisterRequest_MentorRegisterRequestFilterDTO MentorRegisterRequest_MentorRegisterRequestFilterDTO)
        {
            MentorRegisterRequestFilter MentorRegisterRequestFilter = new MentorRegisterRequestFilter();
            MentorRegisterRequestFilter.Selects = MentorRegisterRequestSelect.ALL;
            MentorRegisterRequestFilter.Skip = MentorRegisterRequest_MentorRegisterRequestFilterDTO.Skip;
            MentorRegisterRequestFilter.Take = MentorRegisterRequest_MentorRegisterRequestFilterDTO.Take;
            MentorRegisterRequestFilter.OrderBy = MentorRegisterRequest_MentorRegisterRequestFilterDTO.OrderBy;
            MentorRegisterRequestFilter.OrderType = MentorRegisterRequest_MentorRegisterRequestFilterDTO.OrderType;

            MentorRegisterRequestFilter.UserId = MentorRegisterRequest_MentorRegisterRequestFilterDTO.UserId;
            MentorRegisterRequestFilter.TopicId = MentorRegisterRequest_MentorRegisterRequestFilterDTO.TopicId;
            MentorRegisterRequestFilter.MentorApprovalStatusId = MentorRegisterRequest_MentorRegisterRequestFilterDTO.MentorApprovalStatusId;
            MentorRegisterRequestFilter.Id = MentorRegisterRequest_MentorRegisterRequestFilterDTO.Id;
            return MentorRegisterRequestFilter;
        }

    }
}
