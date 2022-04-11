using TrueSight.Common;
using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using OfficeOpenXml;
using TrueCareer.Repositories;
using TrueCareer.Entities;
using TrueCareer.Enums;

namespace TrueCareer.Services.MNews
{
    public interface INewsService :  IServiceScoped
    {
        Task<int> Count(NewsFilter NewsFilter);
        Task<List<News>> List(NewsFilter NewsFilter);
        Task<News> Get(long Id);
        Task<News> Create(News News);
        Task<News> Update(News News);
        Task<News> Delete(News News);
        Task<List<News>> BulkDelete(List<News> News);
        Task<List<News>> Import(List<News> News);
        Task<NewsFilter> ToFilter(NewsFilter NewsFilter);
    }

    public class NewsService : BaseService, INewsService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private INewsValidator NewsValidator;

        public NewsService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            INewsValidator NewsValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.NewsValidator = NewsValidator;
        }
        public async Task<int> Count(NewsFilter NewsFilter)
        {
            try
            {
                int result = await UOW.NewsRepository.Count(NewsFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NewsService));
            }
            return 0;
        }

        public async Task<List<News>> List(NewsFilter NewsFilter)
        {
            try
            {
                List<News> News = await UOW.NewsRepository.List(NewsFilter);
                return News;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NewsService));
            }
            return null;
        }

        public async Task<News> Get(long Id)
        {
            News News = await UOW.NewsRepository.Get(Id);
            await NewsValidator.Get(News);
            if (News == null)
                return null;
            return News;
        }
        
        public async Task<News> Create(News News)
        {
            if (!await NewsValidator.Create(News))
                return News;

            try
            {
                await UOW.NewsRepository.Create(News);
                News = await UOW.NewsRepository.Get(News.Id);
                Logging.CreateAuditLog(News, new { }, nameof(NewsService));
                return News;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NewsService));
            }
            return null;
        }

        public async Task<News> Update(News News)
        {
            if (!await NewsValidator.Update(News))
                return News;
            try
            {
                var oldData = await UOW.NewsRepository.Get(News.Id);

                await UOW.NewsRepository.Update(News);

                News = await UOW.NewsRepository.Get(News.Id);
                Logging.CreateAuditLog(News, oldData, nameof(NewsService));
                return News;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NewsService));
            }
            return null;
        }

        public async Task<News> Delete(News News)
        {
            if (!await NewsValidator.Delete(News))
                return News;

            try
            {
                await UOW.NewsRepository.Delete(News);
                Logging.CreateAuditLog(new { }, News, nameof(NewsService));
                return News;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NewsService));
            }
            return null;
        }

        public async Task<List<News>> BulkDelete(List<News> News)
        {
            if (!await NewsValidator.BulkDelete(News))
                return News;

            try
            {
                await UOW.NewsRepository.BulkDelete(News);
                Logging.CreateAuditLog(new { }, News, nameof(NewsService));
                return News;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NewsService));
            }
            return null;

        }
        
        public async Task<List<News>> Import(List<News> News)
        {
            if (!await NewsValidator.Import(News))
                return News;
            try
            {
                await UOW.NewsRepository.BulkMerge(News);

                Logging.CreateAuditLog(News, new { }, nameof(NewsService));
                return News;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NewsService));
            }
            return null;
        }     
        
        public async Task<NewsFilter> ToFilter(NewsFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<NewsFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                NewsFilter subFilter = new NewsFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = FilterBuilder.Merge(subFilter.CreatorId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.NewsContent))
                        subFilter.NewsContent = FilterBuilder.Merge(subFilter.NewsContent, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.LikeCounting))
                        subFilter.LikeCounting = FilterBuilder.Merge(subFilter.LikeCounting, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.WatchCounting))
                        subFilter.WatchCounting = FilterBuilder.Merge(subFilter.WatchCounting, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.NewsStatusId))
                        subFilter.NewsStatusId = FilterBuilder.Merge(subFilter.NewsStatusId, FilterPermissionDefinition.IdFilter);
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

        private void Sync(List<News> News)
        {
            List<AppUser> AppUsers = new List<AppUser>();
            List<NewsStatus> NewsStatuses = new List<NewsStatus>();
            AppUsers.AddRange(News.Select(x => new AppUser { Id = x.CreatorId }));
            NewsStatuses.AddRange(News.Select(x => new NewsStatus { Id = x.NewsStatusId }));
            
            AppUsers = AppUsers.Distinct().ToList();
            NewsStatuses = NewsStatuses.Distinct().ToList();
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
            RabbitManager.PublishList(NewsStatuses, RoutingKeyEnum.NewsStatusUsed.Code);
        }

    }
}
