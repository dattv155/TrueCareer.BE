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
    public interface IMbtiSingleTypeRepository
    {
        Task<int> CountAll(MbtiSingleTypeFilter MbtiSingleTypeFilter);
        Task<int> Count(MbtiSingleTypeFilter MbtiSingleTypeFilter);
        Task<List<MbtiSingleType>> List(MbtiSingleTypeFilter MbtiSingleTypeFilter);
        Task<List<MbtiSingleType>> List(List<long> Ids);
    }
    public class MbtiSingleTypeRepository : IMbtiSingleTypeRepository
    {
        private DataContext DataContext;
        public MbtiSingleTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<MbtiSingleTypeDAO>> DynamicFilter(IQueryable<MbtiSingleTypeDAO> query, MbtiSingleTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            return query;
        }

        private async Task<IQueryable<MbtiSingleTypeDAO>> OrFilter(IQueryable<MbtiSingleTypeDAO> query, MbtiSingleTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MbtiSingleTypeDAO> initQuery = query.Where(q => false);
            foreach (MbtiSingleTypeFilter MbtiSingleTypeFilter in filter.OrFilter)
            {
                IQueryable<MbtiSingleTypeDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, MbtiSingleTypeFilter.Id);
                queryable = queryable.Where(q => q.Code, MbtiSingleTypeFilter.Code);
                queryable = queryable.Where(q => q.Name, MbtiSingleTypeFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<MbtiSingleTypeDAO> DynamicOrder(IQueryable<MbtiSingleTypeDAO> query, MbtiSingleTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MbtiSingleTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case MbtiSingleTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case MbtiSingleTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MbtiSingleTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case MbtiSingleTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case MbtiSingleTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<MbtiSingleType>> DynamicSelect(IQueryable<MbtiSingleTypeDAO> query, MbtiSingleTypeFilter filter)
        {
            List<MbtiSingleType> MbtiSingleTypes = await query.Select(q => new MbtiSingleType()
            {
                Id = filter.Selects.Contains(MbtiSingleTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(MbtiSingleTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(MbtiSingleTypeSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return MbtiSingleTypes;
        }

        public async Task<int> CountAll(MbtiSingleTypeFilter filter)
        {
            IQueryable<MbtiSingleTypeDAO> MbtiSingleTypeDAOs = DataContext.MbtiSingleType.AsNoTracking();
            MbtiSingleTypeDAOs = await DynamicFilter(MbtiSingleTypeDAOs, filter);
            return await MbtiSingleTypeDAOs.CountAsync();
        }

        public async Task<int> Count(MbtiSingleTypeFilter filter)
        {
            IQueryable<MbtiSingleTypeDAO> MbtiSingleTypeDAOs = DataContext.MbtiSingleType.AsNoTracking();
            MbtiSingleTypeDAOs = await DynamicFilter(MbtiSingleTypeDAOs, filter);
            MbtiSingleTypeDAOs = await OrFilter(MbtiSingleTypeDAOs, filter);
            return await MbtiSingleTypeDAOs.CountAsync();
        }

        public async Task<List<MbtiSingleType>> List(MbtiSingleTypeFilter filter)
        {
            if (filter == null) return new List<MbtiSingleType>();
            IQueryable<MbtiSingleTypeDAO> MbtiSingleTypeDAOs = DataContext.MbtiSingleType.AsNoTracking();
            MbtiSingleTypeDAOs = await DynamicFilter(MbtiSingleTypeDAOs, filter);
            MbtiSingleTypeDAOs = await OrFilter(MbtiSingleTypeDAOs, filter);
            MbtiSingleTypeDAOs = DynamicOrder(MbtiSingleTypeDAOs, filter);
            List<MbtiSingleType> MbtiSingleTypes = await DynamicSelect(MbtiSingleTypeDAOs, filter);
            return MbtiSingleTypes;
        }

        public async Task<List<MbtiSingleType>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MbtiSingleTypeDAO> query = DataContext.MbtiSingleType.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<MbtiSingleType> MbtiSingleTypes = await query.AsNoTracking()
            .Select(x => new MbtiSingleType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return MbtiSingleTypes;
        }

    }
}
