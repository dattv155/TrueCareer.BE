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
using TrueCareer.Services.MFavouriteMentor;
using TrueCareer.Services.MFavouriteNews;
using TrueCareer.Services.MNews;
using TrueCareer.Services.MInformation;
using TrueCareer.Services.MInformationType;
using TrueCareer.Services.MTopic;

namespace TrueCareer.Rpc.app_user
{
    public partial class AppUserController : RpcController
    {
        private ISexService SexService;
        private IFavouriteMentorService FavouriteMentorService;
        private IFavouriteNewsService FavouriteNewsService;
        private INewsService NewsService;
        private IInformationService InformationService;
        private IInformationTypeService InformationTypeService;
        private ITopicService TopicService;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        public AppUserController(
            ISexService SexService,
            IFavouriteMentorService FavouriteMentorService,
            IFavouriteNewsService FavouriteNewsService,
            INewsService NewsService,
            IInformationService InformationService,
            IInformationTypeService InformationTypeService,
            ITopicService TopicService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext
        )
        {
            this.SexService = SexService;
            this.FavouriteMentorService = FavouriteMentorService;
            this.FavouriteNewsService = FavouriteNewsService;
            this.NewsService = NewsService;
            this.InformationService = InformationService;
            this.InformationTypeService = InformationTypeService;
            this.TopicService = TopicService;
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
        }

        [Route(AppUserRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = await AppUserService.ToFilter(AppUserFilter);
            int count = await AppUserService.Count(AppUserFilter);
            return count;
        }

        [Route(AppUserRoute.List), HttpPost]
        public async Task<ActionResult<List<AppUser_AppUserDTO>>> List([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = await AppUserService.ToFilter(AppUserFilter);
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<AppUser_AppUserDTO> AppUser_AppUserDTOs = AppUsers
                .Select(c => new AppUser_AppUserDTO(c)).ToList();
            return AppUser_AppUserDTOs;
        }

        [Route(AppUserRoute.Get), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Get([FromBody]AppUser_AppUserDTO AppUser_AppUserDTO)
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
            AppUserFilter = await AppUserService.ToFilter(AppUserFilter);
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
                AppUserFilter = await AppUserService.ToFilter(AppUserFilter);
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
                #region FavouriteMentor
                var FavouriteMentorFilter = new FavouriteMentorFilter();
                FavouriteMentorFilter.Selects = FavouriteMentorSelect.ALL;
                FavouriteMentorFilter.OrderBy = FavouriteMentorOrder.Id;
                FavouriteMentorFilter.OrderType = OrderType.ASC;
                FavouriteMentorFilter.Skip = 0;
                FavouriteMentorFilter.Take = int.MaxValue;
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
                #region FavouriteNews
                var FavouriteNewsFilter = new FavouriteNewsFilter();
                FavouriteNewsFilter.Selects = FavouriteNewsSelect.ALL;
                FavouriteNewsFilter.OrderBy = FavouriteNewsOrder.Id;
                FavouriteNewsFilter.OrderType = OrderType.ASC;
                FavouriteNewsFilter.Skip = 0;
                FavouriteNewsFilter.Take = int.MaxValue;
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
                #region Information
                var InformationFilter = new InformationFilter();
                InformationFilter.Selects = InformationSelect.ALL;
                InformationFilter.OrderBy = InformationOrder.Id;
                InformationFilter.OrderType = OrderType.ASC;
                InformationFilter.Skip = 0;
                InformationFilter.Take = int.MaxValue;
                List<Information> Information = await InformationService.List(InformationFilter);

                var InformationHeaders = new List<string>()
                {
                    "Id",
                    "InformationTypeId",
                    "Name",
                    "Description",
                    "StartAt",
                    "Role",
                    "Image",
                    "TopicId",
                    "UserId",
                    "EndAt",
                };
                List<object[]> InformationData = new List<object[]>();
                for (int i = 0; i < Information.Count; i++)
                {
                    var Information = Information[i];
                    InformationData.Add(new Object[]
                    {
                        Information.Id,
                        Information.InformationTypeId,
                        Information.Name,
                        Information.Description,
                        Information.StartAt,
                        Information.Role,
                        Information.Image,
                        Information.TopicId,
                        Information.UserId,
                        Information.EndAt,
                    });
                }
                excel.GenerateWorksheet("Information", InformationHeaders, InformationData);
                #endregion
                #region InformationType
                var InformationTypeFilter = new InformationTypeFilter();
                InformationTypeFilter.Selects = InformationTypeSelect.ALL;
                InformationTypeFilter.OrderBy = InformationTypeOrder.Id;
                InformationTypeFilter.OrderType = OrderType.ASC;
                InformationTypeFilter.Skip = 0;
                InformationTypeFilter.Take = int.MaxValue;
                List<InformationType> InformationTypes = await InformationTypeService.List(InformationTypeFilter);

                var InformationTypeHeaders = new List<string>()
                {
                    "Id",
                    "Name",
                    "Code",
                };
                List<object[]> InformationTypeData = new List<object[]>();
                for (int i = 0; i < InformationTypes.Count; i++)
                {
                    var InformationType = InformationTypes[i];
                    InformationTypeData.Add(new Object[]
                    {
                        InformationType.Id,
                        InformationType.Name,
                        InformationType.Code,
                    });
                }
                excel.GenerateWorksheet("InformationType", InformationTypeHeaders, InformationTypeData);
                #endregion
                #region Topic
                var TopicFilter = new TopicFilter();
                TopicFilter.Selects = TopicSelect.ALL;
                TopicFilter.OrderBy = TopicOrder.Id;
                TopicFilter.OrderType = OrderType.ASC;
                TopicFilter.Skip = 0;
                TopicFilter.Take = int.MaxValue;
                List<Topic> Topics = await TopicService.List(TopicFilter);

                var TopicHeaders = new List<string>()
                {
                    "Id",
                    "Title",
                    "Description",
                    "Cost",
                };
                List<object[]> TopicData = new List<object[]>();
                for (int i = 0; i < Topics.Count; i++)
                {
                    var Topic = Topics[i];
                    TopicData.Add(new Object[]
                    {
                        Topic.Id,
                        Topic.Title,
                        Topic.Description,
                        Topic.Cost,
                    });
                }
                excel.GenerateWorksheet("Topic", TopicHeaders, TopicData);
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
            AppUserFilter = await AppUserService.ToFilter(AppUserFilter);
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
            AppUser.FavouriteMentorMentors = AppUser_AppUserDTO.FavouriteMentorMentors?
                .Select(x => new FavouriteMentor
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    MentorId = x.MentorId,
                }).ToList();
            AppUser.FavouriteMentorUsers = AppUser_AppUserDTO.FavouriteMentorUsers?
                .Select(x => new FavouriteMentor
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    MentorId = x.MentorId,
                }).ToList();
            AppUser.FavouriteNews = AppUser_AppUserDTO.FavouriteNews?
                .Select(x => new FavouriteNews
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    NewsId = x.NewsId,
                    News = x.News == null ? null : new News
                    {
                        Id = x.News.Id,
                        CreatorId = x.News.CreatorId,
                        NewsContent = x.News.NewsContent,
                        LikeCounting = x.News.LikeCounting,
                        WatchCounting = x.News.WatchCounting,
                        NewsStatusId = x.News.NewsStatusId,
                    },
                }).ToList();
            AppUser.Information = AppUser_AppUserDTO.Information?
                .Select(x => new Information
                {
                    Id = x.Id,
                    InformationTypeId = x.InformationTypeId,
                    Name = x.Name,
                    Description = x.Description,
                    StartAt = x.StartAt,
                    Role = x.Role,
                    Image = x.Image,
                    TopicId = x.TopicId,
                    UserId = x.UserId,
                    EndAt = x.EndAt,
                    InformationType = x.InformationType == null ? null : new InformationType
                    {
                        Id = x.InformationType.Id,
                        Name = x.InformationType.Name,
                        Code = x.InformationType.Code,
                    },
                    Topic = x.Topic == null ? null : new Topic
                    {
                        Id = x.Topic.Id,
                        Title = x.Topic.Title,
                        Description = x.Topic.Description,
                        Cost = x.Topic.Cost,
                    },
                }).ToList();
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

