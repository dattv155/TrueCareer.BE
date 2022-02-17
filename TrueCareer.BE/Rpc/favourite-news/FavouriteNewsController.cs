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

