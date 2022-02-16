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
using TrueCareer.Services.MMessage;
using TrueCareer.Services.MConversation;

namespace TrueCareer.Rpc.message
{
    public partial class MessageController : RpcController
    {
        private IConversationService ConversationService;
        private IMessageService MessageService;
        private ICurrentContext CurrentContext;
        public MessageController(
            IConversationService ConversationService,
            IMessageService MessageService,
            ICurrentContext CurrentContext
        )
        {
            this.ConversationService = ConversationService;
            this.MessageService = MessageService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MessageRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Message_MessageFilterDTO Message_MessageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MessageFilter MessageFilter = ConvertFilterDTOToFilterEntity(Message_MessageFilterDTO);
            MessageFilter = await MessageService.ToFilter(MessageFilter);
            int count = await MessageService.Count(MessageFilter);
            return count;
        }

        [Route(MessageRoute.List), HttpPost]
        public async Task<ActionResult<List<Message_MessageDTO>>> List([FromBody] Message_MessageFilterDTO Message_MessageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MessageFilter MessageFilter = ConvertFilterDTOToFilterEntity(Message_MessageFilterDTO);
            MessageFilter = await MessageService.ToFilter(MessageFilter);
            List<Message> Messages = await MessageService.List(MessageFilter);
            List<Message_MessageDTO> Message_MessageDTOs = Messages
                .Select(c => new Message_MessageDTO(c)).ToList();
            return Message_MessageDTOs;
        }

        [Route(MessageRoute.Get), HttpPost]
        public async Task<ActionResult<Message_MessageDTO>> Get([FromBody]Message_MessageDTO Message_MessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Message_MessageDTO.Id))
                return Forbid();

            Message Message = await MessageService.Get(Message_MessageDTO.Id);
            return new Message_MessageDTO(Message);
        }

        [Route(MessageRoute.Create), HttpPost]
        public async Task<ActionResult<Message_MessageDTO>> Create([FromBody] Message_MessageDTO Message_MessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Message_MessageDTO.Id))
                return Forbid();

            Message Message = ConvertDTOToEntity(Message_MessageDTO);
            Message = await MessageService.Create(Message);
            Message_MessageDTO = new Message_MessageDTO(Message);
            if (Message.IsValidated)
                return Message_MessageDTO;
            else
                return BadRequest(Message_MessageDTO);
        }

        [Route(MessageRoute.Update), HttpPost]
        public async Task<ActionResult<Message_MessageDTO>> Update([FromBody] Message_MessageDTO Message_MessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Message_MessageDTO.Id))
                return Forbid();

            Message Message = ConvertDTOToEntity(Message_MessageDTO);
            Message = await MessageService.Update(Message);
            Message_MessageDTO = new Message_MessageDTO(Message);
            if (Message.IsValidated)
                return Message_MessageDTO;
            else
                return BadRequest(Message_MessageDTO);
        }

        [Route(MessageRoute.Delete), HttpPost]
        public async Task<ActionResult<Message_MessageDTO>> Delete([FromBody] Message_MessageDTO Message_MessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Message_MessageDTO.Id))
                return Forbid();

            Message Message = ConvertDTOToEntity(Message_MessageDTO);
            Message = await MessageService.Delete(Message);
            Message_MessageDTO = new Message_MessageDTO(Message);
            if (Message.IsValidated)
                return Message_MessageDTO;
            else
                return BadRequest(Message_MessageDTO);
        }
        
        [Route(MessageRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MessageFilter MessageFilter = new MessageFilter();
            MessageFilter = await MessageService.ToFilter(MessageFilter);
            MessageFilter.Id = new IdFilter { In = Ids };
            MessageFilter.Selects = MessageSelect.Id;
            MessageFilter.Skip = 0;
            MessageFilter.Take = int.MaxValue;

            List<Message> Messages = await MessageService.List(MessageFilter);
            Messages = await MessageService.BulkDelete(Messages);
            if (Messages.Any(x => !x.IsValidated))
                return BadRequest(Messages.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(MessageRoute.Import), HttpPost]
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
            List<Message> Messages = new List<Message>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Messages);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int UserIdColumn = 1 + StartColumn;
                int ContentColumn = 2 + StartColumn;
                int ConversationIdColumn = 6 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string UserIdValue = worksheet.Cells[i, UserIdColumn].Value?.ToString();
                    string ContentValue = worksheet.Cells[i, ContentColumn].Value?.ToString();
                    string ConversationIdValue = worksheet.Cells[i, ConversationIdColumn].Value?.ToString();
                    
                    Message Message = new Message();
                    Message.Content = ContentValue;
                    Conversation Conversation = Conversations.Where(x => x.Id.ToString() == ConversationIdValue).FirstOrDefault();
                    Message.ConversationId = Conversation == null ? 0 : Conversation.Id;
                    Message.Conversation = Conversation;
                    
                    Messages.Add(Message);
                }
            }
            Messages = await MessageService.Import(Messages);
            if (Messages.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Messages.Count; i++)
                {
                    Message Message = Messages[i];
                    if (!Message.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Message.Errors.ContainsKey(nameof(Message.Id)))
                            Error += Message.Errors[nameof(Message.Id)];
                        if (Message.Errors.ContainsKey(nameof(Message.UserId)))
                            Error += Message.Errors[nameof(Message.UserId)];
                        if (Message.Errors.ContainsKey(nameof(Message.Content)))
                            Error += Message.Errors[nameof(Message.Content)];
                        if (Message.Errors.ContainsKey(nameof(Message.ConversationId)))
                            Error += Message.Errors[nameof(Message.ConversationId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(MessageRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Message_MessageFilterDTO Message_MessageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Message
                var MessageFilter = ConvertFilterDTOToFilterEntity(Message_MessageFilterDTO);
                MessageFilter.Skip = 0;
                MessageFilter.Take = int.MaxValue;
                MessageFilter = await MessageService.ToFilter(MessageFilter);
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Message.xlsx");
        }

        [Route(MessageRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Message_MessageFilterDTO Message_MessageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Message_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Message.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            MessageFilter MessageFilter = new MessageFilter();
            MessageFilter = await MessageService.ToFilter(MessageFilter);
            if (Id == 0)
            {

            }
            else
            {
                MessageFilter.Id = new IdFilter { Equal = Id };
                int count = await MessageService.Count(MessageFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Message ConvertDTOToEntity(Message_MessageDTO Message_MessageDTO)
        {
            Message_MessageDTO.TrimString();
            Message Message = new Message();
            Message.Id = Message_MessageDTO.Id;
            Message.UserId = Message_MessageDTO.UserId;
            Message.Content = Message_MessageDTO.Content;
            Message.ConversationId = Message_MessageDTO.ConversationId;
            Message.Conversation = Message_MessageDTO.Conversation == null ? null : new Conversation
            {
                Id = Message_MessageDTO.Conversation.Id,
                LatestContent = Message_MessageDTO.Conversation.LatestContent,
                LatestUserId = Message_MessageDTO.Conversation.LatestUserId,
                Hash = Message_MessageDTO.Conversation.Hash,
            };
            Message.BaseLanguage = CurrentContext.Language;
            return Message;
        }

        private MessageFilter ConvertFilterDTOToFilterEntity(Message_MessageFilterDTO Message_MessageFilterDTO)
        {
            MessageFilter MessageFilter = new MessageFilter();
            MessageFilter.Selects = MessageSelect.ALL;
            MessageFilter.Skip = Message_MessageFilterDTO.Skip;
            MessageFilter.Take = Message_MessageFilterDTO.Take;
            MessageFilter.OrderBy = Message_MessageFilterDTO.OrderBy;
            MessageFilter.OrderType = Message_MessageFilterDTO.OrderType;

            MessageFilter.Id = Message_MessageFilterDTO.Id;
            MessageFilter.UserId = Message_MessageFilterDTO.UserId;
            MessageFilter.Content = Message_MessageFilterDTO.Content;
            MessageFilter.ConversationId = Message_MessageFilterDTO.ConversationId;
            MessageFilter.CreatedAt = Message_MessageFilterDTO.CreatedAt;
            MessageFilter.UpdatedAt = Message_MessageFilterDTO.UpdatedAt;
            return MessageFilter;
        }
    }
}

