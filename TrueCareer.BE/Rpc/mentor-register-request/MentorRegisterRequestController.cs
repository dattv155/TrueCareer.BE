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

        [Route(MentorRegisterRequestRoute.SaveTopic), HttpPost]
        public async Task<ActionResult<MentorRegisterRequest_TopicDTO>> SaveTopic([FromBody] MentorRegisterRequest_TopicDTO MentorRegisterRequest_TopicDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorRegisterRequest_TopicDTO.Id))
                return Forbid();

            Topic Topic = new Topic()
            {
                Id = MentorRegisterRequest_TopicDTO.Id,
                Title = MentorRegisterRequest_TopicDTO.Title,
                Description = MentorRegisterRequest_TopicDTO.Description,
                Cost = MentorRegisterRequest_TopicDTO.Cost
            };
            Topic = await TopicService.Create(Topic);
            MentorRegisterRequest_TopicDTO = new MentorRegisterRequest_TopicDTO(Topic);
            if (Topic.IsValidated)
            {
                return MentorRegisterRequest_TopicDTO;
            }
            else
            {
                return BadRequest(MentorRegisterRequest_TopicDTO);
            }
        }

        [Route(MentorRegisterRequestRoute.SaveMentorConnection), HttpPost]
        public async Task<ActionResult<MentorRegisterRequest_MentorConnectionDTO>> SaveMentorConnection([FromBody] MentorRegisterRequest_MentorConnectionDTO MentorRegisterRequest_MentorConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorRegisterRequest_MentorConnectionDTO.Id))
                return Forbid();

            MentorConnection MentorConnection = new MentorConnection()
            {
                Id = MentorRegisterRequest_MentorConnectionDTO.Id,
                MentorId = MentorRegisterRequest_MentorConnectionDTO.MentorId,
                Url = MentorRegisterRequest_MentorConnectionDTO.Url,
                ConnectionTypeId = MentorRegisterRequest_MentorConnectionDTO.ConnectionTypeId,
                Mentor = MentorRegisterRequest_MentorConnectionDTO.Mentor == null ? null : new AppUser
                {
                    Id = MentorRegisterRequest_MentorConnectionDTO.Mentor.Id
                },
                ConnectionType = MentorRegisterRequest_MentorConnectionDTO.ConnectionType == null ? null : new ConnectionType
                {
                    Id = MentorRegisterRequest_MentorConnectionDTO.ConnectionType.Id
                }
            };
            MentorConnection = await MentorConnectionService.Create(MentorConnection);
            MentorRegisterRequest_MentorConnectionDTO = new MentorRegisterRequest_MentorConnectionDTO(MentorConnection);
            if (MentorConnection.IsValidated)
            {
                return MentorRegisterRequest_MentorConnectionDTO;
            }
            else
            {
                return BadRequest(MentorRegisterRequest_MentorConnectionDTO);
            }
        }


        [Route(MentorRegisterRequestRoute.SaveActiveTime), HttpPost]
        public async Task<ActionResult<MentorRegisterRequest_ActiveTimeDTO>> SaveActiveTime([FromBody] MentorRegisterRequest_ActiveTimeDTO MentorRegisterRequest_ActiveTimeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorRegisterRequest_ActiveTimeDTO.Id))
                return Forbid();

            ActiveTime ActiveTime = new ActiveTime()
            {
                Id = MentorRegisterRequest_ActiveTimeDTO.Id,
                UnitOfTimeId = MentorRegisterRequest_ActiveTimeDTO.UnitOfTimeId, // bo sung constructor unit of time
                UnitOfTime = MentorRegisterRequest_ActiveTimeDTO.UnitOfTime == null ? null : new UnitOfTime
                {
                    Id = MentorRegisterRequest_ActiveTimeDTO.UnitOfTime.Id
                },
                ActiveDate = MentorRegisterRequest_ActiveTimeDTO.ActiveDate,
                MentorId = MentorRegisterRequest_ActiveTimeDTO.MentorId, // bo sung constructor app user
                Mentor = MentorRegisterRequest_ActiveTimeDTO.Mentor == null ? null : new AppUser
                {
                    Id = MentorRegisterRequest_ActiveTimeDTO.Mentor.Id
                }

            };
            ActiveTime = await ActiveTimeService.Create(ActiveTime);
            MentorRegisterRequest_ActiveTimeDTO = new MentorRegisterRequest_ActiveTimeDTO(ActiveTime);
            if (ActiveTime.IsValidated)
            {
                return MentorRegisterRequest_ActiveTimeDTO;
            }
            else
            {
                return BadRequest(MentorRegisterRequest_ActiveTimeDTO);
            }
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
            MentorRegisterRequest.UserId = MentorRegisterRequest_MentorRegisterRequestDTO.UserId;
            MentorRegisterRequest.TopicId = MentorRegisterRequest_MentorRegisterRequestDTO.TopicId;
            MentorRegisterRequest.MentorApprovalStatusId = MentorRegisterRequest_MentorRegisterRequestDTO.MentorApprovalStatusId;
            MentorRegisterRequest.Topic = MentorRegisterRequest_MentorRegisterRequestDTO.Topic == null ? null : new Topic
            {
                Id = MentorRegisterRequest_MentorRegisterRequestDTO.Topic.Id,
                Title = MentorRegisterRequest_MentorRegisterRequestDTO.Topic.Title,
                Description = MentorRegisterRequest_MentorRegisterRequestDTO.Topic.Description,
                Cost = MentorRegisterRequest_MentorRegisterRequestDTO.Topic.Cost,
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
