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
    public interface IInformationTypeRepository
    {
        Task<int> CountAll(InformationTypeFilter InformationTypeFilter);
        Task<int> Count(InformationTypeFilter InformationTypeFilter);
        Task<List<InformationType>> List(InformationTypeFilter InformationTypeFilter);
        Task<List<InformationType>> List(List<long> Ids);
    }
    public class InformationTypeRepository : IInformationTypeRepository
    {
        private DataContext DataContext;
        public InformationTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<InformationTypeDAO>> DynamicFilter(IQueryable<InformationTypeDAO> query, InformationTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Code, filter.Code);
            return query;
        }

        private async Task<IQueryable<InformationTypeDAO>> OrFilter(IQueryable<InformationTypeDAO> query, InformationTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<InformationTypeDAO> initQuery = query.Where(q => false);
            foreach (InformationTypeFilter InformationTypeFilter in filter.OrFilter)
            {
                IQueryable<InformationTypeDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, InformationTypeFilter.Id);
                queryable = queryable.Where(q => q.Name, InformationTypeFilter.Name);
                queryable = queryable.Where(q => q.Code, InformationTypeFilter.Code);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<InformationTypeDAO> DynamicOrder(IQueryable<InformationTypeDAO> query, InformationTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case InformationTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case InformationTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case InformationTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case InformationTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case InformationTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case InformationTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<InformationType>> DynamicSelect(IQueryable<InformationTypeDAO> query, InformationTypeFilter filter)
        {
            List<InformationType> InformationTypes = await query.Select(q => new InformationType()
            {
                Id = filter.Selects.Contains(InformationTypeSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(InformationTypeSelect.Name) ? q.Name : default(string),
                Code = filter.Selects.Contains(InformationTypeSelect.Code) ? q.Code : default(string),
            }).ToListAsync();
            return InformationTypes;
        }

        public async Task<int> CountAll(InformationTypeFilter filter)
        {
            IQueryable<InformationTypeDAO> InformationTypeDAOs = DataContext.InformationType.AsNoTracking();
            InformationTypeDAOs = await DynamicFilter(InformationTypeDAOs, filter);
            return await InformationTypeDAOs.CountAsync();
        }

        public async Task<int> Count(InformationTypeFilter filter)
        {
            IQueryable<InformationTypeDAO> InformationTypeDAOs = DataContext.InformationType.AsNoTracking();
            InformationTypeDAOs = await DynamicFilter(InformationTypeDAOs, filter);
            InformationTypeDAOs = await OrFilter(InformationTypeDAOs, filter);
            return await InformationTypeDAOs.CountAsync();
        }

        public async Task<List<InformationType>> List(InformationTypeFilter filter)
        {
            if (filter == null) return new List<InformationType>();
            IQueryable<InformationTypeDAO> InformationTypeDAOs = DataContext.InformationType.AsNoTracking();
            InformationTypeDAOs = await DynamicFilter(InformationTypeDAOs, filter);
            InformationTypeDAOs = await OrFilter(InformationTypeDAOs, filter);
            InformationTypeDAOs = DynamicOrder(InformationTypeDAOs, filter);
            List<InformationType> InformationTypes = await DynamicSelect(InformationTypeDAOs, filter);
            return InformationTypes;
        }

        public async Task<List<InformationType>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<InformationTypeDAO> query = DataContext.InformationType.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<InformationType> InformationTypes = await query.AsNoTracking()
            .Select(x => new InformationType()
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
            }).ToListAsync();
            

            return InformationTypes;
        }

    }
}
