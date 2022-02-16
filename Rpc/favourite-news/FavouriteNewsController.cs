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
using TrueCareer.Services.MFavouriteNews;
using TrueCareer.Services.MNews;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.favourite_news
{
    public partial class FavouriteNewsController : RpcController
    {
        private INewsService NewsService;
        private IAppUserService AppUserService;
        private IFavouriteNewsService FavouriteNewsService;
        private ICurrentContext CurrentContext;
        public FavouriteNewsController(
            INewsService NewsService,
            IAppUserService AppUserService,
            IFavouriteNewsService FavouriteNewsService,
            ICurrentContext CurrentContext
        )
        {
            this.NewsService = NewsService;
            this.AppUserService = AppUserService;
            this.FavouriteNewsService = FavouriteNewsService;
            this.CurrentContext = CurrentContext;
        }

        [Route(FavouriteNewsRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] FavouriteNews_FavouriteNewsFilterDTO FavouriteNews_FavouriteNewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FavouriteNewsFilter FavouriteNewsFilter = ConvertFilterDTOToFilterEntity(FavouriteNews_FavouriteNewsFilterDTO);
            FavouriteNewsFilter = await FavouriteNewsService.ToFilter(FavouriteNewsFilter);
            int count = await FavouriteNewsService.Count(FavouriteNewsFilter);
            return count;
        }

        [Route(FavouriteNewsRoute.List), HttpPost]
        public async Task<ActionResult<List<FavouriteNews_FavouriteNewsDTO>>> List([FromBody] FavouriteNews_FavouriteNewsFilterDTO FavouriteNews_FavouriteNewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FavouriteNewsFilter FavouriteNewsFilter = ConvertFilterDTOToFilterEntity(FavouriteNews_FavouriteNewsFilterDTO);
            FavouriteNewsFilter = await FavouriteNewsService.ToFilter(FavouriteNewsFilter);
            List<FavouriteNews> FavouriteNews = await FavouriteNewsService.List(FavouriteNewsFilter);
            List<FavouriteNews_FavouriteNewsDTO> FavouriteNews_FavouriteNewsDTOs = FavouriteNews
                .Select(c => new FavouriteNews_FavouriteNewsDTO(c)).ToList();
            return FavouriteNews_FavouriteNewsDTOs;
        }

        [Route(FavouriteNewsRoute.Get), HttpPost]
        public async Task<ActionResult<FavouriteNews_FavouriteNewsDTO>> Get([FromBody]FavouriteNews_FavouriteNewsDTO FavouriteNews_FavouriteNewsDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(FavouriteNews_FavouriteNewsDTO.Id))
                return Forbid();

            FavouriteNews FavouriteNews = await FavouriteNewsService.Get(FavouriteNews_FavouriteNewsDTO.Id);
            return new FavouriteNews_FavouriteNewsDTO(FavouriteNews);
        }

        [Route(FavouriteNewsRoute.Create), HttpPost]
        public async Task<ActionResult<FavouriteNews_FavouriteNewsDTO>> Create([FromBody] FavouriteNews_FavouriteNewsDTO FavouriteNews_FavouriteNewsDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(FavouriteNews_FavouriteNewsDTO.Id))
                return Forbid();

            FavouriteNews FavouriteNews = ConvertDTOToEntity(FavouriteNews_FavouriteNewsDTO);
            FavouriteNews = await FavouriteNewsService.Create(FavouriteNews);
            FavouriteNews_FavouriteNewsDTO = new FavouriteNews_FavouriteNewsDTO(FavouriteNews);
            if (FavouriteNews.IsValidated)
                return FavouriteNews_FavouriteNewsDTO;
            else
                return BadRequest(FavouriteNews_FavouriteNewsDTO);
        }

        [Route(FavouriteNewsRoute.Update), HttpPost]
        public async Task<ActionResult<FavouriteNews_FavouriteNewsDTO>> Update([FromBody] FavouriteNews_FavouriteNewsDTO FavouriteNews_FavouriteNewsDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(FavouriteNews_FavouriteNewsDTO.Id))
                return Forbid();

            FavouriteNews FavouriteNews = ConvertDTOToEntity(FavouriteNews_FavouriteNewsDTO);
            FavouriteNews = await FavouriteNewsService.Update(FavouriteNews);
            FavouriteNews_FavouriteNewsDTO = new FavouriteNews_FavouriteNewsDTO(FavouriteNews);
            if (FavouriteNews.IsValidated)
                return FavouriteNews_FavouriteNewsDTO;
            else
                return BadRequest(FavouriteNews_FavouriteNewsDTO);
        }

        [Route(FavouriteNewsRoute.Delete), HttpPost]
        public async Task<ActionResult<FavouriteNews_FavouriteNewsDTO>> Delete([FromBody] FavouriteNews_FavouriteNewsDTO FavouriteNews_FavouriteNewsDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(FavouriteNews_FavouriteNewsDTO.Id))
                return Forbid();

            FavouriteNews FavouriteNews = ConvertDTOToEntity(FavouriteNews_FavouriteNewsDTO);
            FavouriteNews = await FavouriteNewsService.Delete(FavouriteNews);
            FavouriteNews_FavouriteNewsDTO = new FavouriteNews_FavouriteNewsDTO(FavouriteNews);
            if (FavouriteNews.IsValidated)
                return FavouriteNews_FavouriteNewsDTO;
            else
                return BadRequest(FavouriteNews_FavouriteNewsDTO);
        }
        
        [Route(FavouriteNewsRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FavouriteNewsFilter FavouriteNewsFilter = new FavouriteNewsFilter();
            FavouriteNewsFilter = await FavouriteNewsService.ToFilter(FavouriteNewsFilter);
            FavouriteNewsFilter.Id = new IdFilter { In = Ids };
            FavouriteNewsFilter.Selects = FavouriteNewsSelect.Id;
            FavouriteNewsFilter.Skip = 0;
            FavouriteNewsFilter.Take = int.MaxValue;

            List<FavouriteNews> FavouriteNews = await FavouriteNewsService.List(FavouriteNewsFilter);
            FavouriteNews = await FavouriteNewsService.BulkDelete(FavouriteNews);
            if (FavouriteNews.Any(x => !x.IsValidated))
                return BadRequest(FavouriteNews.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(FavouriteNewsRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            NewsFilter NewsFilter = new NewsFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = NewsSelect.ALL
            };
            List<News> News = await NewsService.List(NewsFilter);
            AppUserFilter UserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Users = await AppUserService.List(UserFilter);
            List<FavouriteNews> FavouriteNews = new List<FavouriteNews>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(FavouriteNews);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int UserIdColumn = 1 + StartColumn;
                int NewsIdColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string UserIdValue = worksheet.Cells[i, UserIdColumn].Value?.ToString();
                    string NewsIdValue = worksheet.Cells[i, NewsIdColumn].Value?.ToString();
                    
                    FavouriteNews FavouriteNews = new FavouriteNews();
                    News News = News.Where(x => x.Id.ToString() == NewsIdValue).FirstOrDefault();
                    FavouriteNews.NewsId = News == null ? 0 : News.Id;
                    FavouriteNews.News = News;
                    AppUser User = Users.Where(x => x.Id.ToString() == UserIdValue).FirstOrDefault();
                    FavouriteNews.UserId = User == null ? 0 : User.Id;
                    FavouriteNews.User = User;
                    
                    FavouriteNews.Add(FavouriteNews);
                }
            }
            FavouriteNews = await FavouriteNewsService.Import(FavouriteNews);
            if (FavouriteNews.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < FavouriteNews.Count; i++)
                {
                    FavouriteNews FavouriteNews = FavouriteNews[i];
                    if (!FavouriteNews.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (FavouriteNews.Errors.ContainsKey(nameof(FavouriteNews.Id)))
                            Error += FavouriteNews.Errors[nameof(FavouriteNews.Id)];
                        if (FavouriteNews.Errors.ContainsKey(nameof(FavouriteNews.UserId)))
                            Error += FavouriteNews.Errors[nameof(FavouriteNews.UserId)];
                        if (FavouriteNews.Errors.ContainsKey(nameof(FavouriteNews.NewsId)))
                            Error += FavouriteNews.Errors[nameof(FavouriteNews.NewsId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(FavouriteNewsRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] FavouriteNews_FavouriteNewsFilterDTO FavouriteNews_FavouriteNewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region FavouriteNews
                var FavouriteNewsFilter = ConvertFilterDTOToFilterEntity(FavouriteNews_FavouriteNewsFilterDTO);
                FavouriteNewsFilter.Skip = 0;
                FavouriteNewsFilter.Take = int.MaxValue;
                FavouriteNewsFilter = await FavouriteNewsService.ToFilter(FavouriteNewsFilter);
                List<FavouriteNews> FavouriteNews = await FavouriteNewsService.List(FavouriteNewsFilter);

                var FavouriteNewsHeaders = new List<string>()
                {
                    "Id",
                    "UserId",
                    "NewsId",
                };
                List<object[]> FavouriteNewsData = new List<object[]>();
                for (int i = 0; i < FavouriteNews.Count; i++)
                {
                    var FavouriteNews = FavouriteNews[i];
                    FavouriteNewsData.Add(new Object[]
                    {
                        FavouriteNews.Id,
                        FavouriteNews.UserId,
                        FavouriteNews.NewsId,
                    });
                }
                excel.GenerateWorksheet("FavouriteNews", FavouriteNewsHeaders, FavouriteNewsData);
                #endregion
                
                #region News
                var NewsFilter = new NewsFilter();
                NewsFilter.Selects = NewsSelect.ALL;
                NewsFilter.OrderBy = NewsOrder.Id;
                NewsFilter.OrderType = OrderType.ASC;
                NewsFilter.Skip = 0;
                NewsFilter.Take = int.MaxValue;
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "FavouriteNews.xlsx");
        }

        [Route(FavouriteNewsRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] FavouriteNews_FavouriteNewsFilterDTO FavouriteNews_FavouriteNewsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/FavouriteNews_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "FavouriteNews.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            FavouriteNewsFilter FavouriteNewsFilter = new FavouriteNewsFilter();
            FavouriteNewsFilter = await FavouriteNewsService.ToFilter(FavouriteNewsFilter);
            if (Id == 0)
            {

            }
            else
            {
                FavouriteNewsFilter.Id = new IdFilter { Equal = Id };
                int count = await FavouriteNewsService.Count(FavouriteNewsFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private FavouriteNews ConvertDTOToEntity(FavouriteNews_FavouriteNewsDTO FavouriteNews_FavouriteNewsDTO)
        {
            FavouriteNews_FavouriteNewsDTO.TrimString();
            FavouriteNews FavouriteNews = new FavouriteNews();
            FavouriteNews.Id = FavouriteNews_FavouriteNewsDTO.Id;
            FavouriteNews.UserId = FavouriteNews_FavouriteNewsDTO.UserId;
            FavouriteNews.NewsId = FavouriteNews_FavouriteNewsDTO.NewsId;
            FavouriteNews.News = FavouriteNews_FavouriteNewsDTO.News == null ? null : new News
            {
                Id = FavouriteNews_FavouriteNewsDTO.News.Id,
                CreatorId = FavouriteNews_FavouriteNewsDTO.News.CreatorId,
                NewsContent = FavouriteNews_FavouriteNewsDTO.News.NewsContent,
                LikeCounting = FavouriteNews_FavouriteNewsDTO.News.LikeCounting,
                WatchCounting = FavouriteNews_FavouriteNewsDTO.News.WatchCounting,
                NewsStatusId = FavouriteNews_FavouriteNewsDTO.News.NewsStatusId,
            };
            FavouriteNews.User = FavouriteNews_FavouriteNewsDTO.User == null ? null : new AppUser
            {
                Id = FavouriteNews_FavouriteNewsDTO.User.Id,
                Username = FavouriteNews_FavouriteNewsDTO.User.Username,
                Email = FavouriteNews_FavouriteNewsDTO.User.Email,
                Phone = FavouriteNews_FavouriteNewsDTO.User.Phone,
                Password = FavouriteNews_FavouriteNewsDTO.User.Password,
                DisplayName = FavouriteNews_FavouriteNewsDTO.User.DisplayName,
                SexId = FavouriteNews_FavouriteNewsDTO.User.SexId,
                Birthday = FavouriteNews_FavouriteNewsDTO.User.Birthday,
                Avatar = FavouriteNews_FavouriteNewsDTO.User.Avatar,
                CoverImage = FavouriteNews_FavouriteNewsDTO.User.CoverImage,
            };
            FavouriteNews.BaseLanguage = CurrentContext.Language;
            return FavouriteNews;
        }

        private FavouriteNewsFilter ConvertFilterDTOToFilterEntity(FavouriteNews_FavouriteNewsFilterDTO FavouriteNews_FavouriteNewsFilterDTO)
        {
            FavouriteNewsFilter FavouriteNewsFilter = new FavouriteNewsFilter();
            FavouriteNewsFilter.Selects = FavouriteNewsSelect.ALL;
            FavouriteNewsFilter.Skip = FavouriteNews_FavouriteNewsFilterDTO.Skip;
            FavouriteNewsFilter.Take = FavouriteNews_FavouriteNewsFilterDTO.Take;
            FavouriteNewsFilter.OrderBy = FavouriteNews_FavouriteNewsFilterDTO.OrderBy;
            FavouriteNewsFilter.OrderType = FavouriteNews_FavouriteNewsFilterDTO.OrderType;

            FavouriteNewsFilter.Id = FavouriteNews_FavouriteNewsFilterDTO.Id;
            FavouriteNewsFilter.UserId = FavouriteNews_FavouriteNewsFilterDTO.UserId;
            FavouriteNewsFilter.NewsId = FavouriteNews_FavouriteNewsFilterDTO.NewsId;
            return FavouriteNewsFilter;
        }
    }
}

