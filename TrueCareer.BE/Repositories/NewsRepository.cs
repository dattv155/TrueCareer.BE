using TrueSight.Common;
using TrueCareer.Common;
using TrueCareer.Helpers;
using TrueCareer.Entities;
using TrueCareer.Models;
using TrueCareer.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace TrueCareer.Repositories
{
    public interface INewsRepository
    {
        Task<int> CountAll(NewsFilter NewsFilter);
        Task<int> Count(NewsFilter NewsFilter);
        Task<List<News>> List(NewsFilter NewsFilter);
        Task<List<News>> List(List<long> Ids);
        Task<News> Get(long Id);
        Task<bool> Create(News News);
        Task<bool> Update(News News);
        Task<bool> Delete(News News);
        Task<bool> BulkMerge(List<News> News);
        Task<bool> BulkDelete(List<News> News);
    }
    public class NewsRepository : INewsRepository
    {
        private DataContext DataContext;
        public NewsRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<NewsDAO>> DynamicFilter(IQueryable<NewsDAO> query, NewsFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.NewsContent, filter.NewsContent);
            query = query.Where(q => q.LikeCounting, filter.LikeCounting);
            query = query.Where(q => q.WatchCounting, filter.WatchCounting);
            query = query.Where(q => q.CreatorId, filter.CreatorId);
            query = query.Where(q => q.NewsStatusId, filter.NewsStatusId);
            return query;
        }

        private async Task<IQueryable<NewsDAO>> OrFilter(IQueryable<NewsDAO> query, NewsFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<NewsDAO> initQuery = query.Where(q => false);
            foreach (NewsFilter NewsFilter in filter.OrFilter)
            {
                IQueryable<NewsDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, NewsFilter.Id);
                queryable = queryable.Where(q => q.NewsContent, NewsFilter.NewsContent);
                queryable = queryable.Where(q => q.LikeCounting, NewsFilter.LikeCounting);
                queryable = queryable.Where(q => q.WatchCounting, NewsFilter.WatchCounting);
                queryable = queryable.Where(q => q.CreatorId, NewsFilter.CreatorId);
                queryable = queryable.Where(q => q.NewsStatusId, NewsFilter.NewsStatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<NewsDAO> DynamicOrder(IQueryable<NewsDAO> query, NewsFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case NewsOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case NewsOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case NewsOrder.NewsContent:
                            query = query.OrderBy(q => q.NewsContent);
                            break;
                        case NewsOrder.LikeCounting:
                            query = query.OrderBy(q => q.LikeCounting);
                            break;
                        case NewsOrder.WatchCounting:
                            query = query.OrderBy(q => q.WatchCounting);
                            break;
                        case NewsOrder.NewsStatus:
                            query = query.OrderBy(q => q.NewsStatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case NewsOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case NewsOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case NewsOrder.NewsContent:
                            query = query.OrderByDescending(q => q.NewsContent);
                            break;
                        case NewsOrder.LikeCounting:
                            query = query.OrderByDescending(q => q.LikeCounting);
                            break;
                        case NewsOrder.WatchCounting:
                            query = query.OrderByDescending(q => q.WatchCounting);
                            break;
                        case NewsOrder.NewsStatus:
                            query = query.OrderByDescending(q => q.NewsStatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<News>> DynamicSelect(IQueryable<NewsDAO> query, NewsFilter filter)
        {
            List<News> News = await query.Select(q => new News()
            {
                Id = filter.Selects.Contains(NewsSelect.Id) ? q.Id : default(long),
                CreatorId = filter.Selects.Contains(NewsSelect.Creator) ? q.CreatorId : default(long),
                NewsContent = filter.Selects.Contains(NewsSelect.NewsContent) ? q.NewsContent : default(string),
                LikeCounting = filter.Selects.Contains(NewsSelect.LikeCounting) ? q.LikeCounting : default(long),
                WatchCounting = filter.Selects.Contains(NewsSelect.WatchCounting) ? q.WatchCounting : default(long),
                NewsStatusId = filter.Selects.Contains(NewsSelect.NewsStatus) ? q.NewsStatusId : default(long),
                Creator = filter.Selects.Contains(NewsSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    Email = q.Creator.Email,
                    Phone = q.Creator.Phone,
                    Password = q.Creator.Password,
                    DisplayName = q.Creator.DisplayName,
                    SexId = q.Creator.SexId,
                    Birthday = q.Creator.Birthday,
                    Avatar = q.Creator.Avatar,
                    CoverImage = q.Creator.CoverImage,
                } : null,
                NewsStatus = filter.Selects.Contains(NewsSelect.NewsStatus) && q.NewsStatus != null ? new NewsStatus
                {
                    Id = q.NewsStatus.Id,
                    Code = q.NewsStatus.Code,
                    Name = q.NewsStatus.Name,
                } : null,
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return News;
        }

        public async Task<int> CountAll(NewsFilter filter)
        {
            IQueryable<NewsDAO> NewsDAOs = DataContext.News.AsNoTracking();
            NewsDAOs = await DynamicFilter(NewsDAOs, filter);
            return await NewsDAOs.CountAsync();
        }

        public async Task<int> Count(NewsFilter filter)
        {
            IQueryable<NewsDAO> NewsDAOs = DataContext.News.AsNoTracking();
            NewsDAOs = await DynamicFilter(NewsDAOs, filter);
            NewsDAOs = await OrFilter(NewsDAOs, filter);
            return await NewsDAOs.CountAsync();
        }

        public async Task<List<News>> List(NewsFilter filter)
        {
            if (filter == null) return new List<News>();
            IQueryable<NewsDAO> NewsDAOs = DataContext.News.AsNoTracking();
            NewsDAOs = await DynamicFilter(NewsDAOs, filter);
            NewsDAOs = await OrFilter(NewsDAOs, filter);
            NewsDAOs = DynamicOrder(NewsDAOs, filter);
            List<News> News = await DynamicSelect(NewsDAOs, filter);
            return News;
        }

        public async Task<List<News>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<NewsDAO> query = DataContext.News.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<News> News = await query.AsNoTracking()
            .Select(x => new News()
            {
                RowId = x.RowId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                CreatorId = x.CreatorId,
                NewsContent = x.NewsContent,
                LikeCounting = x.LikeCounting,
                WatchCounting = x.WatchCounting,
                NewsStatusId = x.NewsStatusId,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    Password = x.Creator.Password,
                    DisplayName = x.Creator.DisplayName,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                    Avatar = x.Creator.Avatar,
                    CoverImage = x.Creator.CoverImage,
                },
                NewsStatus = x.NewsStatus == null ? null : new NewsStatus
                {
                    Id = x.NewsStatus.Id,
                    Code = x.NewsStatus.Code,
                    Name = x.NewsStatus.Name,
                },
            }).ToListAsync();
            

            return News;
        }

        public async Task<News> Get(long Id)
        {
            News News = await DataContext.News.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new News()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                CreatorId = x.CreatorId,
                NewsContent = x.NewsContent,
                LikeCounting = x.LikeCounting,
                WatchCounting = x.WatchCounting,
                NewsStatusId = x.NewsStatusId,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    Password = x.Creator.Password,
                    DisplayName = x.Creator.DisplayName,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                    Avatar = x.Creator.Avatar,
                    CoverImage = x.Creator.CoverImage,
                },
                NewsStatus = x.NewsStatus == null ? null : new NewsStatus
                {
                    Id = x.NewsStatus.Id,
                    Code = x.NewsStatus.Code,
                    Name = x.NewsStatus.Name,
                },
            }).FirstOrDefaultAsync();

            if (News == null)
                return null;

            return News;
        }
        
        public async Task<bool> Create(News News)
        {
            NewsDAO NewsDAO = new NewsDAO();
            NewsDAO.Id = News.Id;
            NewsDAO.CreatorId = News.CreatorId;
            NewsDAO.NewsContent = News.NewsContent;
            NewsDAO.LikeCounting = News.LikeCounting;
            NewsDAO.WatchCounting = News.WatchCounting;
            NewsDAO.NewsStatusId = News.NewsStatusId;
            NewsDAO.RowId = Guid.NewGuid();
            NewsDAO.CreatedAt = StaticParams.DateTimeNow;
            NewsDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.News.Add(NewsDAO);
            await DataContext.SaveChangesAsync();
            News.Id = NewsDAO.Id;
            await SaveReference(News);
            return true;
        }

        public async Task<bool> Update(News News)
        {
            NewsDAO NewsDAO = DataContext.News
                .Where(x => x.Id == News.Id)
                .FirstOrDefault();
            if (NewsDAO == null)
                return false;
            NewsDAO.Id = News.Id;
            NewsDAO.CreatorId = News.CreatorId;
            NewsDAO.NewsContent = News.NewsContent;
            NewsDAO.LikeCounting = News.LikeCounting;
            NewsDAO.WatchCounting = News.WatchCounting;
            NewsDAO.NewsStatusId = News.NewsStatusId;
            NewsDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(News);
            return true;
        }

        public async Task<bool> Delete(News News)
        {
            await DataContext.News
                .Where(x => x.Id == News.Id)
                .UpdateFromQueryAsync(x => new NewsDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<News> News)
        {
            IdFilter IdFilter = new IdFilter { In = News.Select(x => x.Id).ToList() };
            List<NewsDAO> NewsDAOs = new List<NewsDAO>();
            List<NewsDAO> DbNewsDAOs = await DataContext.News
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (News New in News)
            {
                NewsDAO NewsDAO = DbNewsDAOs
                        .Where(x => x.Id == New.Id)
                        .FirstOrDefault();
                if (NewsDAO == null)
                {
                    NewsDAO = new NewsDAO();
                    NewsDAO.CreatedAt = StaticParams.DateTimeNow;
                    NewsDAO.RowId = Guid.NewGuid();
                    New.RowId = NewsDAO.RowId;
                }
                NewsDAO.CreatorId = New.CreatorId;
                NewsDAO.NewsContent = New.NewsContent;
                NewsDAO.LikeCounting = New.LikeCounting;
                NewsDAO.WatchCounting = New.WatchCounting;
                NewsDAO.NewsStatusId = New.NewsStatusId;
                NewsDAO.UpdatedAt = StaticParams.DateTimeNow;
                NewsDAOs.Add(NewsDAO);
            }
            await DataContext.BulkMergeAsync(NewsDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<News> News)
        {
            List<long> Ids = News.Select(x => x.Id).ToList();
            await DataContext.News
                .WhereBulkContains(Ids, x => x.Id)
                .UpdateFromQueryAsync(x => new NewsDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(News News)
        {
        }
        
    }
}
