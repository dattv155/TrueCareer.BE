using TrueSight.Common;
using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using TrueCareer.Repositories;
using TrueCareer.Entities;
using TrueCareer.Enums;

namespace TrueCareer.Services.MFavouriteNews
{
    public interface IFavouriteNewsService :  IServiceScoped
    {
        Task<int> Count(FavouriteNewsFilter FavouriteNewsFilter);
        Task<List<FavouriteNews>> List(FavouriteNewsFilter FavouriteNewsFilter);
        Task<FavouriteNews> Get(long Id);
        Task<FavouriteNews> Create(FavouriteNews FavouriteNews);
        Task<FavouriteNews> Update(FavouriteNews FavouriteNews);
        Task<FavouriteNews> Delete(FavouriteNews FavouriteNews);
        Task<List<FavouriteNews>> BulkDelete(List<FavouriteNews> FavouriteNews);
        Task<List<FavouriteNews>> Import(List<FavouriteNews> FavouriteNews);
        Task<FavouriteNewsFilter> ToFilter(FavouriteNewsFilter FavouriteNewsFilter);
    }

    public class FavouriteNewsService : BaseService, IFavouriteNewsService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IFavouriteNewsValidator FavouriteNewsValidator;

        public FavouriteNewsService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IFavouriteNewsValidator FavouriteNewsValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.FavouriteNewsValidator = FavouriteNewsValidator;
        }
        public async Task<int> Count(FavouriteNewsFilter FavouriteNewsFilter)
        {
            try
            {
                int result = await UOW.FavouriteNewsRepository.Count(FavouriteNewsFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteNewsService));
            }
            return 0;
        }

        public async Task<List<FavouriteNews>> List(FavouriteNewsFilter FavouriteNewsFilter)
        {
            try
            {
                List<FavouriteNews> FavouriteNews = await UOW.FavouriteNewsRepository.List(FavouriteNewsFilter);
                return FavouriteNews;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteNewsService));
            }
            return null;
        }

        public async Task<FavouriteNews> Get(long Id)
        {
            FavouriteNews FavouriteNews = await UOW.FavouriteNewsRepository.Get(Id);
            await FavouriteNewsValidator.Get(FavouriteNews);
            if (FavouriteNews == null)
                return null;
            return FavouriteNews;
        }
        
        public async Task<FavouriteNews> Create(FavouriteNews FavouriteNews)
        {
            if (!await FavouriteNewsValidator.Create(FavouriteNews))
                return FavouriteNews;

            try
            {
                await UOW.FavouriteNewsRepository.Create(FavouriteNews);
                FavouriteNews = await UOW.FavouriteNewsRepository.Get(FavouriteNews.Id);
                Logging.CreateAuditLog(FavouriteNews, new { }, nameof(FavouriteNewsService));
                return FavouriteNews;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteNewsService));
            }
            return null;
        }

        public async Task<FavouriteNews> Update(FavouriteNews FavouriteNews)
        {
            if (!await FavouriteNewsValidator.Update(FavouriteNews))
                return FavouriteNews;
            try
            {
                var oldData = await UOW.FavouriteNewsRepository.Get(FavouriteNews.Id);

                await UOW.FavouriteNewsRepository.Update(FavouriteNews);

                FavouriteNews = await UOW.FavouriteNewsRepository.Get(FavouriteNews.Id);
                Logging.CreateAuditLog(FavouriteNews, oldData, nameof(FavouriteNewsService));
                return FavouriteNews;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteNewsService));
            }
            return null;
        }

        public async Task<FavouriteNews> Delete(FavouriteNews FavouriteNews)
        {
            if (!await FavouriteNewsValidator.Delete(FavouriteNews))
                return FavouriteNews;

            try
            {
                await UOW.FavouriteNewsRepository.Delete(FavouriteNews);
                Logging.CreateAuditLog(new { }, FavouriteNews, nameof(FavouriteNewsService));
                return FavouriteNews;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteNewsService));
            }
            return null;
        }

        public async Task<List<FavouriteNews>> BulkDelete(List<FavouriteNews> FavouriteNews)
        {
            if (!await FavouriteNewsValidator.BulkDelete(FavouriteNews))
                return FavouriteNews;

            try
            {
                await UOW.FavouriteNewsRepository.BulkDelete(FavouriteNews);
                Logging.CreateAuditLog(new { }, FavouriteNews, nameof(FavouriteNewsService));
                return FavouriteNews;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteNewsService));
            }
            return null;

        }
        
        public async Task<List<FavouriteNews>> Import(List<FavouriteNews> FavouriteNews)
        {
            if (!await FavouriteNewsValidator.Import(FavouriteNews))
                return FavouriteNews;
            try
            {
                await UOW.FavouriteNewsRepository.BulkMerge(FavouriteNews);

                Logging.CreateAuditLog(FavouriteNews, new { }, nameof(FavouriteNewsService));
                return FavouriteNews;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteNewsService));
            }
            return null;
        }     
        
        public async Task<FavouriteNewsFilter> ToFilter(FavouriteNewsFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<FavouriteNewsFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                FavouriteNewsFilter subFilter = new FavouriteNewsFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UserId))
                        subFilter.UserId = FilterBuilder.Merge(subFilter.UserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.NewsId))
                        subFilter.NewsId = FilterBuilder.Merge(subFilter.NewsId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private void Sync(List<FavouriteNews> FavouriteNews)
        {
            List<News> News = new List<News>();
            List<AppUser> AppUsers = new List<AppUser>();
            News.AddRange(FavouriteNews.Select(x => new News { Id = x.NewsId }));
            AppUsers.AddRange(FavouriteNews.Select(x => new AppUser { Id = x.UserId }));
            
            News = News.Distinct().ToList();
            AppUsers = AppUsers.Distinct().ToList();
            RabbitManager.PublishList(News, RoutingKeyEnum.NewsUsed.Code);
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
        }

    }
}
