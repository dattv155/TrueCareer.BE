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
using TrueCareer.Services.MFavouriteMentor;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.favourite_mentor
{
    public partial class FavouriteMentorController : RpcController
    {
        private IAppUserService AppUserService;
        private IFavouriteMentorService FavouriteMentorService;
        private ICurrentContext CurrentContext;
        public FavouriteMentorController(
            IAppUserService AppUserService,
            IFavouriteMentorService FavouriteMentorService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.FavouriteMentorService = FavouriteMentorService;
            this.CurrentContext = CurrentContext;
        }

        [Route(FavouriteMentorRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] FavouriteMentor_FavouriteMentorFilterDTO FavouriteMentor_FavouriteMentorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FavouriteMentorFilter FavouriteMentorFilter = ConvertFilterDTOToFilterEntity(FavouriteMentor_FavouriteMentorFilterDTO);
            FavouriteMentorFilter = await FavouriteMentorService.ToFilter(FavouriteMentorFilter);
            int count = await FavouriteMentorService.Count(FavouriteMentorFilter);
            return count;
        }

        [Route(FavouriteMentorRoute.List), HttpPost]
        public async Task<ActionResult<List<FavouriteMentor_FavouriteMentorDTO>>> List([FromBody] FavouriteMentor_FavouriteMentorFilterDTO FavouriteMentor_FavouriteMentorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FavouriteMentorFilter FavouriteMentorFilter = ConvertFilterDTOToFilterEntity(FavouriteMentor_FavouriteMentorFilterDTO);
            FavouriteMentorFilter = await FavouriteMentorService.ToFilter(FavouriteMentorFilter);
            List<FavouriteMentor> FavouriteMentors = await FavouriteMentorService.List(FavouriteMentorFilter);
            List<FavouriteMentor_FavouriteMentorDTO> FavouriteMentor_FavouriteMentorDTOs = FavouriteMentors
                .Select(c => new FavouriteMentor_FavouriteMentorDTO(c)).ToList();
            return FavouriteMentor_FavouriteMentorDTOs;
        }

        [Route(FavouriteMentorRoute.Get), HttpPost]
        public async Task<ActionResult<FavouriteMentor_FavouriteMentorDTO>> Get([FromBody]FavouriteMentor_FavouriteMentorDTO FavouriteMentor_FavouriteMentorDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(FavouriteMentor_FavouriteMentorDTO.Id))
                return Forbid();

            FavouriteMentor FavouriteMentor = await FavouriteMentorService.Get(FavouriteMentor_FavouriteMentorDTO.Id);
            return new FavouriteMentor_FavouriteMentorDTO(FavouriteMentor);
        }

        [Route(FavouriteMentorRoute.Create), HttpPost]
        public async Task<ActionResult<FavouriteMentor_FavouriteMentorDTO>> Create([FromBody] FavouriteMentor_FavouriteMentorDTO FavouriteMentor_FavouriteMentorDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(FavouriteMentor_FavouriteMentorDTO.Id))
                return Forbid();

            FavouriteMentor FavouriteMentor = ConvertDTOToEntity(FavouriteMentor_FavouriteMentorDTO);
            FavouriteMentor = await FavouriteMentorService.Create(FavouriteMentor);
            FavouriteMentor_FavouriteMentorDTO = new FavouriteMentor_FavouriteMentorDTO(FavouriteMentor);
            if (FavouriteMentor.IsValidated)
                return FavouriteMentor_FavouriteMentorDTO;
            else
                return BadRequest(FavouriteMentor_FavouriteMentorDTO);
        }

        [Route(FavouriteMentorRoute.Update), HttpPost]
        public async Task<ActionResult<FavouriteMentor_FavouriteMentorDTO>> Update([FromBody] FavouriteMentor_FavouriteMentorDTO FavouriteMentor_FavouriteMentorDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(FavouriteMentor_FavouriteMentorDTO.Id))
                return Forbid();

            FavouriteMentor FavouriteMentor = ConvertDTOToEntity(FavouriteMentor_FavouriteMentorDTO);
            FavouriteMentor = await FavouriteMentorService.Update(FavouriteMentor);
            FavouriteMentor_FavouriteMentorDTO = new FavouriteMentor_FavouriteMentorDTO(FavouriteMentor);
            if (FavouriteMentor.IsValidated)
                return FavouriteMentor_FavouriteMentorDTO;
            else
                return BadRequest(FavouriteMentor_FavouriteMentorDTO);
        }

        [Route(FavouriteMentorRoute.Delete), HttpPost]
        public async Task<ActionResult<FavouriteMentor_FavouriteMentorDTO>> Delete([FromBody] FavouriteMentor_FavouriteMentorDTO FavouriteMentor_FavouriteMentorDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(FavouriteMentor_FavouriteMentorDTO.Id))
                return Forbid();

            FavouriteMentor FavouriteMentor = ConvertDTOToEntity(FavouriteMentor_FavouriteMentorDTO);
            FavouriteMentor = await FavouriteMentorService.Delete(FavouriteMentor);
            FavouriteMentor_FavouriteMentorDTO = new FavouriteMentor_FavouriteMentorDTO(FavouriteMentor);
            if (FavouriteMentor.IsValidated)
                return FavouriteMentor_FavouriteMentorDTO;
            else
                return BadRequest(FavouriteMentor_FavouriteMentorDTO);
        }
        
        [Route(FavouriteMentorRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FavouriteMentorFilter FavouriteMentorFilter = new FavouriteMentorFilter();
            FavouriteMentorFilter = await FavouriteMentorService.ToFilter(FavouriteMentorFilter);
            FavouriteMentorFilter.Id = new IdFilter { In = Ids };
            FavouriteMentorFilter.Selects = FavouriteMentorSelect.Id;
            FavouriteMentorFilter.Skip = 0;
            FavouriteMentorFilter.Take = int.MaxValue;

            List<FavouriteMentor> FavouriteMentors = await FavouriteMentorService.List(FavouriteMentorFilter);
            FavouriteMentors = await FavouriteMentorService.BulkDelete(FavouriteMentors);
            if (FavouriteMentors.Any(x => !x.IsValidated))
                return BadRequest(FavouriteMentors.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(FavouriteMentorRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter MentorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Mentors = await AppUserService.List(MentorFilter);
            AppUserFilter UserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Users = await AppUserService.List(UserFilter);
            List<FavouriteMentor> FavouriteMentors = new List<FavouriteMentor>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(FavouriteMentors);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int UserIdColumn = 1 + StartColumn;
                int MentorIdColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string UserIdValue = worksheet.Cells[i, UserIdColumn].Value?.ToString();
                    string MentorIdValue = worksheet.Cells[i, MentorIdColumn].Value?.ToString();
                    
                    FavouriteMentor FavouriteMentor = new FavouriteMentor();
                    AppUser Mentor = Mentors.Where(x => x.Id.ToString() == MentorIdValue).FirstOrDefault();
                    FavouriteMentor.MentorId = Mentor == null ? 0 : Mentor.Id;
                    FavouriteMentor.Mentor = Mentor;
                    AppUser User = Users.Where(x => x.Id.ToString() == UserIdValue).FirstOrDefault();
                    FavouriteMentor.UserId = User == null ? 0 : User.Id;
                    FavouriteMentor.User = User;
                    
                    FavouriteMentors.Add(FavouriteMentor);
                }
            }
            FavouriteMentors = await FavouriteMentorService.Import(FavouriteMentors);
            if (FavouriteMentors.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < FavouriteMentors.Count; i++)
                {
                    FavouriteMentor FavouriteMentor = FavouriteMentors[i];
                    if (!FavouriteMentor.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (FavouriteMentor.Errors.ContainsKey(nameof(FavouriteMentor.Id)))
                            Error += FavouriteMentor.Errors[nameof(FavouriteMentor.Id)];
                        if (FavouriteMentor.Errors.ContainsKey(nameof(FavouriteMentor.UserId)))
                            Error += FavouriteMentor.Errors[nameof(FavouriteMentor.UserId)];
                        if (FavouriteMentor.Errors.ContainsKey(nameof(FavouriteMentor.MentorId)))
                            Error += FavouriteMentor.Errors[nameof(FavouriteMentor.MentorId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(FavouriteMentorRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] FavouriteMentor_FavouriteMentorFilterDTO FavouriteMentor_FavouriteMentorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region FavouriteMentor
                var FavouriteMentorFilter = ConvertFilterDTOToFilterEntity(FavouriteMentor_FavouriteMentorFilterDTO);
                FavouriteMentorFilter.Skip = 0;
                FavouriteMentorFilter.Take = int.MaxValue;
                FavouriteMentorFilter = await FavouriteMentorService.ToFilter(FavouriteMentorFilter);
                List<FavouriteMentor> FavouriteMentors = await FavouriteMentorService.List(FavouriteMentorFilter);

                var FavouriteMentorHeaders = new List<string>()
                {
                    "Id",
                    "UserId",
                    "MentorId",
                };
                List<object[]> FavouriteMentorData = new List<object[]>();
                for (int i = 0; i < FavouriteMentors.Count; i++)
                {
                    var FavouriteMentor = FavouriteMentors[i];
                    FavouriteMentorData.Add(new Object[]
                    {
                        FavouriteMentor.Id,
                        FavouriteMentor.UserId,
                        FavouriteMentor.MentorId,
                    });
                }
                excel.GenerateWorksheet("FavouriteMentor", FavouriteMentorHeaders, FavouriteMentorData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "FavouriteMentor.xlsx");
        }

        [Route(FavouriteMentorRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] FavouriteMentor_FavouriteMentorFilterDTO FavouriteMentor_FavouriteMentorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/FavouriteMentor_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "FavouriteMentor.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            FavouriteMentorFilter FavouriteMentorFilter = new FavouriteMentorFilter();
            FavouriteMentorFilter = await FavouriteMentorService.ToFilter(FavouriteMentorFilter);
            if (Id == 0)
            {

            }
            else
            {
                FavouriteMentorFilter.Id = new IdFilter { Equal = Id };
                int count = await FavouriteMentorService.Count(FavouriteMentorFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private FavouriteMentor ConvertDTOToEntity(FavouriteMentor_FavouriteMentorDTO FavouriteMentor_FavouriteMentorDTO)
        {
            FavouriteMentor_FavouriteMentorDTO.TrimString();
            FavouriteMentor FavouriteMentor = new FavouriteMentor();
            FavouriteMentor.Id = FavouriteMentor_FavouriteMentorDTO.Id;
            FavouriteMentor.UserId = FavouriteMentor_FavouriteMentorDTO.UserId;
            FavouriteMentor.MentorId = FavouriteMentor_FavouriteMentorDTO.MentorId;
            FavouriteMentor.Mentor = FavouriteMentor_FavouriteMentorDTO.Mentor == null ? null : new AppUser
            {
                Id = FavouriteMentor_FavouriteMentorDTO.Mentor.Id,
                Username = FavouriteMentor_FavouriteMentorDTO.Mentor.Username,
                Email = FavouriteMentor_FavouriteMentorDTO.Mentor.Email,
                Phone = FavouriteMentor_FavouriteMentorDTO.Mentor.Phone,
                Password = FavouriteMentor_FavouriteMentorDTO.Mentor.Password,
                DisplayName = FavouriteMentor_FavouriteMentorDTO.Mentor.DisplayName,
                SexId = FavouriteMentor_FavouriteMentorDTO.Mentor.SexId,
                Birthday = FavouriteMentor_FavouriteMentorDTO.Mentor.Birthday,
                Avatar = FavouriteMentor_FavouriteMentorDTO.Mentor.Avatar,
                CoverImage = FavouriteMentor_FavouriteMentorDTO.Mentor.CoverImage,
            };
            FavouriteMentor.User = FavouriteMentor_FavouriteMentorDTO.User == null ? null : new AppUser
            {
                Id = FavouriteMentor_FavouriteMentorDTO.User.Id,
                Username = FavouriteMentor_FavouriteMentorDTO.User.Username,
                Email = FavouriteMentor_FavouriteMentorDTO.User.Email,
                Phone = FavouriteMentor_FavouriteMentorDTO.User.Phone,
                Password = FavouriteMentor_FavouriteMentorDTO.User.Password,
                DisplayName = FavouriteMentor_FavouriteMentorDTO.User.DisplayName,
                SexId = FavouriteMentor_FavouriteMentorDTO.User.SexId,
                Birthday = FavouriteMentor_FavouriteMentorDTO.User.Birthday,
                Avatar = FavouriteMentor_FavouriteMentorDTO.User.Avatar,
                CoverImage = FavouriteMentor_FavouriteMentorDTO.User.CoverImage,
            };
            FavouriteMentor.BaseLanguage = CurrentContext.Language;
            return FavouriteMentor;
        }

        private FavouriteMentorFilter ConvertFilterDTOToFilterEntity(FavouriteMentor_FavouriteMentorFilterDTO FavouriteMentor_FavouriteMentorFilterDTO)
        {
            FavouriteMentorFilter FavouriteMentorFilter = new FavouriteMentorFilter();
            FavouriteMentorFilter.Selects = FavouriteMentorSelect.ALL;
            FavouriteMentorFilter.Skip = FavouriteMentor_FavouriteMentorFilterDTO.Skip;
            FavouriteMentorFilter.Take = FavouriteMentor_FavouriteMentorFilterDTO.Take;
            FavouriteMentorFilter.OrderBy = FavouriteMentor_FavouriteMentorFilterDTO.OrderBy;
            FavouriteMentorFilter.OrderType = FavouriteMentor_FavouriteMentorFilterDTO.OrderType;

            FavouriteMentorFilter.Id = FavouriteMentor_FavouriteMentorFilterDTO.Id;
            FavouriteMentorFilter.UserId = FavouriteMentor_FavouriteMentorFilterDTO.UserId;
            FavouriteMentorFilter.MentorId = FavouriteMentor_FavouriteMentorFilterDTO.MentorId;
            return FavouriteMentorFilter;
        }
    }
}

