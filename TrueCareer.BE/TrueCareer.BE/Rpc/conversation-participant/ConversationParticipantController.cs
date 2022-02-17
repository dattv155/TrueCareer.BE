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
using TrueCareer.Services.MConversationParticipant;
using TrueCareer.Services.MConversation;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.conversation_participant
{
    public partial class ConversationParticipantController : RpcController
    {
        private IConversationService ConversationService;
        private IAppUserService AppUserService;
        private IConversationParticipantService ConversationParticipantService;
        private ICurrentContext CurrentContext;
        public ConversationParticipantController(
            IConversationService ConversationService,
            IAppUserService AppUserService,
            IConversationParticipantService ConversationParticipantService,
            ICurrentContext CurrentContext
        )
        {
            this.ConversationService = ConversationService;
            this.AppUserService = AppUserService;
            this.ConversationParticipantService = ConversationParticipantService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ConversationParticipantRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ConversationParticipant_ConversationParticipantFilterDTO ConversationParticipant_ConversationParticipantFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationParticipantFilter ConversationParticipantFilter = ConvertFilterDTOToFilterEntity(ConversationParticipant_ConversationParticipantFilterDTO);
            ConversationParticipantFilter = await ConversationParticipantService.ToFilter(ConversationParticipantFilter);
            int count = await ConversationParticipantService.Count(ConversationParticipantFilter);
            return count;
        }

        [Route(ConversationParticipantRoute.List), HttpPost]
        public async Task<ActionResult<List<ConversationParticipant_ConversationParticipantDTO>>> List([FromBody] ConversationParticipant_ConversationParticipantFilterDTO ConversationParticipant_ConversationParticipantFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationParticipantFilter ConversationParticipantFilter = ConvertFilterDTOToFilterEntity(ConversationParticipant_ConversationParticipantFilterDTO);
            ConversationParticipantFilter = await ConversationParticipantService.ToFilter(ConversationParticipantFilter);
            List<ConversationParticipant> ConversationParticipants = await ConversationParticipantService.List(ConversationParticipantFilter);
            List<ConversationParticipant_ConversationParticipantDTO> ConversationParticipant_ConversationParticipantDTOs = ConversationParticipants
                .Select(c => new ConversationParticipant_ConversationParticipantDTO(c)).ToList();
            return ConversationParticipant_ConversationParticipantDTOs;
        }

        [Route(ConversationParticipantRoute.Get), HttpPost]
        public async Task<ActionResult<ConversationParticipant_ConversationParticipantDTO>> Get([FromBody]ConversationParticipant_ConversationParticipantDTO ConversationParticipant_ConversationParticipantDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ConversationParticipant_ConversationParticipantDTO.Id))
                return Forbid();

            ConversationParticipant ConversationParticipant = await ConversationParticipantService.Get(ConversationParticipant_ConversationParticipantDTO.Id);
            return new ConversationParticipant_ConversationParticipantDTO(ConversationParticipant);
        }

        [Route(ConversationParticipantRoute.Create), HttpPost]
        public async Task<ActionResult<ConversationParticipant_ConversationParticipantDTO>> Create([FromBody] ConversationParticipant_ConversationParticipantDTO ConversationParticipant_ConversationParticipantDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ConversationParticipant_ConversationParticipantDTO.Id))
                return Forbid();

            ConversationParticipant ConversationParticipant = ConvertDTOToEntity(ConversationParticipant_ConversationParticipantDTO);
            ConversationParticipant = await ConversationParticipantService.Create(ConversationParticipant);
            ConversationParticipant_ConversationParticipantDTO = new ConversationParticipant_ConversationParticipantDTO(ConversationParticipant);
            if (ConversationParticipant.IsValidated)
                return ConversationParticipant_ConversationParticipantDTO;
            else
                return BadRequest(ConversationParticipant_ConversationParticipantDTO);
        }

        [Route(ConversationParticipantRoute.Update), HttpPost]
        public async Task<ActionResult<ConversationParticipant_ConversationParticipantDTO>> Update([FromBody] ConversationParticipant_ConversationParticipantDTO ConversationParticipant_ConversationParticipantDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ConversationParticipant_ConversationParticipantDTO.Id))
                return Forbid();

            ConversationParticipant ConversationParticipant = ConvertDTOToEntity(ConversationParticipant_ConversationParticipantDTO);
            ConversationParticipant = await ConversationParticipantService.Update(ConversationParticipant);
            ConversationParticipant_ConversationParticipantDTO = new ConversationParticipant_ConversationParticipantDTO(ConversationParticipant);
            if (ConversationParticipant.IsValidated)
                return ConversationParticipant_ConversationParticipantDTO;
            else
                return BadRequest(ConversationParticipant_ConversationParticipantDTO);
        }

        [Route(ConversationParticipantRoute.Delete), HttpPost]
        public async Task<ActionResult<ConversationParticipant_ConversationParticipantDTO>> Delete([FromBody] ConversationParticipant_ConversationParticipantDTO ConversationParticipant_ConversationParticipantDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ConversationParticipant_ConversationParticipantDTO.Id))
                return Forbid();

            ConversationParticipant ConversationParticipant = ConvertDTOToEntity(ConversationParticipant_ConversationParticipantDTO);
            ConversationParticipant = await ConversationParticipantService.Delete(ConversationParticipant);
            ConversationParticipant_ConversationParticipantDTO = new ConversationParticipant_ConversationParticipantDTO(ConversationParticipant);
            if (ConversationParticipant.IsValidated)
                return ConversationParticipant_ConversationParticipantDTO;
            else
                return BadRequest(ConversationParticipant_ConversationParticipantDTO);
        }
        
        [Route(ConversationParticipantRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationParticipantFilter ConversationParticipantFilter = new ConversationParticipantFilter();
            ConversationParticipantFilter = await ConversationParticipantService.ToFilter(ConversationParticipantFilter);
            ConversationParticipantFilter.Id = new IdFilter { In = Ids };
            ConversationParticipantFilter.Selects = ConversationParticipantSelect.Id;
            ConversationParticipantFilter.Skip = 0;
            ConversationParticipantFilter.Take = int.MaxValue;

            List<ConversationParticipant> ConversationParticipants = await ConversationParticipantService.List(ConversationParticipantFilter);
            ConversationParticipants = await ConversationParticipantService.BulkDelete(ConversationParticipants);
            if (ConversationParticipants.Any(x => !x.IsValidated))
                return BadRequest(ConversationParticipants.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(ConversationParticipantRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            ConversationFilter ConversationFilter = new ConversationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ConversationSelect.ALL
            };
            List<Conversation> Conversations = await ConversationService.List(ConversationFilter);
            AppUserFilter UserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Users = await AppUserService.List(UserFilter);
            List<ConversationParticipant> ConversationParticipants = new List<ConversationParticipant>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ConversationParticipants);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int ConversationIdColumn = 1 + StartColumn;
                int UserIdColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string ConversationIdValue = worksheet.Cells[i, ConversationIdColumn].Value?.ToString();
                    string UserIdValue = worksheet.Cells[i, UserIdColumn].Value?.ToString();
                    
                    ConversationParticipant ConversationParticipant = new ConversationParticipant();
                    Conversation Conversation = Conversations.Where(x => x.Id.ToString() == ConversationIdValue).FirstOrDefault();
                    ConversationParticipant.ConversationId = Conversation == null ? 0 : Conversation.Id;
                    ConversationParticipant.Conversation = Conversation;
                    AppUser User = Users.Where(x => x.Id.ToString() == UserIdValue).FirstOrDefault();
                    ConversationParticipant.UserId = User == null ? 0 : User.Id;
                    ConversationParticipant.User = User;
                    
                    ConversationParticipants.Add(ConversationParticipant);
                }
            }
            ConversationParticipants = await ConversationParticipantService.Import(ConversationParticipants);
            if (ConversationParticipants.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ConversationParticipants.Count; i++)
                {
                    ConversationParticipant ConversationParticipant = ConversationParticipants[i];
                    if (!ConversationParticipant.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ConversationParticipant.Errors.ContainsKey(nameof(ConversationParticipant.Id)))
                            Error += ConversationParticipant.Errors[nameof(ConversationParticipant.Id)];
                        if (ConversationParticipant.Errors.ContainsKey(nameof(ConversationParticipant.ConversationId)))
                            Error += ConversationParticipant.Errors[nameof(ConversationParticipant.ConversationId)];
                        if (ConversationParticipant.Errors.ContainsKey(nameof(ConversationParticipant.UserId)))
                            Error += ConversationParticipant.Errors[nameof(ConversationParticipant.UserId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ConversationParticipantRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ConversationParticipant_ConversationParticipantFilterDTO ConversationParticipant_ConversationParticipantFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ConversationParticipant
                var ConversationParticipantFilter = ConvertFilterDTOToFilterEntity(ConversationParticipant_ConversationParticipantFilterDTO);
                ConversationParticipantFilter.Skip = 0;
                ConversationParticipantFilter.Take = int.MaxValue;
                ConversationParticipantFilter = await ConversationParticipantService.ToFilter(ConversationParticipantFilter);
                List<ConversationParticipant> ConversationParticipants = await ConversationParticipantService.List(ConversationParticipantFilter);

                var ConversationParticipantHeaders = new List<string>()
                {
                    "Id",
                    "ConversationId",
                    "UserId",
                };
                List<object[]> ConversationParticipantData = new List<object[]>();
                for (int i = 0; i < ConversationParticipants.Count; i++)
                {
                    var ConversationParticipant = ConversationParticipants[i];
                    ConversationParticipantData.Add(new Object[]
                    {
                        ConversationParticipant.Id,
                        ConversationParticipant.ConversationId,
                        ConversationParticipant.UserId,
                    });
                }
                excel.GenerateWorksheet("ConversationParticipant", ConversationParticipantHeaders, ConversationParticipantData);
                #endregion
                
                #region Conversation
                var ConversationFilter = new ConversationFilter();
                ConversationFilter.Selects = ConversationSelect.ALL;
                ConversationFilter.OrderBy = ConversationOrder.Id;
                ConversationFilter.OrderType = OrderType.ASC;
                ConversationFilter.Skip = 0;
                ConversationFilter.Take = int.MaxValue;
                List<Conversation> Conversations = await ConversationService.List(ConversationFilter);

                var ConversationHeaders = new List<string>()
                {
                    "Id",
                    "LatestContent",
                    "LatestUserId",
                    "Hash",
                };
                List<object[]> ConversationData = new List<object[]>();
                for (int i = 0; i < Conversations.Count; i++)
                {
                    var Conversation = Conversations[i];
                    ConversationData.Add(new Object[]
                    {
                        Conversation.Id,
                        Conversation.LatestContent,
                        Conversation.LatestUserId,
                        Conversation.Hash,
                    });
                }
                excel.GenerateWorksheet("Conversation", ConversationHeaders, ConversationData);
                #endregion
                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string>()
                {
                    "Id",
                    "Username",
                    "Email",
                    "Phone",
                    "Password",
                    "DisplayName",
                    "SexId",
                    "Birthday",
                    "Avatar",
                    "CoverImage",
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.Password,
                        AppUser.DisplayName,
                        AppUser.SexId,
                        AppUser.Birthday,
                        AppUser.Avatar,
                        AppUser.CoverImage,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ConversationParticipant.xlsx");
        }

        [Route(ConversationParticipantRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] ConversationParticipant_ConversationParticipantFilterDTO ConversationParticipant_ConversationParticipantFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/ConversationParticipant_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "ConversationParticipant.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ConversationParticipantFilter ConversationParticipantFilter = new ConversationParticipantFilter();
            ConversationParticipantFilter = await ConversationParticipantService.ToFilter(ConversationParticipantFilter);
            if (Id == 0)
            {

            }
            else
            {
                ConversationParticipantFilter.Id = new IdFilter { Equal = Id };
                int count = await ConversationParticipantService.Count(ConversationParticipantFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ConversationParticipant ConvertDTOToEntity(ConversationParticipant_ConversationParticipantDTO ConversationParticipant_ConversationParticipantDTO)
        {
            ConversationParticipant_ConversationParticipantDTO.TrimString();
            ConversationParticipant ConversationParticipant = new ConversationParticipant();
            ConversationParticipant.Id = ConversationParticipant_ConversationParticipantDTO.Id;
            ConversationParticipant.ConversationId = ConversationParticipant_ConversationParticipantDTO.ConversationId;
            ConversationParticipant.UserId = ConversationParticipant_ConversationParticipantDTO.UserId;
            ConversationParticipant.Conversation = ConversationParticipant_ConversationParticipantDTO.Conversation == null ? null : new Conversation
            {
                Id = ConversationParticipant_ConversationParticipantDTO.Conversation.Id,
                LatestContent = ConversationParticipant_ConversationParticipantDTO.Conversation.LatestContent,
                LatestUserId = ConversationParticipant_ConversationParticipantDTO.Conversation.LatestUserId,
                Hash = ConversationParticipant_ConversationParticipantDTO.Conversation.Hash,
            };
            ConversationParticipant.User = ConversationParticipant_ConversationParticipantDTO.User == null ? null : new AppUser
            {
                Id = ConversationParticipant_ConversationParticipantDTO.User.Id,
                Username = ConversationParticipant_ConversationParticipantDTO.User.Username,
                Email = ConversationParticipant_ConversationParticipantDTO.User.Email,
                Phone = ConversationParticipant_ConversationParticipantDTO.User.Phone,
                Password = ConversationParticipant_ConversationParticipantDTO.User.Password,
                DisplayName = ConversationParticipant_ConversationParticipantDTO.User.DisplayName,
                SexId = ConversationParticipant_ConversationParticipantDTO.User.SexId,
                Birthday = ConversationParticipant_ConversationParticipantDTO.User.Birthday,
                Avatar = ConversationParticipant_ConversationParticipantDTO.User.Avatar,
                CoverImage = ConversationParticipant_ConversationParticipantDTO.User.CoverImage,
            };
            ConversationParticipant.BaseLanguage = CurrentContext.Language;
            return ConversationParticipant;
        }

        private ConversationParticipantFilter ConvertFilterDTOToFilterEntity(ConversationParticipant_ConversationParticipantFilterDTO ConversationParticipant_ConversationParticipantFilterDTO)
        {
            ConversationParticipantFilter ConversationParticipantFilter = new ConversationParticipantFilter();
            ConversationParticipantFilter.Selects = ConversationParticipantSelect.ALL;
            ConversationParticipantFilter.Skip = ConversationParticipant_ConversationParticipantFilterDTO.Skip;
            ConversationParticipantFilter.Take = ConversationParticipant_ConversationParticipantFilterDTO.Take;
            ConversationParticipantFilter.OrderBy = ConversationParticipant_ConversationParticipantFilterDTO.OrderBy;
            ConversationParticipantFilter.OrderType = ConversationParticipant_ConversationParticipantFilterDTO.OrderType;

            ConversationParticipantFilter.Id = ConversationParticipant_ConversationParticipantFilterDTO.Id;
            ConversationParticipantFilter.ConversationId = ConversationParticipant_ConversationParticipantFilterDTO.ConversationId;
            ConversationParticipantFilter.UserId = ConversationParticipant_ConversationParticipantFilterDTO.UserId;
            return ConversationParticipantFilter;
        }
    }
}

