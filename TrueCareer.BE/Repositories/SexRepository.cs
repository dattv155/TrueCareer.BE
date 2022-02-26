using TrueSight.Common;
using TrueCareer.Common;
using TrueCareer.Helpers;
using TrueCareer.Entities;
using TrueCareer.BE.Models;
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
    public interface ISexRepository
    {
        Task<int> CountAll(SexFilter SexFilter);
        Task<int> Count(SexFilter SexFilter);
        Task<List<Sex>> List(SexFilter SexFilter);
        Task<List<Sex>> List(List<long> Ids);
    }
    public class SexRepository : ISexRepository
    {
        private DataContext DataContext;
        public SexRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<SexDAO>> DynamicFilter(IQueryable<SexDAO> query, SexFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            return query;
        }

        private async Task<IQueryable<SexDAO>> OrFilter(IQueryable<SexDAO> query, SexFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<SexDAO> initQuery = query.Where(q => false);
            foreach (SexFilter SexFilter in filter.OrFilter)
            {
                IQueryable<SexDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, SexFilter.Id);
                queryable = queryable.Where(q => q.Code, SexFilter.Code);
                queryable = queryable.Where(q => q.Name, SexFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<SexDAO> DynamicOrder(IQueryable<SexDAO> query, SexFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SexOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SexOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case SexOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SexOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SexOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case SexOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Sex>> DynamicSelect(IQueryable<SexDAO> query, SexFilter filter)
        {
            List<Sex> Sexes = await query.Select(q => new Sex()
            {
                Id = filter.Selects.Contains(SexSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(SexSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(SexSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return Sexes;
        }

        public async Task<int> CountAll(SexFilter filter)
        {
            IQueryable<SexDAO> SexDAOs = DataContext.Sex.AsNoTracking();
            SexDAOs = await DynamicFilter(SexDAOs, filter);
            return await SexDAOs.CountAsync();
        }

        public async Task<int> Count(SexFilter filter)
        {
            IQueryable<SexDAO> SexDAOs = DataContext.Sex.AsNoTracking();
            SexDAOs = await DynamicFilter(SexDAOs, filter);
            SexDAOs = await OrFilter(SexDAOs, filter);
            return await SexDAOs.CountAsync();
        }

        public async Task<List<Sex>> List(SexFilter filter)
        {
            if (filter == null) return new List<Sex>();
            IQueryable<SexDAO> SexDAOs = DataContext.Sex.AsNoTracking();
            SexDAOs = await DynamicFilter(SexDAOs, filter);
            SexDAOs = await OrFilter(SexDAOs, filter);
            SexDAOs = DynamicOrder(SexDAOs, filter);
            List<Sex> Sexes = await DynamicSelect(SexDAOs, filter);
            return Sexes;
        }

        public async Task<List<Sex>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<SexDAO> query = DataContext.Sex.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Sex> Sexes = await query.AsNoTracking()
            .Select(x => new Sex()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return Sexes;
        }

    }
}
