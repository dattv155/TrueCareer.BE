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
using TrueCareer.Services.MAppUser;
using TrueCareer.Services.MSex;

namespace TrueCareer.Rpc.app_user
{
    public partial class AppUserController : RpcController
    {
        private ISexService SexService;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        public AppUserController(
            ISexService SexService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext
        )
        {
            this.SexService = SexService;
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
        }

        [Route(AppUserRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            int count = await AppUserService.Count(AppUserFilter);
            return count;
        }

        [Route(AppUserRoute.List), HttpPost]
        public async Task<ActionResult<List<AppUser_AppUserDTO>>> List([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<AppUser_AppUserDTO> AppUser_AppUserDTOs = AppUsers
                .Select(c => new AppUser_AppUserDTO(c)).ToList();
            return AppUser_AppUserDTOs;
        }

        [Route(AppUserRoute.Get), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Get([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = await AppUserService.Get(AppUser_AppUserDTO.Id);
            return new AppUser_AppUserDTO(AppUser);
        }
        [Route(AppUserRoute.Create), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Create([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Create(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Update), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Update([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Update(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Delete), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Delete([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Delete(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            AppUserFilter.Id = new IdFilter { In = Ids };
            AppUserFilter.Selects = AppUserSelect.Id;
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            AppUsers = await AppUserService.BulkDelete(AppUsers);
            if (AppUsers.Any(x => !x.IsValidated))
                return BadRequest(AppUsers.Where(x => !x.IsValidated));
            return true;
        }

        [Route(AppUserRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            SexFilter SexFilter = new SexFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SexSelect.ALL
            };
            List<Sex> Sexes = await SexService.List(SexFilter);
            List<AppUser> AppUsers = new List<AppUser>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(AppUsers);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int UsernameColumn = 1 + StartColumn;
                int EmailColumn = 2 + StartColumn;
                int PhoneColumn = 3 + StartColumn;
                int PasswordColumn = 4 + StartColumn;
                int DisplayNameColumn = 5 + StartColumn;
                int SexIdColumn = 6 + StartColumn;
                int BirthdayColumn = 7 + StartColumn;
                int AvatarColumn = 8 + StartColumn;
                int CoverImageColumn = 9 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string UsernameValue = worksheet.Cells[i, UsernameColumn].Value?.ToString();
                    string EmailValue = worksheet.Cells[i, EmailColumn].Value?.ToString();
                    string PhoneValue = worksheet.Cells[i, PhoneColumn].Value?.ToString();
                    string PasswordValue = worksheet.Cells[i, PasswordColumn].Value?.ToString();
                    string DisplayNameValue = worksheet.Cells[i, DisplayNameColumn].Value?.ToString();
                    string SexIdValue = worksheet.Cells[i, SexIdColumn].Value?.ToString();
                    string BirthdayValue = worksheet.Cells[i, BirthdayColumn].Value?.ToString();
                    string AvatarValue = worksheet.Cells[i, AvatarColumn].Value?.ToString();
                    string CoverImageValue = worksheet.Cells[i, CoverImageColumn].Value?.ToString();

                    AppUser AppUser = new AppUser();
                    AppUser.Username = UsernameValue;
                    AppUser.Email = EmailValue;
                    AppUser.Phone = PhoneValue;
                    AppUser.Password = PasswordValue;
                    AppUser.DisplayName = DisplayNameValue;
                    AppUser.Birthday = DateTime.TryParse(BirthdayValue, out DateTime Birthday) ? Birthday : DateTime.Now;
                    AppUser.Avatar = AvatarValue;
                    AppUser.CoverImage = CoverImageValue;
                    Sex Sex = Sexes.Where(x => x.Id.ToString() == SexIdValue).FirstOrDefault();
                    AppUser.SexId = Sex == null ? 0 : Sex.Id;
                    AppUser.Sex = Sex;

                    AppUsers.Add(AppUser);
                }
            }
            AppUsers = await AppUserService.Import(AppUsers);
            if (AppUsers.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    AppUser AppUser = AppUsers[i];
                    if (!AppUser.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Id)))
                            Error += AppUser.Errors[nameof(AppUser.Id)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Username)))
                            Error += AppUser.Errors[nameof(AppUser.Username)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Email)))
                            Error += AppUser.Errors[nameof(AppUser.Email)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Phone)))
                            Error += AppUser.Errors[nameof(AppUser.Phone)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Password)))
                            Error += AppUser.Errors[nameof(AppUser.Password)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.DisplayName)))
                            Error += AppUser.Errors[nameof(AppUser.DisplayName)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.SexId)))
                            Error += AppUser.Errors[nameof(AppUser.SexId)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Birthday)))
                            Error += AppUser.Errors[nameof(AppUser.Birthday)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Avatar)))
                            Error += AppUser.Errors[nameof(AppUser.Avatar)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.CoverImage)))
                            Error += AppUser.Errors[nameof(AppUser.CoverImage)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }

        [Route(AppUserRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region AppUser
                var AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                AppUserFilter = AppUserService.ToFilter(AppUserFilter);
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

                #region Sex
                var SexFilter = new SexFilter();
                SexFilter.Selects = SexSelect.ALL;
                SexFilter.OrderBy = SexOrder.Id;
                SexFilter.OrderType = OrderType.ASC;
                SexFilter.Skip = 0;
                SexFilter.Take = int.MaxValue;
                List<Sex> Sexes = await SexService.List(SexFilter);

                var SexHeaders = new List<string>()
                {
                    "Id",
                    "Code",
                    "Name",
                };
                List<object[]> SexData = new List<object[]>();
                for (int i = 0; i < Sexes.Count; i++)
                {
                    var Sex = Sexes[i];
                    SexData.Add(new Object[]
                    {
                        Sex.Id,
                        Sex.Code,
                        Sex.Name,
                    });
                }
                excel.GenerateWorksheet("Sex", SexHeaders, SexData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "AppUser.xlsx");
        }

        [Route(AppUserRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            string path = "Templates/AppUser_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "AppUser.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            if (Id == 0)
            {

            }
            else
            {
                AppUserFilter.Id = new IdFilter { Equal = Id };
                int count = await AppUserService.Count(AppUserFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private AppUser ConvertDTOToEntity(AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            AppUser_AppUserDTO.TrimString();
            AppUser AppUser = new AppUser();
            AppUser.Id = AppUser_AppUserDTO.Id;
            AppUser.Username = AppUser_AppUserDTO.Username;
            AppUser.Email = AppUser_AppUserDTO.Email;
            AppUser.Phone = AppUser_AppUserDTO.Phone;
            AppUser.Password = AppUser_AppUserDTO.Password;
            AppUser.DisplayName = AppUser_AppUserDTO.DisplayName;
            AppUser.SexId = AppUser_AppUserDTO.SexId;
            AppUser.Birthday = AppUser_AppUserDTO.Birthday;
            AppUser.Avatar = AppUser_AppUserDTO.Avatar;
            AppUser.CoverImage = AppUser_AppUserDTO.CoverImage;
            AppUser.Sex = AppUser_AppUserDTO.Sex == null ? null : new Sex
            {
                Id = AppUser_AppUserDTO.Sex.Id,
                Code = AppUser_AppUserDTO.Sex.Code,
                Name = AppUser_AppUserDTO.Sex.Name,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            return AppUser;
        }

        private AppUserFilter ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Skip = AppUser_AppUserFilterDTO.Skip;
            AppUserFilter.Take = AppUser_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUser_AppUserFilterDTO.OrderBy;
            AppUserFilter.OrderType = AppUser_AppUserFilterDTO.OrderType;

            AppUserFilter.Id = AppUser_AppUserFilterDTO.Id;
            AppUserFilter.Username = AppUser_AppUserFilterDTO.Username;
            AppUserFilter.Email = AppUser_AppUserFilterDTO.Email;
            AppUserFilter.Phone = AppUser_AppUserFilterDTO.Phone;
            AppUserFilter.Password = AppUser_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = AppUser_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = AppUser_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = AppUser_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = AppUser_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = AppUser_AppUserFilterDTO.CoverImage;
            AppUserFilter.CreatedAt = AppUser_AppUserFilterDTO.CreatedAt;
            AppUserFilter.UpdatedAt = AppUser_AppUserFilterDTO.UpdatedAt;
            return AppUserFilter;
        }
    }
}

