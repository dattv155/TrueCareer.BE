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
using TrueCareer.Services.MConversation;
using TrueCareer.Services.MConversationMessage;
using TrueCareer.Services.MConversationType;
using TrueCareer.Services.MGlobalUser;
using TrueCareer.Service;
using TrueCareer.Common;

namespace TrueCareer.Rpc.conversation
{
    public partial class ConversationController : RpcController
    {
        private IConversationMessageService ConversationMessageService;
        private IConversationTypeService ConversationTypeService;
        private IGlobalUserService GlobalUserService;
        private IConversationService ConversationService;
        private IFileService FileService;
        private ICurrentContext CurrentContext;
        public ConversationController(
            IConversationMessageService ConversationMessageService,
            IConversationTypeService ConversationTypeService,
            IGlobalUserService GlobalUserService,
            IConversationService ConversationService,
            IFileService FileService,
            ICurrentContext CurrentContext
        )
        {
            this.ConversationMessageService = ConversationMessageService;
            this.ConversationTypeService = ConversationTypeService;
            this.GlobalUserService = GlobalUserService;
            this.ConversationService = ConversationService;
            this.FileService = FileService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ConversationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Conversation_ConversationFilterDTO Conversation_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationFilter ConversationFilter = ConvertFilterDTOToFilterEntity(Conversation_ConversationFilterDTO);
            int count = await ConversationService.Count(ConversationFilter);
            return count;
        }

        [Route(ConversationRoute.List), HttpPost]
        public async Task<ActionResult<List<Conversation_ConversationDTO>>> List([FromBody] Conversation_ConversationFilterDTO Conversation_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationFilter ConversationFilter = ConvertFilterDTOToFilterEntity(Conversation_ConversationFilterDTO);
            List<Conversation> Conversations = await ConversationService.List(ConversationFilter);
            List<Conversation_ConversationDTO> Conversation_ConversationDTOs = Conversations
                .Select(c => new Conversation_ConversationDTO(c)).ToList();
            return Conversation_ConversationDTOs;
        }

        [Route(ConversationRoute.Get), HttpPost]
        public async Task<ActionResult<Conversation_ConversationDTO>> Get([FromBody]Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Conversation Conversation = await ConversationService.Get(Conversation_ConversationDTO.Id);
            return new Conversation_ConversationDTO(Conversation);
        }

        [Route(ConversationRoute.Create), HttpPost]
        public async Task<ActionResult<Conversation_ConversationDTO>> Create([FromBody] Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Conversation Conversation = ConvertDTOToEntity(Conversation_ConversationDTO);
            Conversation = await ConversationService.Create(Conversation);
            Conversation_ConversationDTO = new Conversation_ConversationDTO(Conversation);
            if (Conversation.IsValidated)
                return Conversation_ConversationDTO;
            else
                return BadRequest(Conversation_ConversationDTO);
        }

        [Route(ConversationRoute.Update), HttpPost]
        public async Task<ActionResult<Conversation_ConversationDTO>> Update([FromBody] Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Conversation Conversation = ConvertDTOToEntity(Conversation_ConversationDTO);
            Conversation = await ConversationService.Update(Conversation);
            Conversation_ConversationDTO = new Conversation_ConversationDTO(Conversation);
            if (Conversation.IsValidated)
                return Conversation_ConversationDTO;
            else
                return BadRequest(Conversation_ConversationDTO);
        }

        [Route(ConversationRoute.Delete), HttpPost]
        public async Task<ActionResult<Conversation_ConversationDTO>> Delete([FromBody] Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Conversation Conversation = ConvertDTOToEntity(Conversation_ConversationDTO);
            Conversation = await ConversationService.Delete(Conversation);
            Conversation_ConversationDTO = new Conversation_ConversationDTO(Conversation);
            if (Conversation.IsValidated)
                return Conversation_ConversationDTO;
            else
                return BadRequest(Conversation_ConversationDTO);
        }

        private Conversation ConvertDTOToEntity(Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            Conversation_ConversationDTO.TrimString();
            Conversation Conversation = new Conversation();
            Conversation.Id = Conversation_ConversationDTO.Id;
            Conversation.Name = Conversation_ConversationDTO.Name;
            Conversation.Avatar = Conversation_ConversationDTO.Avatar;
            Conversation.ConversationParticipants = Conversation_ConversationDTO.ConversationParticipants?
                .Select(x => new ConversationParticipant
                {
                    Id = x.Id,
                    GlobalUserId = x.GlobalUserId,
                    GlobalUser = x.GlobalUser == null ? null : new GlobalUser
                    {
                        Id = x.GlobalUser.Id,
                        Username = x.GlobalUser.Username,
                        DisplayName = x.GlobalUser.DisplayName,
                        RowId = x.GlobalUser.RowId,
                    },
                }).ToList();
            Conversation.BaseLanguage = CurrentContext.Language;
            return Conversation;
        }

        private ConversationFilter ConvertFilterDTOToFilterEntity(Conversation_ConversationFilterDTO Conversation_ConversationFilterDTO)
        {
            ConversationFilter ConversationFilter = new ConversationFilter();
            ConversationFilter.Selects = ConversationSelect.ALL;
            ConversationFilter.Skip = Conversation_ConversationFilterDTO.Skip;
            ConversationFilter.Take = Conversation_ConversationFilterDTO.Take;
            ConversationFilter.OrderBy = ConversationOrder.UpdatedAt;
            ConversationFilter.OrderType = OrderType.DESC;

            ConversationFilter.Id = Conversation_ConversationFilterDTO.Id;
            ConversationFilter.Name = Conversation_ConversationFilterDTO.Name;
            ConversationFilter.CreatedAt = Conversation_ConversationFilterDTO.CreatedAt;
            ConversationFilter.UpdatedAt = Conversation_ConversationFilterDTO.UpdatedAt;
            ConversationFilter.ConversationTypeId = Conversation_ConversationFilterDTO.ConversationTypeId;
            return ConversationFilter;
        }


        [Route(ConversationRoute.GetGlobalUser), HttpPost]
        public async Task<Conversation_GlobalUserDTO> GetGlobalUser([FromBody] Conversation_GlobalUserDTO Conversation_GlobalUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GlobalUser GlobalUser = await GlobalUserService.Get(Conversation_GlobalUserDTO.RowId);
            Conversation_GlobalUserDTO = new Conversation_GlobalUserDTO(GlobalUser);
            return Conversation_GlobalUserDTO;
        }

        [HttpPost]
        [Route(ConversationRoute.UploadFile)]
        public async Task<ActionResult<Conversation_FileDTO>> UploadFile(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            Entities.File File = new Entities.File
            {
                Path = $"/conversation/{CurrentContext.UserId}/{StaticParams.DateTimeNow.ToFileTimeUtc()}/{file.FileName}",
                Name = file.FileName,
                Content = memoryStream.ToArray()
            };
            File = await FileService.Create(File);
            if (File == null)
                return BadRequest();
            File.Path = "/rpc/TrueCareer/file/download" + File.Path;
            Conversation_FileDTO EndUser_FileDTO = new Conversation_FileDTO(File);
            return Ok(EndUser_FileDTO);
        }

        [HttpPost]
        [Route(ConversationRoute.UploadAvatar)]
        public async Task<ActionResult<Conversation_ConversationDTO>> UploadAvatar([FromForm]long ConversationId, IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            Entities.File File = new Entities.File
            {
                Path = $"/conversation/{CurrentContext.UserId}/{StaticParams.DateTimeNow.ToFileTimeUtc()}/{file.FileName}",
                Name = file.FileName,
                Content = memoryStream.ToArray()
            };
            File = await FileService.Create(File);
            if (File == null)
                return BadRequest(file);
            Conversation Conversation = await ConversationService.Get(ConversationId);
            if (Conversation == null)
                return BadRequest(ConversationId);
            Conversation.Avatar = "/rpc/TrueCareer/file/download" + File.Path;
            Conversation = await ConversationService.Update(Conversation);
            Conversation_ConversationDTO Conversation_ConversationDTO = new Conversation_ConversationDTO(Conversation);

            if (!Conversation.IsValidated)
                return BadRequest(Conversation_ConversationDTO);
            return Ok(Conversation_ConversationDTO);
        }

        [HttpPost]
        [Route(ConversationRoute.MultiUploadFile)]
        public async Task<ActionResult<Conversation_FileDTO>> MultiUploadFile(List<IFormFile> files)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<Conversation_FileDTO> Conversation_FileDTOs = new List<Conversation_FileDTO>();
            foreach (IFormFile file in files)
            {
                MemoryStream memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                Entities.File File = new Entities.File
                {
                    Path = $"/conversation/{CurrentContext.UserId}/{StaticParams.DateTimeNow.ToFileTimeUtc()}/{file.FileName}",
                    Name = file.FileName,
                    Content = memoryStream.ToArray()
                };
                File = await FileService.Create(File);
                if (File == null)
                    return BadRequest();
                File.Path = "/rpc/TrueCareer/file/download" + File.Path;
                Conversation_FileDTO Conversation_FileDTO = new Conversation_FileDTO(File);
                Conversation_FileDTOs.Add(Conversation_FileDTO);
            }
            return Ok(Conversation_FileDTOs);
        }
    }
}

