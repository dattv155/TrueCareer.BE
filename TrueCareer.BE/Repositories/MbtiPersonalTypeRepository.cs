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
    public interface IMbtiPersonalTypeRepository
    {
        Task<int> CountAll(MbtiPersonalTypeFilter MbtiPersonalTypeFilter);
        Task<int> Count(MbtiPersonalTypeFilter MbtiPersonalTypeFilter);
        Task<List<MbtiPersonalType>> List(MbtiPersonalTypeFilter MbtiPersonalTypeFilter);
        Task<List<MbtiPersonalType>> List(List<long> Ids);
    }
    public class MbtiPersonalTypeRepository : IMbtiPersonalTypeRepository
    {
        private DataContext DataContext;
        public MbtiPersonalTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<MbtiPersonalTypeDAO>> DynamicFilter(IQueryable<MbtiPersonalTypeDAO> query, MbtiPersonalTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Code, filter.Code);
            return query;
        }

        private async Task<IQueryable<MbtiPersonalTypeDAO>> OrFilter(IQueryable<MbtiPersonalTypeDAO> query, MbtiPersonalTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MbtiPersonalTypeDAO> initQuery = query.Where(q => false);
            foreach (MbtiPersonalTypeFilter MbtiPersonalTypeFilter in filter.OrFilter)
            {
                IQueryable<MbtiPersonalTypeDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, MbtiPersonalTypeFilter.Id);
                queryable = queryable.Where(q => q.Name, MbtiPersonalTypeFilter.Name);
                queryable = queryable.Where(q => q.Code, MbtiPersonalTypeFilter.Code);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<MbtiPersonalTypeDAO> DynamicOrder(IQueryable<MbtiPersonalTypeDAO> query, MbtiPersonalTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MbtiPersonalTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case MbtiPersonalTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case MbtiPersonalTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MbtiPersonalTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case MbtiPersonalTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case MbtiPersonalTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<MbtiPersonalType>> DynamicSelect(IQueryable<MbtiPersonalTypeDAO> query, MbtiPersonalTypeFilter filter)
        {
            List<MbtiPersonalType> MbtiPersonalTypes = await query.Select(q => new MbtiPersonalType()
            {
                Id = filter.Selects.Contains(MbtiPersonalTypeSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(MbtiPersonalTypeSelect.Name) ? q.Name : default(string),
                Code = filter.Selects.Contains(MbtiPersonalTypeSelect.Code) ? q.Code : default(string),
            }).ToListAsync();
            return MbtiPersonalTypes;
        }

        public async Task<int> CountAll(MbtiPersonalTypeFilter filter)
        {
            IQueryable<MbtiPersonalTypeDAO> MbtiPersonalTypeDAOs = DataContext.MbtiPersonalType.AsNoTracking();
            MbtiPersonalTypeDAOs = await DynamicFilter(MbtiPersonalTypeDAOs, filter);
            return await MbtiPersonalTypeDAOs.CountAsync();
        }

        public async Task<int> Count(MbtiPersonalTypeFilter filter)
        {
            IQueryable<MbtiPersonalTypeDAO> MbtiPersonalTypeDAOs = DataContext.MbtiPersonalType.AsNoTracking();
            MbtiPersonalTypeDAOs = await DynamicFilter(MbtiPersonalTypeDAOs, filter);
            MbtiPersonalTypeDAOs = await OrFilter(MbtiPersonalTypeDAOs, filter);
            return await MbtiPersonalTypeDAOs.CountAsync();
        }

        public async Task<List<MbtiPersonalType>> List(MbtiPersonalTypeFilter filter)
        {
            if (filter == null) return new List<MbtiPersonalType>();
            IQueryable<MbtiPersonalTypeDAO> MbtiPersonalTypeDAOs = DataContext.MbtiPersonalType.AsNoTracking();
            MbtiPersonalTypeDAOs = await DynamicFilter(MbtiPersonalTypeDAOs, filter);
            MbtiPersonalTypeDAOs = await OrFilter(MbtiPersonalTypeDAOs, filter);
            MbtiPersonalTypeDAOs = DynamicOrder(MbtiPersonalTypeDAOs, filter);
            List<MbtiPersonalType> MbtiPersonalTypes = await DynamicSelect(MbtiPersonalTypeDAOs, filter);
            return MbtiPersonalTypes;
        }

        public async Task<List<MbtiPersonalType>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MbtiPersonalTypeDAO> query = DataContext.MbtiPersonalType.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<MbtiPersonalType> MbtiPersonalTypes = await query.AsNoTracking()
            .Select(x => new MbtiPersonalType()
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
            }).ToListAsync();
            

            return MbtiPersonalTypes;
        }

    }
}
