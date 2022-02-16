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
using TrueCareer.Services.MNotification;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.notification
{
    public partial class NotificationController : RpcController
    {
        private IAppUserService AppUserService;
        private INotificationService NotificationService;
        private ICurrentContext CurrentContext;
        public NotificationController(
            IAppUserService AppUserService,
            INotificationService NotificationService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.NotificationService = NotificationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(NotificationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Notification_NotificationFilterDTO Notification_NotificationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NotificationFilter NotificationFilter = ConvertFilterDTOToFilterEntity(Notification_NotificationFilterDTO);
            NotificationFilter = await NotificationService.ToFilter(NotificationFilter);
            int count = await NotificationService.Count(NotificationFilter);
            return count;
        }

        [Route(NotificationRoute.List), HttpPost]
        public async Task<ActionResult<List<Notification_NotificationDTO>>> List([FromBody] Notification_NotificationFilterDTO Notification_NotificationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NotificationFilter NotificationFilter = ConvertFilterDTOToFilterEntity(Notification_NotificationFilterDTO);
            NotificationFilter = await NotificationService.ToFilter(NotificationFilter);
            List<Notification> Notifications = await NotificationService.List(NotificationFilter);
            List<Notification_NotificationDTO> Notification_NotificationDTOs = Notifications
                .Select(c => new Notification_NotificationDTO(c)).ToList();
            return Notification_NotificationDTOs;
        }

        [Route(NotificationRoute.Get), HttpPost]
        public async Task<ActionResult<Notification_NotificationDTO>> Get([FromBody]Notification_NotificationDTO Notification_NotificationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Notification_NotificationDTO.Id))
                return Forbid();

            Notification Notification = await NotificationService.Get(Notification_NotificationDTO.Id);
            return new Notification_NotificationDTO(Notification);
        }

        [Route(NotificationRoute.Create), HttpPost]
        public async Task<ActionResult<Notification_NotificationDTO>> Create([FromBody] Notification_NotificationDTO Notification_NotificationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Notification_NotificationDTO.Id))
                return Forbid();

            Notification Notification = ConvertDTOToEntity(Notification_NotificationDTO);
            Notification = await NotificationService.Create(Notification);
            Notification_NotificationDTO = new Notification_NotificationDTO(Notification);
            if (Notification.IsValidated)
                return Notification_NotificationDTO;
            else
                return BadRequest(Notification_NotificationDTO);
        }

        [Route(NotificationRoute.Update), HttpPost]
        public async Task<ActionResult<Notification_NotificationDTO>> Update([FromBody] Notification_NotificationDTO Notification_NotificationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Notification_NotificationDTO.Id))
                return Forbid();

            Notification Notification = ConvertDTOToEntity(Notification_NotificationDTO);
            Notification = await NotificationService.Update(Notification);
            Notification_NotificationDTO = new Notification_NotificationDTO(Notification);
            if (Notification.IsValidated)
                return Notification_NotificationDTO;
            else
                return BadRequest(Notification_NotificationDTO);
        }

        [Route(NotificationRoute.Delete), HttpPost]
        public async Task<ActionResult<Notification_NotificationDTO>> Delete([FromBody] Notification_NotificationDTO Notification_NotificationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Notification_NotificationDTO.Id))
                return Forbid();

            Notification Notification = ConvertDTOToEntity(Notification_NotificationDTO);
            Notification = await NotificationService.Delete(Notification);
            Notification_NotificationDTO = new Notification_NotificationDTO(Notification);
            if (Notification.IsValidated)
                return Notification_NotificationDTO;
            else
                return BadRequest(Notification_NotificationDTO);
        }
        
        [Route(NotificationRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NotificationFilter NotificationFilter = new NotificationFilter();
            NotificationFilter = await NotificationService.ToFilter(NotificationFilter);
            NotificationFilter.Id = new IdFilter { In = Ids };
            NotificationFilter.Selects = NotificationSelect.Id;
            NotificationFilter.Skip = 0;
            NotificationFilter.Take = int.MaxValue;

            List<Notification> Notifications = await NotificationService.List(NotificationFilter);
            Notifications = await NotificationService.BulkDelete(Notifications);
            if (Notifications.Any(x => !x.IsValidated))
                return BadRequest(Notifications.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(NotificationRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter RecipientFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Recipients = await AppUserService.List(RecipientFilter);
            AppUserFilter SenderFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Senders = await AppUserService.List(SenderFilter);
            List<Notification> Notifications = new List<Notification>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Notifications);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int TitleWebColumn = 1 + StartColumn;
                int ContentWebColumn = 2 + StartColumn;
                int SenderIdColumn = 3 + StartColumn;
                int RecipientIdColumn = 4 + StartColumn;
                int UnreadColumn = 5 + StartColumn;
                int TimeColumn = 6 + StartColumn;
                int LinkWebsiteColumn = 7 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string TitleWebValue = worksheet.Cells[i, TitleWebColumn].Value?.ToString();
                    string ContentWebValue = worksheet.Cells[i, ContentWebColumn].Value?.ToString();
                    string SenderIdValue = worksheet.Cells[i, SenderIdColumn].Value?.ToString();
                    string RecipientIdValue = worksheet.Cells[i, RecipientIdColumn].Value?.ToString();
                    string UnreadValue = worksheet.Cells[i, UnreadColumn].Value?.ToString();
                    string TimeValue = worksheet.Cells[i, TimeColumn].Value?.ToString();
                    string LinkWebsiteValue = worksheet.Cells[i, LinkWebsiteColumn].Value?.ToString();
                    
                    Notification Notification = new Notification();
                    Notification.TitleWeb = TitleWebValue;
                    Notification.ContentWeb = ContentWebValue;
                    Notification.Unread = bool.TryParse(UnreadValue, out bool Unread) ? Unread : false;
                    Notification.Time = DateTime.TryParse(TimeValue, out DateTime Time) ? Time : DateTime.Now;
                    Notification.LinkWebsite = LinkWebsiteValue;
                    AppUser Recipient = Recipients.Where(x => x.Id.ToString() == RecipientIdValue).FirstOrDefault();
                    Notification.RecipientId = Recipient == null ? 0 : Recipient.Id;
                    Notification.Recipient = Recipient;
                    AppUser Sender = Senders.Where(x => x.Id.ToString() == SenderIdValue).FirstOrDefault();
                    Notification.SenderId = Sender == null ? 0 : Sender.Id;
                    Notification.Sender = Sender;
                    
                    Notifications.Add(Notification);
                }
            }
            Notifications = await NotificationService.Import(Notifications);
            if (Notifications.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Notifications.Count; i++)
                {
                    Notification Notification = Notifications[i];
                    if (!Notification.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Notification.Errors.ContainsKey(nameof(Notification.Id)))
                            Error += Notification.Errors[nameof(Notification.Id)];
                        if (Notification.Errors.ContainsKey(nameof(Notification.TitleWeb)))
                            Error += Notification.Errors[nameof(Notification.TitleWeb)];
                        if (Notification.Errors.ContainsKey(nameof(Notification.ContentWeb)))
                            Error += Notification.Errors[nameof(Notification.ContentWeb)];
                        if (Notification.Errors.ContainsKey(nameof(Notification.SenderId)))
                            Error += Notification.Errors[nameof(Notification.SenderId)];
                        if (Notification.Errors.ContainsKey(nameof(Notification.RecipientId)))
                            Error += Notification.Errors[nameof(Notification.RecipientId)];
                        if (Notification.Errors.ContainsKey(nameof(Notification.Unread)))
                            Error += Notification.Errors[nameof(Notification.Unread)];
                        if (Notification.Errors.ContainsKey(nameof(Notification.Time)))
                            Error += Notification.Errors[nameof(Notification.Time)];
                        if (Notification.Errors.ContainsKey(nameof(Notification.LinkWebsite)))
                            Error += Notification.Errors[nameof(Notification.LinkWebsite)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(NotificationRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Notification_NotificationFilterDTO Notification_NotificationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Notification
                var NotificationFilter = ConvertFilterDTOToFilterEntity(Notification_NotificationFilterDTO);
                NotificationFilter.Skip = 0;
                NotificationFilter.Take = int.MaxValue;
                NotificationFilter = await NotificationService.ToFilter(NotificationFilter);
                List<Notification> Notifications = await NotificationService.List(NotificationFilter);

                var NotificationHeaders = new List<string>()
                {
                    "Id",
                    "TitleWeb",
                    "ContentWeb",
                    "SenderId",
                    "RecipientId",
                    "Unread",
                    "Time",
                    "LinkWebsite",
                };
                List<object[]> NotificationData = new List<object[]>();
                for (int i = 0; i < Notifications.Count; i++)
                {
                    var Notification = Notifications[i];
                    NotificationData.Add(new Object[]
                    {
                        Notification.Id,
                        Notification.TitleWeb,
                        Notification.ContentWeb,
                        Notification.SenderId,
                        Notification.RecipientId,
                        Notification.Unread,
                        Notification.Time,
                        Notification.LinkWebsite,
                    });
                }
                excel.GenerateWorksheet("Notification", NotificationHeaders, NotificationData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "Notification.xlsx");
        }

        [Route(NotificationRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Notification_NotificationFilterDTO Notification_NotificationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Notification_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Notification.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            NotificationFilter NotificationFilter = new NotificationFilter();
            NotificationFilter = await NotificationService.ToFilter(NotificationFilter);
            if (Id == 0)
            {

            }
            else
            {
                NotificationFilter.Id = new IdFilter { Equal = Id };
                int count = await NotificationService.Count(NotificationFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Notification ConvertDTOToEntity(Notification_NotificationDTO Notification_NotificationDTO)
        {
            Notification_NotificationDTO.TrimString();
            Notification Notification = new Notification();
            Notification.Id = Notification_NotificationDTO.Id;
            Notification.TitleWeb = Notification_NotificationDTO.TitleWeb;
            Notification.ContentWeb = Notification_NotificationDTO.ContentWeb;
            Notification.SenderId = Notification_NotificationDTO.SenderId;
            Notification.RecipientId = Notification_NotificationDTO.RecipientId;
            Notification.Unread = Notification_NotificationDTO.Unread;
            Notification.Time = Notification_NotificationDTO.Time;
            Notification.LinkWebsite = Notification_NotificationDTO.LinkWebsite;
            Notification.Recipient = Notification_NotificationDTO.Recipient == null ? null : new AppUser
            {
                Id = Notification_NotificationDTO.Recipient.Id,
                Username = Notification_NotificationDTO.Recipient.Username,
                Email = Notification_NotificationDTO.Recipient.Email,
                Phone = Notification_NotificationDTO.Recipient.Phone,
                Password = Notification_NotificationDTO.Recipient.Password,
                DisplayName = Notification_NotificationDTO.Recipient.DisplayName,
                SexId = Notification_NotificationDTO.Recipient.SexId,
                Birthday = Notification_NotificationDTO.Recipient.Birthday,
                Avatar = Notification_NotificationDTO.Recipient.Avatar,
                CoverImage = Notification_NotificationDTO.Recipient.CoverImage,
            };
            Notification.Sender = Notification_NotificationDTO.Sender == null ? null : new AppUser
            {
                Id = Notification_NotificationDTO.Sender.Id,
                Username = Notification_NotificationDTO.Sender.Username,
                Email = Notification_NotificationDTO.Sender.Email,
                Phone = Notification_NotificationDTO.Sender.Phone,
                Password = Notification_NotificationDTO.Sender.Password,
                DisplayName = Notification_NotificationDTO.Sender.DisplayName,
                SexId = Notification_NotificationDTO.Sender.SexId,
                Birthday = Notification_NotificationDTO.Sender.Birthday,
                Avatar = Notification_NotificationDTO.Sender.Avatar,
                CoverImage = Notification_NotificationDTO.Sender.CoverImage,
            };
            Notification.BaseLanguage = CurrentContext.Language;
            return Notification;
        }

        private NotificationFilter ConvertFilterDTOToFilterEntity(Notification_NotificationFilterDTO Notification_NotificationFilterDTO)
        {
            NotificationFilter NotificationFilter = new NotificationFilter();
            NotificationFilter.Selects = NotificationSelect.ALL;
            NotificationFilter.Skip = Notification_NotificationFilterDTO.Skip;
            NotificationFilter.Take = Notification_NotificationFilterDTO.Take;
            NotificationFilter.OrderBy = Notification_NotificationFilterDTO.OrderBy;
            NotificationFilter.OrderType = Notification_NotificationFilterDTO.OrderType;

            NotificationFilter.Id = Notification_NotificationFilterDTO.Id;
            NotificationFilter.TitleWeb = Notification_NotificationFilterDTO.TitleWeb;
            NotificationFilter.ContentWeb = Notification_NotificationFilterDTO.ContentWeb;
            NotificationFilter.SenderId = Notification_NotificationFilterDTO.SenderId;
            NotificationFilter.RecipientId = Notification_NotificationFilterDTO.RecipientId;
            NotificationFilter.Time = Notification_NotificationFilterDTO.Time;
            NotificationFilter.LinkWebsite = Notification_NotificationFilterDTO.LinkWebsite;
            return NotificationFilter;
        }
    }
}

