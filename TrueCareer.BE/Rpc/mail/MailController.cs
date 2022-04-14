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
using TrueCareer.Services.MMail;

namespace TrueCareer.Rpc.mail
{
    public partial class MailController : RpcController
    {
        private IMailService MailService;
        private ICurrentContext CurrentContext;
        public MailController(
            IMailService MailService,
            ICurrentContext CurrentContext
        )
        {
            this.MailService = MailService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MailRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Mail_MailFilterDTO Mail_MailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MailFilter MailFilter = ConvertFilterDTOToFilterEntity(Mail_MailFilterDTO);
            MailFilter = await MailService.ToFilter(MailFilter);
            int count = await MailService.Count(MailFilter);
            return count;
        }

        [Route(MailRoute.List), HttpPost]
        public async Task<ActionResult<List<Mail_MailDTO>>> List([FromBody] Mail_MailFilterDTO Mail_MailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MailFilter MailFilter = ConvertFilterDTOToFilterEntity(Mail_MailFilterDTO);
            MailFilter = await MailService.ToFilter(MailFilter);
            List<Mail> Mails = await MailService.List(MailFilter);
            List<Mail_MailDTO> Mail_MailDTOs = Mails
                .Select(c => new Mail_MailDTO(c)).ToList();
            return Mail_MailDTOs;
        }

        [Route(MailRoute.Get), HttpPost]
        public async Task<ActionResult<Mail_MailDTO>> Get([FromBody]Mail_MailDTO Mail_MailDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Mail_MailDTO.Id))
                return Forbid();

            Mail Mail = await MailService.Get(Mail_MailDTO.Id);
            return new Mail_MailDTO(Mail);
        }

        [Route(MailRoute.Create), HttpPost]
        public async Task<ActionResult<Mail_MailDTO>> Create([FromBody] Mail_MailDTO Mail_MailDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Mail_MailDTO.Id))
                return Forbid();

            Mail Mail = ConvertDTOToEntity(Mail_MailDTO);
            Mail = await MailService.Create(Mail);
            Mail_MailDTO = new Mail_MailDTO(Mail);
            if (Mail.IsValidated)
                return Mail_MailDTO;
            else
                return BadRequest(Mail_MailDTO);
        }

        [Route(MailRoute.Update), HttpPost]
        public async Task<ActionResult<Mail_MailDTO>> Update([FromBody] Mail_MailDTO Mail_MailDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Mail_MailDTO.Id))
                return Forbid();

            Mail Mail = ConvertDTOToEntity(Mail_MailDTO);
            Mail = await MailService.Update(Mail);
            Mail_MailDTO = new Mail_MailDTO(Mail);
            if (Mail.IsValidated)
                return Mail_MailDTO;
            else
                return BadRequest(Mail_MailDTO);
        }

        [Route(MailRoute.Delete), HttpPost]
        public async Task<ActionResult<Mail_MailDTO>> Delete([FromBody] Mail_MailDTO Mail_MailDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Mail_MailDTO.Id))
                return Forbid();

            Mail Mail = ConvertDTOToEntity(Mail_MailDTO);
            Mail = await MailService.Delete(Mail);
            Mail_MailDTO = new Mail_MailDTO(Mail);
            if (Mail.IsValidated)
                return Mail_MailDTO;
            else
                return BadRequest(Mail_MailDTO);
        }
        
        [Route(MailRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MailFilter MailFilter = new MailFilter();
            MailFilter = await MailService.ToFilter(MailFilter);
            MailFilter.Id = new IdFilter { In = Ids };
            MailFilter.Selects = MailSelect.Id;
            MailFilter.Skip = 0;
            MailFilter.Take = int.MaxValue;

            List<Mail> Mails = await MailService.List(MailFilter);
            Mails = await MailService.BulkDelete(Mails);
            if (Mails.Any(x => !x.IsValidated))
                return BadRequest(Mails.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(MailRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<Mail> Mails = new List<Mail>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Mails);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int UsernameColumn = 1 + StartColumn;
                int PasswordColumn = 2 + StartColumn;
                int RecipientsColumn = 3 + StartColumn;
                int BccRecipientsColumn = 4 + StartColumn;
                int CcRecipientsColumn = 5 + StartColumn;
                int SubjectColumn = 6 + StartColumn;
                int BodyColumn = 7 + StartColumn;
                int RetryCountColumn = 8 + StartColumn;
                int ErrorColumn = 9 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string UsernameValue = worksheet.Cells[i, UsernameColumn].Value?.ToString();
                    string PasswordValue = worksheet.Cells[i, PasswordColumn].Value?.ToString();
                    string RecipientsValue = worksheet.Cells[i, RecipientsColumn].Value?.ToString();
                    string BccRecipientsValue = worksheet.Cells[i, BccRecipientsColumn].Value?.ToString();
                    string CcRecipientsValue = worksheet.Cells[i, CcRecipientsColumn].Value?.ToString();
                    string SubjectValue = worksheet.Cells[i, SubjectColumn].Value?.ToString();
                    string BodyValue = worksheet.Cells[i, BodyColumn].Value?.ToString();
                    string RetryCountValue = worksheet.Cells[i, RetryCountColumn].Value?.ToString();
                    string ErrorValue = worksheet.Cells[i, ErrorColumn].Value?.ToString();
                    
                    Mail Mail = new Mail();
                    Mail.Username = UsernameValue;
                    Mail.Password = PasswordValue;
                    Mail.Recipients = RecipientsValue;
                    Mail.BccRecipients = BccRecipientsValue;
                    Mail.CcRecipients = CcRecipientsValue;
                    Mail.Subject = SubjectValue;
                    Mail.Body = BodyValue;
                    Mail.RetryCount = long.TryParse(RetryCountValue, out long RetryCount) ? RetryCount : 0;
                    Mail.Error = ErrorValue;
                    
                    Mails.Add(Mail);
                }
            }
            Mails = await MailService.Import(Mails);
            if (Mails.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Mails.Count; i++)
                {
                    Mail Mail = Mails[i];
                    if (!Mail.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Mail.Errors.ContainsKey(nameof(Mail.Id)))
                            Error += Mail.Errors[nameof(Mail.Id)];
                        if (Mail.Errors.ContainsKey(nameof(Mail.Username)))
                            Error += Mail.Errors[nameof(Mail.Username)];
                        if (Mail.Errors.ContainsKey(nameof(Mail.Password)))
                            Error += Mail.Errors[nameof(Mail.Password)];
                        if (Mail.Errors.ContainsKey(nameof(Mail.Recipients)))
                            Error += Mail.Errors[nameof(Mail.Recipients)];
                        if (Mail.Errors.ContainsKey(nameof(Mail.BccRecipients)))
                            Error += Mail.Errors[nameof(Mail.BccRecipients)];
                        if (Mail.Errors.ContainsKey(nameof(Mail.CcRecipients)))
                            Error += Mail.Errors[nameof(Mail.CcRecipients)];
                        if (Mail.Errors.ContainsKey(nameof(Mail.Subject)))
                            Error += Mail.Errors[nameof(Mail.Subject)];
                        if (Mail.Errors.ContainsKey(nameof(Mail.Body)))
                            Error += Mail.Errors[nameof(Mail.Body)];
                        if (Mail.Errors.ContainsKey(nameof(Mail.RetryCount)))
                            Error += Mail.Errors[nameof(Mail.RetryCount)];
                        if (Mail.Errors.ContainsKey(nameof(Mail.Error)))
                            Error += Mail.Errors[nameof(Mail.Error)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(MailRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Mail_MailFilterDTO Mail_MailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Mail
                var MailFilter = ConvertFilterDTOToFilterEntity(Mail_MailFilterDTO);
                MailFilter.Skip = 0;
                MailFilter.Take = int.MaxValue;
                MailFilter = await MailService.ToFilter(MailFilter);
                List<Mail> Mails = await MailService.List(MailFilter);

                var MailHeaders = new List<string>()
                {
                    "Id",
                    "Username",
                    "Password",
                    "Recipients",
                    "BccRecipients",
                    "CcRecipients",
                    "Subject",
                    "Body",
                    "RetryCount",
                    "Error",
                };
                List<object[]> MailData = new List<object[]>();
                for (int i = 0; i < Mails.Count; i++)
                {
                    var Mail = Mails[i];
                    MailData.Add(new Object[]
                    {
                        Mail.Id,
                        Mail.Username,
                        Mail.Password,
                        Mail.Recipients,
                        Mail.BccRecipients,
                        Mail.CcRecipients,
                        Mail.Subject,
                        Mail.Body,
                        Mail.RetryCount,
                        Mail.Error,
                    });
                }
                excel.GenerateWorksheet("Mail", MailHeaders, MailData);
                #endregion
                
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Mail.xlsx");
        }

        [Route(MailRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Mail_MailFilterDTO Mail_MailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Mail_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Mail.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            MailFilter MailFilter = new MailFilter();
            MailFilter = await MailService.ToFilter(MailFilter);
            if (Id == 0)
            {

            }
            else
            {
                MailFilter.Id = new IdFilter { Equal = Id };
                int count = await MailService.Count(MailFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Mail ConvertDTOToEntity(Mail_MailDTO Mail_MailDTO)
        {
            Mail_MailDTO.TrimString();
            Mail Mail = new Mail();
            Mail.Id = Mail_MailDTO.Id;
            Mail.Username = Mail_MailDTO.Username;
            Mail.Password = Mail_MailDTO.Password;
            Mail.Recipients = Mail_MailDTO.Recipients;
            Mail.BccRecipients = Mail_MailDTO.BccRecipients;
            Mail.CcRecipients = Mail_MailDTO.CcRecipients;
            Mail.Subject = Mail_MailDTO.Subject;
            Mail.Body = Mail_MailDTO.Body;
            Mail.RetryCount = Mail_MailDTO.RetryCount;
            Mail.Error = Mail_MailDTO.Error;
            Mail.BaseLanguage = CurrentContext.Language;
            return Mail;
        }

        private MailFilter ConvertFilterDTOToFilterEntity(Mail_MailFilterDTO Mail_MailFilterDTO)
        {
            MailFilter MailFilter = new MailFilter();
            MailFilter.Selects = MailSelect.ALL;
            MailFilter.Skip = Mail_MailFilterDTO.Skip;
            MailFilter.Take = Mail_MailFilterDTO.Take;
            MailFilter.OrderBy = Mail_MailFilterDTO.OrderBy;
            MailFilter.OrderType = Mail_MailFilterDTO.OrderType;

            MailFilter.Id = Mail_MailFilterDTO.Id;
            MailFilter.Username = Mail_MailFilterDTO.Username;
            MailFilter.Password = Mail_MailFilterDTO.Password;
            MailFilter.Recipients = Mail_MailFilterDTO.Recipients;
            MailFilter.BccRecipients = Mail_MailFilterDTO.BccRecipients;
            MailFilter.CcRecipients = Mail_MailFilterDTO.CcRecipients;
            MailFilter.Subject = Mail_MailFilterDTO.Subject;
            MailFilter.Body = Mail_MailFilterDTO.Body;
            MailFilter.RetryCount = Mail_MailFilterDTO.RetryCount;
            MailFilter.Error = Mail_MailFilterDTO.Error;
            MailFilter.CreatedAt = Mail_MailFilterDTO.CreatedAt;
            MailFilter.UpdatedAt = Mail_MailFilterDTO.UpdatedAt;
            return MailFilter;
        }
    }
}

