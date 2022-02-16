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
using TrueCareer.Services.MMentorReview;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.mentor_review
{
    public partial class MentorReviewController : RpcController
    {
        private IAppUserService AppUserService;
        private IMentorReviewService MentorReviewService;
        private ICurrentContext CurrentContext;
        public MentorReviewController(
            IAppUserService AppUserService,
            IMentorReviewService MentorReviewService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.MentorReviewService = MentorReviewService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MentorReviewRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] MentorReview_MentorReviewFilterDTO MentorReview_MentorReviewFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorReviewFilter MentorReviewFilter = ConvertFilterDTOToFilterEntity(MentorReview_MentorReviewFilterDTO);
            MentorReviewFilter = await MentorReviewService.ToFilter(MentorReviewFilter);
            int count = await MentorReviewService.Count(MentorReviewFilter);
            return count;
        }

        [Route(MentorReviewRoute.List), HttpPost]
        public async Task<ActionResult<List<MentorReview_MentorReviewDTO>>> List([FromBody] MentorReview_MentorReviewFilterDTO MentorReview_MentorReviewFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorReviewFilter MentorReviewFilter = ConvertFilterDTOToFilterEntity(MentorReview_MentorReviewFilterDTO);
            MentorReviewFilter = await MentorReviewService.ToFilter(MentorReviewFilter);
            List<MentorReview> MentorReviews = await MentorReviewService.List(MentorReviewFilter);
            List<MentorReview_MentorReviewDTO> MentorReview_MentorReviewDTOs = MentorReviews
                .Select(c => new MentorReview_MentorReviewDTO(c)).ToList();
            return MentorReview_MentorReviewDTOs;
        }

        [Route(MentorReviewRoute.Get), HttpPost]
        public async Task<ActionResult<MentorReview_MentorReviewDTO>> Get([FromBody]MentorReview_MentorReviewDTO MentorReview_MentorReviewDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorReview_MentorReviewDTO.Id))
                return Forbid();

            MentorReview MentorReview = await MentorReviewService.Get(MentorReview_MentorReviewDTO.Id);
            return new MentorReview_MentorReviewDTO(MentorReview);
        }

        [Route(MentorReviewRoute.Create), HttpPost]
        public async Task<ActionResult<MentorReview_MentorReviewDTO>> Create([FromBody] MentorReview_MentorReviewDTO MentorReview_MentorReviewDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(MentorReview_MentorReviewDTO.Id))
                return Forbid();

            MentorReview MentorReview = ConvertDTOToEntity(MentorReview_MentorReviewDTO);
            MentorReview = await MentorReviewService.Create(MentorReview);
            MentorReview_MentorReviewDTO = new MentorReview_MentorReviewDTO(MentorReview);
            if (MentorReview.IsValidated)
                return MentorReview_MentorReviewDTO;
            else
                return BadRequest(MentorReview_MentorReviewDTO);
        }

        [Route(MentorReviewRoute.Update), HttpPost]
        public async Task<ActionResult<MentorReview_MentorReviewDTO>> Update([FromBody] MentorReview_MentorReviewDTO MentorReview_MentorReviewDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(MentorReview_MentorReviewDTO.Id))
                return Forbid();

            MentorReview MentorReview = ConvertDTOToEntity(MentorReview_MentorReviewDTO);
            MentorReview = await MentorReviewService.Update(MentorReview);
            MentorReview_MentorReviewDTO = new MentorReview_MentorReviewDTO(MentorReview);
            if (MentorReview.IsValidated)
                return MentorReview_MentorReviewDTO;
            else
                return BadRequest(MentorReview_MentorReviewDTO);
        }

        [Route(MentorReviewRoute.Delete), HttpPost]
        public async Task<ActionResult<MentorReview_MentorReviewDTO>> Delete([FromBody] MentorReview_MentorReviewDTO MentorReview_MentorReviewDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorReview_MentorReviewDTO.Id))
                return Forbid();

            MentorReview MentorReview = ConvertDTOToEntity(MentorReview_MentorReviewDTO);
            MentorReview = await MentorReviewService.Delete(MentorReview);
            MentorReview_MentorReviewDTO = new MentorReview_MentorReviewDTO(MentorReview);
            if (MentorReview.IsValidated)
                return MentorReview_MentorReviewDTO;
            else
                return BadRequest(MentorReview_MentorReviewDTO);
        }
        
        [Route(MentorReviewRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorReviewFilter MentorReviewFilter = new MentorReviewFilter();
            MentorReviewFilter = await MentorReviewService.ToFilter(MentorReviewFilter);
            MentorReviewFilter.Id = new IdFilter { In = Ids };
            MentorReviewFilter.Selects = MentorReviewSelect.Id;
            MentorReviewFilter.Skip = 0;
            MentorReviewFilter.Take = int.MaxValue;

            List<MentorReview> MentorReviews = await MentorReviewService.List(MentorReviewFilter);
            MentorReviews = await MentorReviewService.BulkDelete(MentorReviews);
            if (MentorReviews.Any(x => !x.IsValidated))
                return BadRequest(MentorReviews.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(MentorReviewRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter CreatorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Creators = await AppUserService.List(CreatorFilter);
            AppUserFilter MentorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Mentors = await AppUserService.List(MentorFilter);
            List<MentorReview> MentorReviews = new List<MentorReview>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(MentorReviews);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int DescriptionColumn = 1 + StartColumn;
                int ContentReviewColumn = 2 + StartColumn;
                int StarColumn = 3 + StartColumn;
                int MentorIdColumn = 4 + StartColumn;
                int CreatorIdColumn = 5 + StartColumn;
                int TimeColumn = 6 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i, DescriptionColumn].Value?.ToString();
                    string ContentReviewValue = worksheet.Cells[i, ContentReviewColumn].Value?.ToString();
                    string StarValue = worksheet.Cells[i, StarColumn].Value?.ToString();
                    string MentorIdValue = worksheet.Cells[i, MentorIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i, CreatorIdColumn].Value?.ToString();
                    string TimeValue = worksheet.Cells[i, TimeColumn].Value?.ToString();
                    
                    MentorReview MentorReview = new MentorReview();
                    MentorReview.Description = DescriptionValue;
                    MentorReview.ContentReview = ContentReviewValue;
                    MentorReview.Star = long.TryParse(StarValue, out long Star) ? Star : 0;
                    MentorReview.Time = DateTime.TryParse(TimeValue, out DateTime Time) ? Time : DateTime.Now;
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    MentorReview.CreatorId = Creator == null ? 0 : Creator.Id;
                    MentorReview.Creator = Creator;
                    AppUser Mentor = Mentors.Where(x => x.Id.ToString() == MentorIdValue).FirstOrDefault();
                    MentorReview.MentorId = Mentor == null ? 0 : Mentor.Id;
                    MentorReview.Mentor = Mentor;
                    
                    MentorReviews.Add(MentorReview);
                }
            }
            MentorReviews = await MentorReviewService.Import(MentorReviews);
            if (MentorReviews.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < MentorReviews.Count; i++)
                {
                    MentorReview MentorReview = MentorReviews[i];
                    if (!MentorReview.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (MentorReview.Errors.ContainsKey(nameof(MentorReview.Id)))
                            Error += MentorReview.Errors[nameof(MentorReview.Id)];
                        if (MentorReview.Errors.ContainsKey(nameof(MentorReview.Description)))
                            Error += MentorReview.Errors[nameof(MentorReview.Description)];
                        if (MentorReview.Errors.ContainsKey(nameof(MentorReview.ContentReview)))
                            Error += MentorReview.Errors[nameof(MentorReview.ContentReview)];
                        if (MentorReview.Errors.ContainsKey(nameof(MentorReview.Star)))
                            Error += MentorReview.Errors[nameof(MentorReview.Star)];
                        if (MentorReview.Errors.ContainsKey(nameof(MentorReview.MentorId)))
                            Error += MentorReview.Errors[nameof(MentorReview.MentorId)];
                        if (MentorReview.Errors.ContainsKey(nameof(MentorReview.CreatorId)))
                            Error += MentorReview.Errors[nameof(MentorReview.CreatorId)];
                        if (MentorReview.Errors.ContainsKey(nameof(MentorReview.Time)))
                            Error += MentorReview.Errors[nameof(MentorReview.Time)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(MentorReviewRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] MentorReview_MentorReviewFilterDTO MentorReview_MentorReviewFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region MentorReview
                var MentorReviewFilter = ConvertFilterDTOToFilterEntity(MentorReview_MentorReviewFilterDTO);
                MentorReviewFilter.Skip = 0;
                MentorReviewFilter.Take = int.MaxValue;
                MentorReviewFilter = await MentorReviewService.ToFilter(MentorReviewFilter);
                List<MentorReview> MentorReviews = await MentorReviewService.List(MentorReviewFilter);

                var MentorReviewHeaders = new List<string>()
                {
                    "Id",
                    "Description",
                    "ContentReview",
                    "Star",
                    "MentorId",
                    "CreatorId",
                    "Time",
                };
                List<object[]> MentorReviewData = new List<object[]>();
                for (int i = 0; i < MentorReviews.Count; i++)
                {
                    var MentorReview = MentorReviews[i];
                    MentorReviewData.Add(new Object[]
                    {
                        MentorReview.Id,
                        MentorReview.Description,
                        MentorReview.ContentReview,
                        MentorReview.Star,
                        MentorReview.MentorId,
                        MentorReview.CreatorId,
                        MentorReview.Time,
                    });
                }
                excel.GenerateWorksheet("MentorReview", MentorReviewHeaders, MentorReviewData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "MentorReview.xlsx");
        }

        [Route(MentorReviewRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] MentorReview_MentorReviewFilterDTO MentorReview_MentorReviewFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/MentorReview_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "MentorReview.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            MentorReviewFilter MentorReviewFilter = new MentorReviewFilter();
            MentorReviewFilter = await MentorReviewService.ToFilter(MentorReviewFilter);
            if (Id == 0)
            {

            }
            else
            {
                MentorReviewFilter.Id = new IdFilter { Equal = Id };
                int count = await MentorReviewService.Count(MentorReviewFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private MentorReview ConvertDTOToEntity(MentorReview_MentorReviewDTO MentorReview_MentorReviewDTO)
        {
            MentorReview_MentorReviewDTO.TrimString();
            MentorReview MentorReview = new MentorReview();
            MentorReview.Id = MentorReview_MentorReviewDTO.Id;
            MentorReview.Description = MentorReview_MentorReviewDTO.Description;
            MentorReview.ContentReview = MentorReview_MentorReviewDTO.ContentReview;
            MentorReview.Star = MentorReview_MentorReviewDTO.Star;
            MentorReview.MentorId = MentorReview_MentorReviewDTO.MentorId;
            MentorReview.CreatorId = MentorReview_MentorReviewDTO.CreatorId;
            MentorReview.Time = MentorReview_MentorReviewDTO.Time;
            MentorReview.Creator = MentorReview_MentorReviewDTO.Creator == null ? null : new AppUser
            {
                Id = MentorReview_MentorReviewDTO.Creator.Id,
                Username = MentorReview_MentorReviewDTO.Creator.Username,
                Email = MentorReview_MentorReviewDTO.Creator.Email,
                Phone = MentorReview_MentorReviewDTO.Creator.Phone,
                Password = MentorReview_MentorReviewDTO.Creator.Password,
                DisplayName = MentorReview_MentorReviewDTO.Creator.DisplayName,
                SexId = MentorReview_MentorReviewDTO.Creator.SexId,
                Birthday = MentorReview_MentorReviewDTO.Creator.Birthday,
                Avatar = MentorReview_MentorReviewDTO.Creator.Avatar,
                CoverImage = MentorReview_MentorReviewDTO.Creator.CoverImage,
            };
            MentorReview.Mentor = MentorReview_MentorReviewDTO.Mentor == null ? null : new AppUser
            {
                Id = MentorReview_MentorReviewDTO.Mentor.Id,
                Username = MentorReview_MentorReviewDTO.Mentor.Username,
                Email = MentorReview_MentorReviewDTO.Mentor.Email,
                Phone = MentorReview_MentorReviewDTO.Mentor.Phone,
                Password = MentorReview_MentorReviewDTO.Mentor.Password,
                DisplayName = MentorReview_MentorReviewDTO.Mentor.DisplayName,
                SexId = MentorReview_MentorReviewDTO.Mentor.SexId,
                Birthday = MentorReview_MentorReviewDTO.Mentor.Birthday,
                Avatar = MentorReview_MentorReviewDTO.Mentor.Avatar,
                CoverImage = MentorReview_MentorReviewDTO.Mentor.CoverImage,
            };
            MentorReview.BaseLanguage = CurrentContext.Language;
            return MentorReview;
        }

        private MentorReviewFilter ConvertFilterDTOToFilterEntity(MentorReview_MentorReviewFilterDTO MentorReview_MentorReviewFilterDTO)
        {
            MentorReviewFilter MentorReviewFilter = new MentorReviewFilter();
            MentorReviewFilter.Selects = MentorReviewSelect.ALL;
            MentorReviewFilter.Skip = MentorReview_MentorReviewFilterDTO.Skip;
            MentorReviewFilter.Take = MentorReview_MentorReviewFilterDTO.Take;
            MentorReviewFilter.OrderBy = MentorReview_MentorReviewFilterDTO.OrderBy;
            MentorReviewFilter.OrderType = MentorReview_MentorReviewFilterDTO.OrderType;

            MentorReviewFilter.Id = MentorReview_MentorReviewFilterDTO.Id;
            MentorReviewFilter.Description = MentorReview_MentorReviewFilterDTO.Description;
            MentorReviewFilter.ContentReview = MentorReview_MentorReviewFilterDTO.ContentReview;
            MentorReviewFilter.Star = MentorReview_MentorReviewFilterDTO.Star;
            MentorReviewFilter.MentorId = MentorReview_MentorReviewFilterDTO.MentorId;
            MentorReviewFilter.CreatorId = MentorReview_MentorReviewFilterDTO.CreatorId;
            MentorReviewFilter.Time = MentorReview_MentorReviewFilterDTO.Time;
            return MentorReviewFilter;
        }
    }
}

