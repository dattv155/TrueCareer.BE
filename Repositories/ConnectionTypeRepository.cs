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
    public interface IConnectionTypeRepository
    {
        Task<int> CountAll(ConnectionTypeFilter ConnectionTypeFilter);
        Task<int> Count(ConnectionTypeFilter ConnectionTypeFilter);
        Task<List<ConnectionType>> List(ConnectionTypeFilter ConnectionTypeFilter);
        Task<List<ConnectionType>> List(List<long> Ids);
    }
    public class ConnectionTypeRepository : IConnectionTypeRepository
    {
        private DataContext DataContext;
        public ConnectionTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ConnectionTypeDAO>> DynamicFilter(IQueryable<ConnectionTypeDAO> query, ConnectionTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Code, filter.Code);
            return query;
        }

        private async Task<IQueryable<ConnectionTypeDAO>> OrFilter(IQueryable<ConnectionTypeDAO> query, ConnectionTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ConnectionTypeDAO> initQuery = query.Where(q => false);
            foreach (ConnectionTypeFilter ConnectionTypeFilter in filter.OrFilter)
            {
                IQueryable<ConnectionTypeDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ConnectionTypeFilter.Id);
                queryable = queryable.Where(q => q.Name, ConnectionTypeFilter.Name);
                queryable = queryable.Where(q => q.Code, ConnectionTypeFilter.Code);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ConnectionTypeDAO> DynamicOrder(IQueryable<ConnectionTypeDAO> query, ConnectionTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ConnectionTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ConnectionTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ConnectionTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ConnectionTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ConnectionTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ConnectionTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ConnectionType>> DynamicSelect(IQueryable<ConnectionTypeDAO> query, ConnectionTypeFilter filter)
        {
            List<ConnectionType> ConnectionTypes = await query.Select(q => new ConnectionType()
            {
                Id = filter.Selects.Contains(ConnectionTypeSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(ConnectionTypeSelect.Name) ? q.Name : default(string),
                Code = filter.Selects.Contains(ConnectionTypeSelect.Code) ? q.Code : default(string),
            }).ToListAsync();
            return ConnectionTypes;
        }

        public async Task<int> CountAll(ConnectionTypeFilter filter)
        {
            IQueryable<ConnectionTypeDAO> ConnectionTypeDAOs = DataContext.ConnectionType.AsNoTracking();
            ConnectionTypeDAOs = await DynamicFilter(ConnectionTypeDAOs, filter);
            return await ConnectionTypeDAOs.CountAsync();
        }

        public async Task<int> Count(ConnectionTypeFilter filter)
        {
            IQueryable<ConnectionTypeDAO> ConnectionTypeDAOs = DataContext.ConnectionType.AsNoTracking();
            ConnectionTypeDAOs = await DynamicFilter(ConnectionTypeDAOs, filter);
            ConnectionTypeDAOs = await OrFilter(ConnectionTypeDAOs, filter);
            return await ConnectionTypeDAOs.CountAsync();
        }

        public async Task<List<ConnectionType>> List(ConnectionTypeFilter filter)
        {
            if (filter == null) return new List<ConnectionType>();
            IQueryable<ConnectionTypeDAO> ConnectionTypeDAOs = DataContext.ConnectionType.AsNoTracking();
            ConnectionTypeDAOs = await DynamicFilter(ConnectionTypeDAOs, filter);
            ConnectionTypeDAOs = await OrFilter(ConnectionTypeDAOs, filter);
            ConnectionTypeDAOs = DynamicOrder(ConnectionTypeDAOs, filter);
            List<ConnectionType> ConnectionTypes = await DynamicSelect(ConnectionTypeDAOs, filter);
            return ConnectionTypes;
        }

        public async Task<List<ConnectionType>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ConnectionTypeDAO> query = DataContext.ConnectionType.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<ConnectionType> ConnectionTypes = await query.AsNoTracking()
            .Select(x => new ConnectionType()
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
            }).ToListAsync();
            

            return ConnectionTypes;
        }

    }
}
