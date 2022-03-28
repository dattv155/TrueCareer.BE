using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueCareer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Dynamic;
using TrueCareer.Entities;
using TrueCareer.Services.MConversationMessage;
using TrueCareer.Services.MConversation;
using TrueCareer.Services.MConversationType;
using TrueCareer.Services.MGlobalUser;
using TrueCareer.Common;
using TrueCareer.Service;
using TrueCareer.Repositories;
using ConversationMessage = TrueCareer.Entities.ConversationMessage;
using File = TrueCareer.Entities.File;

namespace TrueCareer.Rpc.conversation_message
{
    public partial class ConversationMessageController : RpcController
    {
        private IConversationService ConversationService;
        private IConversationTypeService ConversationTypeService;
        private IGlobalUserService GlobalUserService;
        private IConversationMessageService ConversationMessageService;
        private IFileService FileService;
        private ICurrentContext CurrentContext;
        private IUOW UOW;
        public ConversationMessageController(
            IConversationService ConversationService,
            IConversationTypeService ConversationTypeService,
            IGlobalUserService GlobalUserService,
            IConversationMessageService ConversationMessageService,
            IFileService FileService,
            ICurrentContext CurrentContext,
            IUOW UOW
        )
        {
            this.ConversationService = ConversationService;
            this.ConversationTypeService = ConversationTypeService;
            this.GlobalUserService = GlobalUserService;
            this.ConversationMessageService = ConversationMessageService;
            this.FileService = FileService;
            this.CurrentContext = CurrentContext;
            this.UOW = UOW;
        }

        [Route(ConversationMessageRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ConversationMessage_ConversationMessageFilterDTO ConversationMessage_ConversationMessageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessageFilter ConversationMessageFilter = ConvertFilterDTOToFilterEntity(ConversationMessage_ConversationMessageFilterDTO);
            int count = await ConversationMessageService.Count(ConversationMessageFilter);
            return count;
        }

        [Route(ConversationMessageRoute.List), HttpPost]
        public async Task<ActionResult<List<ConversationMessage_ConversationMessageDTO>>> List([FromBody] ConversationMessage_ConversationMessageFilterDTO ConversationMessage_ConversationMessageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessageFilter ConversationMessageFilter = ConvertFilterDTOToFilterEntity(ConversationMessage_ConversationMessageFilterDTO);
            List<ConversationMessage> ConversationMessages = await ConversationMessageService.List(ConversationMessageFilter);
            List<ConversationMessage_ConversationMessageDTO> ConversationMessage_ConversationMessageDTOs = ConversationMessages
                .Select(c => new ConversationMessage_ConversationMessageDTO(c)).ToList();
            return ConversationMessage_ConversationMessageDTOs;
        }

        [Route(ConversationMessageRoute.Get), HttpPost]
        public async Task<ActionResult<ConversationMessage_ConversationMessageDTO>> Get([FromBody] ConversationMessage_ConversationMessageDTO ConversationMessage_ConversationMessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessage ConversationMessage = await ConversationMessageService.Get(ConversationMessage_ConversationMessageDTO.Id);
            return new ConversationMessage_ConversationMessageDTO(ConversationMessage);
        }

        [Route(ConversationMessageRoute.Create), HttpPost]
        public async Task<ActionResult<ConversationMessage_ConversationMessageDTO>> Create([FromBody] ConversationMessage_ConversationMessageDTO ConversationMessage_ConversationMessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessage ConversationMessage = ConvertDTOToEntity(ConversationMessage_ConversationMessageDTO);
            ConversationMessage = await ConversationMessageService.CreateFromInside(ConversationMessage);
            ConversationMessage_ConversationMessageDTO = new ConversationMessage_ConversationMessageDTO(ConversationMessage);
            if (ConversationMessage.IsValidated)
                return ConversationMessage_ConversationMessageDTO;
            else
                return BadRequest(ConversationMessage_ConversationMessageDTO);
        }

        [Route(ConversationMessageRoute.Update), HttpPost]
        public async Task<ActionResult<ConversationMessage_ConversationMessageDTO>> Update([FromBody] ConversationMessage_ConversationMessageDTO ConversationMessage_ConversationMessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessage ConversationMessage = ConvertDTOToEntity(ConversationMessage_ConversationMessageDTO);
            ConversationMessage = await ConversationMessageService.Update(ConversationMessage);
            ConversationMessage_ConversationMessageDTO = new ConversationMessage_ConversationMessageDTO(ConversationMessage);
            if (ConversationMessage.IsValidated)
                return ConversationMessage_ConversationMessageDTO;
            else
                return BadRequest(ConversationMessage_ConversationMessageDTO);
        }

        [Route(ConversationMessageRoute.Delete), HttpPost]
        public async Task<ActionResult<ConversationMessage_ConversationMessageDTO>> Delete([FromBody] ConversationMessage_ConversationMessageDTO ConversationMessage_ConversationMessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessage ConversationMessage = ConvertDTOToEntity(ConversationMessage_ConversationMessageDTO);
            ConversationMessage = await ConversationMessageService.Delete(ConversationMessage);
            ConversationMessage_ConversationMessageDTO = new ConversationMessage_ConversationMessageDTO(ConversationMessage);
            if (ConversationMessage.IsValidated)
                return ConversationMessage_ConversationMessageDTO;
            else
                return BadRequest(ConversationMessage_ConversationMessageDTO);
        }
        [Route(ConversationMessageRoute.Read), HttpPost]
        public async Task<ActionResult<bool>> Read(long ConversationId, long GlobalUserId )
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var result = await ConversationMessageService.Read(ConversationId, GlobalUserId);
            return result;
        }

        private ConversationMessage ConvertDTOToEntity(ConversationMessage_ConversationMessageDTO ConversationMessage_ConversationMessageDTO)
        {
            ConversationMessage_ConversationMessageDTO.TrimString();
            ConversationMessage ConversationMessage = new ConversationMessage();
            ConversationMessage.Id = ConversationMessage_ConversationMessageDTO.Id;
            ConversationMessage.ConversationId = ConversationMessage_ConversationMessageDTO.ConversationId;
            ConversationMessage.GlobalUserId = ConversationMessage_ConversationMessageDTO.GlobalUserId;
            ConversationMessage.Content = ConversationMessage_ConversationMessageDTO.Content;
            ConversationMessage.Conversation = ConversationMessage_ConversationMessageDTO.Conversation == null ? null : new Conversation
            {
                Id = ConversationMessage_ConversationMessageDTO.Conversation.Id,
                ConversationTypeId = ConversationMessage_ConversationMessageDTO.Conversation.ConversationTypeId,
                Name = ConversationMessage_ConversationMessageDTO.Conversation.Name,
                Avatar = ConversationMessage_ConversationMessageDTO.Conversation.Avatar,
            };
            ConversationMessage.GlobalUser = ConversationMessage_ConversationMessageDTO.GlobalUser == null ? null : new GlobalUser
            {
                Id = ConversationMessage_ConversationMessageDTO.GlobalUser.Id,
                GlobalUserTypeId = ConversationMessage_ConversationMessageDTO.GlobalUser.GlobalUserTypeId,
                Username = ConversationMessage_ConversationMessageDTO.GlobalUser.Username,
                DisplayName = ConversationMessage_ConversationMessageDTO.GlobalUser.DisplayName,
                Avatar = ConversationMessage_ConversationMessageDTO.GlobalUser.Avatar,
            };
            ConversationMessage.ConversationAttachments = ConversationMessage_ConversationMessageDTO.ConversationAttachments?
                .Select(x => new ConversationAttachment
                {
                    Id = x.Id,
                    ConversationAttachmentTypeId = x.ConversationAttachmentTypeId,
                    Url = x.Url,
                    Thumbnail = x.Thumbnail,
                    Size = x.Size,
                    Name = x.Name,
                    Checksum = x.Checksum,
                    Type = x.Type,
                    ConversationAttachmentType = x.ConversationAttachmentType == null ? null : new ConversationAttachmentType
                    {
                        Id = x.ConversationAttachmentType.Id,
                        Code = x.ConversationAttachmentType.Code,
                        Name = x.ConversationAttachmentType.Name,
                    },
                }).ToList();
            ConversationMessage.BaseLanguage = CurrentContext.Language;
            return ConversationMessage;
        }

        private ConversationMessageFilter ConvertFilterDTOToFilterEntity(ConversationMessage_ConversationMessageFilterDTO ConversationMessage_ConversationMessageFilterDTO)
        {
            ConversationMessageFilter ConversationMessageFilter = new ConversationMessageFilter();
            ConversationMessageFilter.Selects = ConversationMessageSelect.ALL;
            ConversationMessageFilter.Skip = ConversationMessage_ConversationMessageFilterDTO.Skip;
            ConversationMessageFilter.Take = ConversationMessage_ConversationMessageFilterDTO.Take;
            ConversationMessageFilter.OrderBy = ConversationMessage_ConversationMessageFilterDTO.OrderBy;
            ConversationMessageFilter.OrderType = ConversationMessage_ConversationMessageFilterDTO.OrderType;

            ConversationMessageFilter.Id = ConversationMessage_ConversationMessageFilterDTO.Id;
            ConversationMessageFilter.ConversationId = ConversationMessage_ConversationMessageFilterDTO.ConversationId.Equal.Value;
            ConversationMessageFilter.GlobalUserId = ConversationMessage_ConversationMessageFilterDTO.GlobalUserId;
            ConversationMessageFilter.Content = ConversationMessage_ConversationMessageFilterDTO.Content;
            ConversationMessageFilter.CreatedAt = ConversationMessage_ConversationMessageFilterDTO.CreatedAt;
            ConversationMessageFilter.UpdatedAt = ConversationMessage_ConversationMessageFilterDTO.UpdatedAt;
            return ConversationMessageFilter;
        }


        [HttpPost]
        [Route(ConversationMessageRoute.UploadFile)]
        public async Task<ActionResult<ConversationMessage_FileDTO>> MultiUploadFile(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            FileInfo FileInfo = new FileInfo(file.FileName);
            string Name = file.FileName.ToGuid() + FileInfo.Extension;
            string OriginalName = file.FileName;
            File File = new Entities.File
            {
                Path = $"/conversation-message/{CurrentContext.UserId}/{StaticParams.DateTimeNow.ToFileTimeUtc()}/{Name}",
                Name = Name,
                OriginalName = OriginalName,
                Content = memoryStream.ToArray()
            };
            File = await FileService.Create(File);
            if (File == null)
                return BadRequest();
            File.Path = "/rpc/TrueCareer/public-file/download" + File.Path;
            ConversationMessage_FileDTO ConversationMessage_FileDTO = new ConversationMessage_FileDTO(File);
            return Ok(ConversationMessage_FileDTO);
        }
        [HttpPost]
        [Route(ConversationMessageRoute.MultiUploadFile)]
        public async Task<ActionResult<List<ConversationMessage_FileDTO>>> MultiUploadFile(List<IFormFile> files)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<ConversationMessage_FileDTO> ConversationMessage_FileDTOs = new List<ConversationMessage_FileDTO>();
            foreach (IFormFile file in files)
            {
                MemoryStream memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                FileInfo FileInfo = new FileInfo(file.FileName);
                string Name = file.FileName.ToGuid() + FileInfo.Extension;
                string OriginalName = file.FileName;
                File File = new File()
                {
                    Path = $"/conversation-message/{CurrentContext.UserId}/{StaticParams.DateTimeNow.ToFileTimeUtc()}/{Name}",
                    Name = Name,
                    OriginalName = OriginalName,
                    Content = memoryStream.ToArray()
                };
                File = await FileService.Create(File);
                if (File == null)
                    return BadRequest();
                File.Path = "/rpc/truecareer/public-file/download" + File.Path;
                ConversationMessage_FileDTO ConversationMessage_FileDTO = new ConversationMessage_FileDTO(File);
                ConversationMessage_FileDTOs.Add(ConversationMessage_FileDTO);
            }
            return Ok(ConversationMessage_FileDTOs);
        }
    }
}

