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
using TrueCareer.Services.MConversation;
using TrueCareer.Services.MMessage;

namespace TrueCareer.Rpc.conversation
{
    public partial class ConversationController : RpcController
    {
        private IMessageService MessageService;
        private IConversationService ConversationService;
        private ICurrentContext CurrentContext;
        public ConversationController(
            IMessageService MessageService,
            IConversationService ConversationService,
            ICurrentContext CurrentContext
        )
        {
            this.MessageService = MessageService;
            this.ConversationService = ConversationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ConversationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Conversation_ConversationFilterDTO Conversation_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationFilter ConversationFilter = ConvertFilterDTOToFilterEntity(Conversation_ConversationFilterDTO);
            ConversationFilter = await ConversationService.ToFilter(ConversationFilter);
            int count = await ConversationService.Count(ConversationFilter);
            return count;
        }

        [Route(ConversationRoute.List), HttpPost]
        public async Task<ActionResult<List<Conversation_ConversationDTO>>> List([FromBody] Conversation_ConversationFilterDTO Conversation_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationFilter ConversationFilter = ConvertFilterDTOToFilterEntity(Conversation_ConversationFilterDTO);
            ConversationFilter = await ConversationService.ToFilter(ConversationFilter);
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

            if (!await HasPermission(Conversation_ConversationDTO.Id))
                return Forbid();

            Conversation Conversation = await ConversationService.Get(Conversation_ConversationDTO.Id);
            return new Conversation_ConversationDTO(Conversation);
        }

        [Route(ConversationRoute.Create), HttpPost]
        public async Task<ActionResult<Conversation_ConversationDTO>> Create([FromBody] Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Conversation_ConversationDTO.Id))
                return Forbid();

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
            
            if (!await HasPermission(Conversation_ConversationDTO.Id))
                return Forbid();

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

            if (!await HasPermission(Conversation_ConversationDTO.Id))
                return Forbid();

            Conversation Conversation = ConvertDTOToEntity(Conversation_ConversationDTO);
            Conversation = await ConversationService.Delete(Conversation);
            Conversation_ConversationDTO = new Conversation_ConversationDTO(Conversation);
            if (Conversation.IsValidated)
                return Conversation_ConversationDTO;
            else
                return BadRequest(Conversation_ConversationDTO);
        }
        
        [Route(ConversationRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationFilter ConversationFilter = new ConversationFilter();
            ConversationFilter = await ConversationService.ToFilter(ConversationFilter);
            ConversationFilter.Id = new IdFilter { In = Ids };
            ConversationFilter.Selects = ConversationSelect.Id;
            ConversationFilter.Skip = 0;
            ConversationFilter.Take = int.MaxValue;

            List<Conversation> Conversations = await ConversationService.List(ConversationFilter);
            Conversations = await ConversationService.BulkDelete(Conversations);
            if (Conversations.Any(x => !x.IsValidated))
                return BadRequest(Conversations.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(ConversationRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<Conversation> Conversations = new List<Conversation>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Conversations);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int LatestContentColumn = 1 + StartColumn;
                int LatestUserIdColumn = 3 + StartColumn;
                int HashColumn = 7 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string LatestContentValue = worksheet.Cells[i, LatestContentColumn].Value?.ToString();
                    string LatestUserIdValue = worksheet.Cells[i, LatestUserIdColumn].Value?.ToString();
                    string HashValue = worksheet.Cells[i, HashColumn].Value?.ToString();
                    
                    Conversation Conversation = new Conversation();
                    Conversation.LatestContent = LatestContentValue;
                    Conversation.Hash = HashValue;
                    
                    Conversations.Add(Conversation);
                }
            }
            Conversations = await ConversationService.Import(Conversations);
            if (Conversations.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Conversations.Count; i++)
                {
                    Conversation Conversation = Conversations[i];
                    if (!Conversation.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Conversation.Errors.ContainsKey(nameof(Conversation.Id)))
                            Error += Conversation.Errors[nameof(Conversation.Id)];
                        if (Conversation.Errors.ContainsKey(nameof(Conversation.LatestContent)))
                            Error += Conversation.Errors[nameof(Conversation.LatestContent)];
                        if (Conversation.Errors.ContainsKey(nameof(Conversation.LatestUserId)))
                            Error += Conversation.Errors[nameof(Conversation.LatestUserId)];
                        if (Conversation.Errors.ContainsKey(nameof(Conversation.Hash)))
                            Error += Conversation.Errors[nameof(Conversation.Hash)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ConversationRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Conversation_ConversationFilterDTO Conversation_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Conversation
                var ConversationFilter = ConvertFilterDTOToFilterEntity(Conversation_ConversationFilterDTO);
                ConversationFilter.Skip = 0;
                ConversationFilter.Take = int.MaxValue;
                ConversationFilter = await ConversationService.ToFilter(ConversationFilter);
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
                
                #region Message
                var MessageFilter = new MessageFilter();
                MessageFilter.Selects = MessageSelect.ALL;
                MessageFilter.OrderBy = MessageOrder.Id;
                MessageFilter.OrderType = OrderType.ASC;
                MessageFilter.Skip = 0;
                MessageFilter.Take = int.MaxValue;
                List<Message> Messages = await MessageService.List(MessageFilter);

                var MessageHeaders = new List<string>()
                {
                    "Id",
                    "UserId",
                    "Content",
                    "ConversationId",
                };
                List<object[]> MessageData = new List<object[]>();
                for (int i = 0; i < Messages.Count; i++)
                {
                    var Message = Messages[i];
                    MessageData.Add(new Object[]
                    {
                        Message.Id,
                        Message.UserId,
                        Message.Content,
                        Message.ConversationId,
                    });
                }
                excel.GenerateWorksheet("Message", MessageHeaders, MessageData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Conversation.xlsx");
        }

        [Route(ConversationRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Conversation_ConversationFilterDTO Conversation_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Conversation_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Conversation.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ConversationFilter ConversationFilter = new ConversationFilter();
            ConversationFilter = await ConversationService.ToFilter(ConversationFilter);
            if (Id == 0)
            {

            }
            else
            {
                ConversationFilter.Id = new IdFilter { Equal = Id };
                int count = await ConversationService.Count(ConversationFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Conversation ConvertDTOToEntity(Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            Conversation_ConversationDTO.TrimString();
            Conversation Conversation = new Conversation();
            Conversation.Id = Conversation_ConversationDTO.Id;
            Conversation.LatestContent = Conversation_ConversationDTO.LatestContent;
            Conversation.LatestUserId = Conversation_ConversationDTO.LatestUserId;
            Conversation.Hash = Conversation_ConversationDTO.Hash;
            Conversation.Messages = Conversation_ConversationDTO.Messages?
                .Select(x => new Message
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Content = x.Content,
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
            ConversationFilter.OrderBy = Conversation_ConversationFilterDTO.OrderBy;
            ConversationFilter.OrderType = Conversation_ConversationFilterDTO.OrderType;

            ConversationFilter.Id = Conversation_ConversationFilterDTO.Id;
            ConversationFilter.LatestContent = Conversation_ConversationFilterDTO.LatestContent;
            ConversationFilter.LatestUserId = Conversation_ConversationFilterDTO.LatestUserId;
            ConversationFilter.Hash = Conversation_ConversationFilterDTO.Hash;
            ConversationFilter.CreatedAt = Conversation_ConversationFilterDTO.CreatedAt;
            ConversationFilter.UpdatedAt = Conversation_ConversationFilterDTO.UpdatedAt;
            return ConversationFilter;
        }
    }
}

