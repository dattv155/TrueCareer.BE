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
using TrueCareer.Services.MNews;
using TrueCareer.Services.MAppUser;
using TrueCareer.Services.MNewsStatus;

namespace TrueCareer.Rpc.news
{
    public partial class NewsController : RpcController
    {
        private IAppUserService AppUserService;
        private INewsStatusService NewsStatusService;
        private INewsService NewsService;
        private ICurrentContext CurrentContext;
        public NewsController(
            IAppUserService AppUserService,
            INewsStatusService NewsStatusService,
            INewsService NewsService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.NewsStatusService = NewsStatusService;
            this.NewsService = NewsService;
            this.CurrentContext = CurrentContext;
        }

        [Route(NewsRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] News_NewsFilterDTO News_NewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NewsFilter NewsFilter = ConvertFilterDTOToFilterEntity(News_NewsFilterDTO);
            NewsFilter = await NewsService.ToFilter(NewsFilter);
            int count = await NewsService.Count(NewsFilter);
            return count;
        }

        [Route(NewsRoute.List), HttpPost]
        public async Task<ActionResult<List<News_NewsDTO>>> List([FromBody] News_NewsFilterDTO News_NewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NewsFilter NewsFilter = ConvertFilterDTOToFilterEntity(News_NewsFilterDTO);
            NewsFilter = await NewsService.ToFilter(NewsFilter);
            List<News> News = await NewsService.List(NewsFilter);
            List<News_NewsDTO> News_NewsDTOs = News
                .Select(c => new News_NewsDTO(c)).ToList();
            return News_NewsDTOs;
        }

        [Route(NewsRoute.Get), HttpPost]
        public async Task<ActionResult<News_NewsDTO>> Get([FromBody]News_NewsDTO News_NewsDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(News_NewsDTO.Id))
                return Forbid();

            News News = await NewsService.Get(News_NewsDTO.Id);
            return new News_NewsDTO(News);
        }

        [Route(NewsRoute.Create), HttpPost]
        public async Task<ActionResult<News_NewsDTO>> Create([FromBody] News_NewsDTO News_NewsDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(News_NewsDTO.Id))
                return Forbid();

            News News = ConvertDTOToEntity(News_NewsDTO);
            News = await NewsService.Create(News);
            News_NewsDTO = new News_NewsDTO(News);
            if (News.IsValidated)
                return News_NewsDTO;
            else
                return BadRequest(News_NewsDTO);
        }

        [Route(NewsRoute.Update), HttpPost]
        public async Task<ActionResult<News_NewsDTO>> Update([FromBody] News_NewsDTO News_NewsDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(News_NewsDTO.Id))
                return Forbid();

            News News = ConvertDTOToEntity(News_NewsDTO);
            News = await NewsService.Update(News);
            News_NewsDTO = new News_NewsDTO(News);
            if (News.IsValidated)
                return News_NewsDTO;
            else
                return BadRequest(News_NewsDTO);
        }

        [Route(NewsRoute.Delete), HttpPost]
        public async Task<ActionResult<News_NewsDTO>> Delete([FromBody] News_NewsDTO News_NewsDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(News_NewsDTO.Id))
                return Forbid();

            News News = ConvertDTOToEntity(News_NewsDTO);
            News = await NewsService.Delete(News);
            News_NewsDTO = new News_NewsDTO(News);
            if (News.IsValidated)
                return News_NewsDTO;
            else
                return BadRequest(News_NewsDTO);
        }
        
        [Route(NewsRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NewsFilter NewsFilter = new NewsFilter();
            NewsFilter = await NewsService.ToFilter(NewsFilter);
            NewsFilter.Id = new IdFilter { In = Ids };
            NewsFilter.Selects = NewsSelect.Id;
            NewsFilter.Skip = 0;
            NewsFilter.Take = int.MaxValue;

            List<News> News = await NewsService.List(NewsFilter);
            News = await NewsService.BulkDelete(News);
            if (News.Any(x => !x.IsValidated))
                return BadRequest(News.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(NewsRoute.Import), HttpPost]
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
            NewsStatusFilter NewsStatusFilter = new NewsStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = NewsStatusSelect.ALL
            };
            List<NewsStatus> NewsStatuses = await NewsStatusService.List(NewsStatusFilter);
            List<News> News = new List<News>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(News);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CreatorIdColumn = 2 + StartColumn;
                int NewsContentColumn = 3 + StartColumn;
                int LikeCountingColumn = 4 + StartColumn;
                int WatchCountingColumn = 5 + StartColumn;
                int NewsStatusIdColumn = 6 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i, CreatorIdColumn].Value?.ToString();
                    string NewsContentValue = worksheet.Cells[i, NewsContentColumn].Value?.ToString();
                    string LikeCountingValue = worksheet.Cells[i, LikeCountingColumn].Value?.ToString();
                    string WatchCountingValue = worksheet.Cells[i, WatchCountingColumn].Value?.ToString();
                    string NewsStatusIdValue = worksheet.Cells[i, NewsStatusIdColumn].Value?.ToString();
                    
                    News News = new News();
                    News.NewsContent = NewsContentValue;
                    News.LikeCounting = long.TryParse(LikeCountingValue, out long LikeCounting) ? LikeCounting : 0;
                    News.WatchCounting = long.TryParse(WatchCountingValue, out long WatchCounting) ? WatchCounting : 0;
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    News.CreatorId = Creator == null ? 0 : Creator.Id;
                    News.Creator = Creator;
                    NewsStatus NewsStatus = NewsStatuses.Where(x => x.Id.ToString() == NewsStatusIdValue).FirstOrDefault();
                    News.NewsStatusId = NewsStatus == null ? 0 : NewsStatus.Id;
                    News.NewsStatus = NewsStatus;
                    
                    News.Add(News);
                }
            }
            News = await NewsService.Import(News);
            if (News.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < News.Count; i++)
                {
                    News News = News[i];
                    if (!News.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (News.Errors.ContainsKey(nameof(News.Id)))
                            Error += News.Errors[nameof(News.Id)];
                        if (News.Errors.ContainsKey(nameof(News.CreatorId)))
                            Error += News.Errors[nameof(News.CreatorId)];
                        if (News.Errors.ContainsKey(nameof(News.NewsContent)))
                            Error += News.Errors[nameof(News.NewsContent)];
                        if (News.Errors.ContainsKey(nameof(News.LikeCounting)))
                            Error += News.Errors[nameof(News.LikeCounting)];
                        if (News.Errors.ContainsKey(nameof(News.WatchCounting)))
                            Error += News.Errors[nameof(News.WatchCounting)];
                        if (News.Errors.ContainsKey(nameof(News.NewsStatusId)))
                            Error += News.Errors[nameof(News.NewsStatusId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(NewsRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] News_NewsFilterDTO News_NewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region News
                var NewsFilter = ConvertFilterDTOToFilterEntity(News_NewsFilterDTO);
                NewsFilter.Skip = 0;
                NewsFilter.Take = int.MaxValue;
                NewsFilter = await NewsService.ToFilter(NewsFilter);
                List<News> News = await NewsService.List(NewsFilter);

                var NewsHeaders = new List<string>()
                {
                    "Id",
                    "CreatorId",
                    "NewsContent",
                    "LikeCounting",
                    "WatchCounting",
                    "NewsStatusId",
                };
                List<object[]> NewsData = new List<object[]>();
                for (int i = 0; i < News.Count; i++)
                {
                    var News = News[i];
                    NewsData.Add(new Object[]
                    {
                        News.Id,
                        News.CreatorId,
                        News.NewsContent,
                        News.LikeCounting,
                        News.WatchCounting,
                        News.NewsStatusId,
                    });
                }
                excel.GenerateWorksheet("News", NewsHeaders, NewsData);
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
                #region NewsStatus
                var NewsStatusFilter = new NewsStatusFilter();
                NewsStatusFilter.Selects = NewsStatusSelect.ALL;
                NewsStatusFilter.OrderBy = NewsStatusOrder.Id;
                NewsStatusFilter.OrderType = OrderType.ASC;
                NewsStatusFilter.Skip = 0;
                NewsStatusFilter.Take = int.MaxValue;
                List<NewsStatus> NewsStatuses = await NewsStatusService.List(NewsStatusFilter);

                var NewsStatusHeaders = new List<string>()
                {
                    "Id",
                    "Code",
                    "Name",
                };
                List<object[]> NewsStatusData = new List<object[]>();
                for (int i = 0; i < NewsStatuses.Count; i++)
                {
                    var NewsStatus = NewsStatuses[i];
                    NewsStatusData.Add(new Object[]
                    {
                        NewsStatus.Id,
                        NewsStatus.Code,
                        NewsStatus.Name,
                    });
                }
                excel.GenerateWorksheet("NewsStatus", NewsStatusHeaders, NewsStatusData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "News.xlsx");
        }

        [Route(NewsRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] News_NewsFilterDTO News_NewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/News_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "News.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            NewsFilter NewsFilter = new NewsFilter();
            NewsFilter = await NewsService.ToFilter(NewsFilter);
            if (Id == 0)
            {

            }
            else
            {
                NewsFilter.Id = new IdFilter { Equal = Id };
                int count = await NewsService.Count(NewsFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private News ConvertDTOToEntity(News_NewsDTO News_NewsDTO)
        {
            News_NewsDTO.TrimString();
            News News = new News();
            News.Id = News_NewsDTO.Id;
            News.CreatorId = News_NewsDTO.CreatorId;
            News.NewsContent = News_NewsDTO.NewsContent;
            News.LikeCounting = News_NewsDTO.LikeCounting;
            News.WatchCounting = News_NewsDTO.WatchCounting;
            News.NewsStatusId = News_NewsDTO.NewsStatusId;
            News.Creator = News_NewsDTO.Creator == null ? null : new AppUser
            {
                Id = News_NewsDTO.Creator.Id,
                Username = News_NewsDTO.Creator.Username,
                Email = News_NewsDTO.Creator.Email,
                Phone = News_NewsDTO.Creator.Phone,
                Password = News_NewsDTO.Creator.Password,
                DisplayName = News_NewsDTO.Creator.DisplayName,
                SexId = News_NewsDTO.Creator.SexId,
                Birthday = News_NewsDTO.Creator.Birthday,
                Avatar = News_NewsDTO.Creator.Avatar,
                CoverImage = News_NewsDTO.Creator.CoverImage,
            };
            News.NewsStatus = News_NewsDTO.NewsStatus == null ? null : new NewsStatus
            {
                Id = News_NewsDTO.NewsStatus.Id,
                Code = News_NewsDTO.NewsStatus.Code,
                Name = News_NewsDTO.NewsStatus.Name,
            };
            News.BaseLanguage = CurrentContext.Language;
            return News;
        }

        private NewsFilter ConvertFilterDTOToFilterEntity(News_NewsFilterDTO News_NewsFilterDTO)
        {
            NewsFilter NewsFilter = new NewsFilter();
            NewsFilter.Selects = NewsSelect.ALL;
            NewsFilter.Skip = News_NewsFilterDTO.Skip;
            NewsFilter.Take = News_NewsFilterDTO.Take;
            NewsFilter.OrderBy = News_NewsFilterDTO.OrderBy;
            NewsFilter.OrderType = News_NewsFilterDTO.OrderType;

            NewsFilter.Id = News_NewsFilterDTO.Id;
            NewsFilter.CreatorId = News_NewsFilterDTO.CreatorId;
            NewsFilter.NewsContent = News_NewsFilterDTO.NewsContent;
            NewsFilter.LikeCounting = News_NewsFilterDTO.LikeCounting;
            NewsFilter.WatchCounting = News_NewsFilterDTO.WatchCounting;
            NewsFilter.NewsStatusId = News_NewsFilterDTO.NewsStatusId;
            NewsFilter.CreatedAt = News_NewsFilterDTO.CreatedAt;
            NewsFilter.UpdatedAt = News_NewsFilterDTO.UpdatedAt;
            return NewsFilter;
        }
    }
}

