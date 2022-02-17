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
    public interface IFavouriteNewsRepository
    {
        Task<int> CountAll(FavouriteNewsFilter FavouriteNewsFilter);
        Task<int> Count(FavouriteNewsFilter FavouriteNewsFilter);
        Task<List<FavouriteNews>> List(FavouriteNewsFilter FavouriteNewsFilter);
        Task<List<FavouriteNews>> List(List<long> Ids);
        Task<FavouriteNews> Get(long Id);
        Task<bool> Create(FavouriteNews FavouriteNews);
        Task<bool> Update(FavouriteNews FavouriteNews);
        Task<bool> Delete(FavouriteNews FavouriteNews);
        Task<bool> BulkMerge(List<FavouriteNews> FavouriteNews);
        Task<bool> BulkDelete(List<FavouriteNews> FavouriteNews);
    }
    public class FavouriteNewsRepository : IFavouriteNewsRepository
    {
        private DataContext DataContext;
        public FavouriteNewsRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<FavouriteNewsDAO>> DynamicFilter(IQueryable<FavouriteNewsDAO> query, FavouriteNewsFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.NewsId, filter.NewsId);
            query = query.Where(q => q.UserId, filter.UserId);
            return query;
        }

        private async Task<IQueryable<FavouriteNewsDAO>> OrFilter(IQueryable<FavouriteNewsDAO> query, FavouriteNewsFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<FavouriteNewsDAO> initQuery = query.Where(q => false);
            foreach (FavouriteNewsFilter FavouriteNewsFilter in filter.OrFilter)
            {
                IQueryable<FavouriteNewsDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, FavouriteNewsFilter.Id);
                queryable = queryable.Where(q => q.NewsId, FavouriteNewsFilter.NewsId);
                queryable = queryable.Where(q => q.UserId, FavouriteNewsFilter.UserId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<FavouriteNewsDAO> DynamicOrder(IQueryable<FavouriteNewsDAO> query, FavouriteNewsFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case FavouriteNewsOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case FavouriteNewsOrder.User:
                            query = query.OrderBy(q => q.UserId);
                            break;
                        case FavouriteNewsOrder.News:
                            query = query.OrderBy(q => q.NewsId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case FavouriteNewsOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case FavouriteNewsOrder.User:
                            query = query.OrderByDescending(q => q.UserId);
                            break;
                        case FavouriteNewsOrder.News:
                            query = query.OrderByDescending(q => q.NewsId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<FavouriteNews>> DynamicSelect(IQueryable<FavouriteNewsDAO> query, FavouriteNewsFilter filter)
        {
            List<FavouriteNews> FavouriteNews = await query.Select(q => new FavouriteNews()
            {
                Id = filter.Selects.Contains(FavouriteNewsSelect.Id) ? q.Id : default(long),
                UserId = filter.Selects.Contains(FavouriteNewsSelect.User) ? q.UserId : default(long),
                NewsId = filter.Selects.Contains(FavouriteNewsSelect.News) ? q.NewsId : default(long),
                News = filter.Selects.Contains(FavouriteNewsSelect.News) && q.News != null ? new News
                {
                    Id = q.News.Id,
                    CreatorId = q.News.CreatorId,
                    NewsContent = q.News.NewsContent,
                    LikeCounting = q.News.LikeCounting,
                    WatchCounting = q.News.WatchCounting,
                    NewsStatusId = q.News.NewsStatusId,
                } : null,
                User = filter.Selects.Contains(FavouriteNewsSelect.User) && q.User != null ? new AppUser
                {
                    Id = q.User.Id,
                    Username = q.User.Username,
                    Email = q.User.Email,
                    Phone = q.User.Phone,
                    Password = q.User.Password,
                    DisplayName = q.User.DisplayName,
                    SexId = q.User.SexId,
                    Birthday = q.User.Birthday,
                    Avatar = q.User.Avatar,
                    CoverImage = q.User.CoverImage,
                } : null,
            }).ToListAsync();
            return FavouriteNews;
        }

        public async Task<int> CountAll(FavouriteNewsFilter filter)
        {
            IQueryable<FavouriteNewsDAO> FavouriteNewsDAOs = DataContext.FavouriteNews.AsNoTracking();
            FavouriteNewsDAOs = await DynamicFilter(FavouriteNewsDAOs, filter);
            return await FavouriteNewsDAOs.CountAsync();
        }

        public async Task<int> Count(FavouriteNewsFilter filter)
        {
            IQueryable<FavouriteNewsDAO> FavouriteNewsDAOs = DataContext.FavouriteNews.AsNoTracking();
            FavouriteNewsDAOs = await DynamicFilter(FavouriteNewsDAOs, filter);
            FavouriteNewsDAOs = await OrFilter(FavouriteNewsDAOs, filter);
            return await FavouriteNewsDAOs.CountAsync();
        }

        public async Task<List<FavouriteNews>> List(FavouriteNewsFilter filter)
        {
            if (filter == null) return new List<FavouriteNews>();
            IQueryable<FavouriteNewsDAO> FavouriteNewsDAOs = DataContext.FavouriteNews.AsNoTracking();
            FavouriteNewsDAOs = await DynamicFilter(FavouriteNewsDAOs, filter);
            FavouriteNewsDAOs = await OrFilter(FavouriteNewsDAOs, filter);
            FavouriteNewsDAOs = DynamicOrder(FavouriteNewsDAOs, filter);
            List<FavouriteNews> FavouriteNews = await DynamicSelect(FavouriteNewsDAOs, filter);
            return FavouriteNews;
        }

        public async Task<List<FavouriteNews>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<FavouriteNewsDAO> query = DataContext.FavouriteNews.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<FavouriteNews> FavouriteNews = await query.AsNoTracking()
            .Select(x => new FavouriteNews()
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
                User = x.User == null ? null : new AppUser
                {
                    Id = x.User.Id,
                    Username = x.User.Username,
                    Email = x.User.Email,
                    Phone = x.User.Phone,
                    Password = x.User.Password,
                    DisplayName = x.User.DisplayName,
                    SexId = x.User.SexId,
                    Birthday = x.User.Birthday,
                    Avatar = x.User.Avatar,
                    CoverImage = x.User.CoverImage,
                },
            }).ToListAsync();
            

            return FavouriteNews;
        }

        public async Task<FavouriteNews> Get(long Id)
        {
            FavouriteNews FavouriteNews = await DataContext.FavouriteNews.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new FavouriteNews()
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
                User = x.User == null ? null : new AppUser
                {
                    Id = x.User.Id,
                    Username = x.User.Username,
                    Email = x.User.Email,
                    Phone = x.User.Phone,
                    Password = x.User.Password,
                    DisplayName = x.User.DisplayName,
                    SexId = x.User.SexId,
                    Birthday = x.User.Birthday,
                    Avatar = x.User.Avatar,
                    CoverImage = x.User.CoverImage,
                },
            }).FirstOrDefaultAsync();

            if (FavouriteNews == null)
                return null;

            return FavouriteNews;
        }
        
        public async Task<bool> Create(FavouriteNews FavouriteNews)
        {
            FavouriteNewsDAO FavouriteNewsDAO = new FavouriteNewsDAO();
            FavouriteNewsDAO.Id = FavouriteNews.Id;
            FavouriteNewsDAO.UserId = FavouriteNews.UserId;
            FavouriteNewsDAO.NewsId = FavouriteNews.NewsId;
            DataContext.FavouriteNews.Add(FavouriteNewsDAO);
            await DataContext.SaveChangesAsync();
            FavouriteNews.Id = FavouriteNewsDAO.Id;
            await SaveReference(FavouriteNews);
            return true;
        }

        public async Task<bool> Update(FavouriteNews FavouriteNews)
        {
            FavouriteNewsDAO FavouriteNewsDAO = DataContext.FavouriteNews
                .Where(x => x.Id == FavouriteNews.Id)
                .FirstOrDefault();
            if (FavouriteNewsDAO == null)
                return false;
            FavouriteNewsDAO.Id = FavouriteNews.Id;
            FavouriteNewsDAO.UserId = FavouriteNews.UserId;
            FavouriteNewsDAO.NewsId = FavouriteNews.NewsId;
            await DataContext.SaveChangesAsync();
            await SaveReference(FavouriteNews);
            return true;
        }

        public async Task<bool> Delete(FavouriteNews FavouriteNews)
        {
            await DataContext.FavouriteNews
                .Where(x => x.Id == FavouriteNews.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<FavouriteNews> FavouriteNews)
        {
            IdFilter IdFilter = new IdFilter { In = FavouriteNews.Select(x => x.Id).ToList() };
            List<FavouriteNewsDAO> FavouriteNewsDAOs = new List<FavouriteNewsDAO>();
            List<FavouriteNewsDAO> DbFavouriteNewsDAOs = await DataContext.FavouriteNews
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (FavouriteNews FavouriteNew in FavouriteNews)
            {
                FavouriteNewsDAO FavouriteNewsDAO = DbFavouriteNewsDAOs
                        .Where(x => x.Id == FavouriteNew.Id)
                        .FirstOrDefault();
                if (FavouriteNewsDAO == null)
                {
                    FavouriteNewsDAO = new FavouriteNewsDAO();
                }
                FavouriteNewsDAO.UserId = FavouriteNew.UserId;
                FavouriteNewsDAO.NewsId = FavouriteNew.NewsId;
                FavouriteNewsDAOs.Add(FavouriteNewsDAO);
            }
            await DataContext.BulkMergeAsync(FavouriteNewsDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<FavouriteNews> FavouriteNews)
        {
            List<long> Ids = FavouriteNews.Select(x => x.Id).ToList();
            await DataContext.FavouriteNews
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(FavouriteNews FavouriteNews)
        {
        }
        
    }
}
