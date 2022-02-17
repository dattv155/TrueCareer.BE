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
    public interface INewsStatusRepository
    {
        Task<int> CountAll(NewsStatusFilter NewsStatusFilter);
        Task<int> Count(NewsStatusFilter NewsStatusFilter);
        Task<List<NewsStatus>> List(NewsStatusFilter NewsStatusFilter);
        Task<List<NewsStatus>> List(List<long> Ids);
    }
    public class NewsStatusRepository : INewsStatusRepository
    {
        private DataContext DataContext;
        public NewsStatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<NewsStatusDAO>> DynamicFilter(IQueryable<NewsStatusDAO> query, NewsStatusFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            return query;
        }

        private async Task<IQueryable<NewsStatusDAO>> OrFilter(IQueryable<NewsStatusDAO> query, NewsStatusFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<NewsStatusDAO> initQuery = query.Where(q => false);
            foreach (NewsStatusFilter NewsStatusFilter in filter.OrFilter)
            {
                IQueryable<NewsStatusDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, NewsStatusFilter.Id);
                queryable = queryable.Where(q => q.Code, NewsStatusFilter.Code);
                queryable = queryable.Where(q => q.Name, NewsStatusFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<NewsStatusDAO> DynamicOrder(IQueryable<NewsStatusDAO> query, NewsStatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case NewsStatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case NewsStatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case NewsStatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case NewsStatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case NewsStatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case NewsStatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<NewsStatus>> DynamicSelect(IQueryable<NewsStatusDAO> query, NewsStatusFilter filter)
        {
            List<NewsStatus> NewsStatuses = await query.Select(q => new NewsStatus()
            {
                Id = filter.Selects.Contains(NewsStatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(NewsStatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(NewsStatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return NewsStatuses;
        }

        public async Task<int> CountAll(NewsStatusFilter filter)
        {
            IQueryable<NewsStatusDAO> NewsStatusDAOs = DataContext.NewsStatus.AsNoTracking();
            NewsStatusDAOs = await DynamicFilter(NewsStatusDAOs, filter);
            return await NewsStatusDAOs.CountAsync();
        }

        public async Task<int> Count(NewsStatusFilter filter)
        {
            IQueryable<NewsStatusDAO> NewsStatusDAOs = DataContext.NewsStatus.AsNoTracking();
            NewsStatusDAOs = await DynamicFilter(NewsStatusDAOs, filter);
            NewsStatusDAOs = await OrFilter(NewsStatusDAOs, filter);
            return await NewsStatusDAOs.CountAsync();
        }

        public async Task<List<NewsStatus>> List(NewsStatusFilter filter)
        {
            if (filter == null) return new List<NewsStatus>();
            IQueryable<NewsStatusDAO> NewsStatusDAOs = DataContext.NewsStatus.AsNoTracking();
            NewsStatusDAOs = await DynamicFilter(NewsStatusDAOs, filter);
            NewsStatusDAOs = await OrFilter(NewsStatusDAOs, filter);
            NewsStatusDAOs = DynamicOrder(NewsStatusDAOs, filter);
            List<NewsStatus> NewsStatuses = await DynamicSelect(NewsStatusDAOs, filter);
            return NewsStatuses;
        }

        public async Task<List<NewsStatus>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<NewsStatusDAO> query = DataContext.NewsStatus.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<NewsStatus> NewsStatuses = await query.AsNoTracking()
            .Select(x => new NewsStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return NewsStatuses;
        }

    }
}
