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
    public interface IConnectionStatusRepository
    {
        Task<int> CountAll(ConnectionStatusFilter ConnectionStatusFilter);
        Task<int> Count(ConnectionStatusFilter ConnectionStatusFilter);
        Task<List<ConnectionStatus>> List(ConnectionStatusFilter ConnectionStatusFilter);
        Task<List<ConnectionStatus>> List(List<long> Ids);
    }
    public class ConnectionStatusRepository : IConnectionStatusRepository
    {
        private DataContext DataContext;
        public ConnectionStatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ConnectionStatusDAO>> DynamicFilter(IQueryable<ConnectionStatusDAO> query, ConnectionStatusFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            return query;
        }

        private async Task<IQueryable<ConnectionStatusDAO>> OrFilter(IQueryable<ConnectionStatusDAO> query, ConnectionStatusFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ConnectionStatusDAO> initQuery = query.Where(q => false);
            foreach (ConnectionStatusFilter ConnectionStatusFilter in filter.OrFilter)
            {
                IQueryable<ConnectionStatusDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ConnectionStatusFilter.Id);
                queryable = queryable.Where(q => q.Code, ConnectionStatusFilter.Code);
                queryable = queryable.Where(q => q.Name, ConnectionStatusFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ConnectionStatusDAO> DynamicOrder(IQueryable<ConnectionStatusDAO> query, ConnectionStatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ConnectionStatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ConnectionStatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ConnectionStatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ConnectionStatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ConnectionStatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ConnectionStatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ConnectionStatus>> DynamicSelect(IQueryable<ConnectionStatusDAO> query, ConnectionStatusFilter filter)
        {
            List<ConnectionStatus> ConnectionStatuses = await query.Select(q => new ConnectionStatus()
            {
                Id = filter.Selects.Contains(ConnectionStatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ConnectionStatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ConnectionStatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return ConnectionStatuses;
        }

        public async Task<int> CountAll(ConnectionStatusFilter filter)
        {
            IQueryable<ConnectionStatusDAO> ConnectionStatusDAOs = DataContext.ConnectionStatus.AsNoTracking();
            ConnectionStatusDAOs = await DynamicFilter(ConnectionStatusDAOs, filter);
            return await ConnectionStatusDAOs.CountAsync();
        }

        public async Task<int> Count(ConnectionStatusFilter filter)
        {
            IQueryable<ConnectionStatusDAO> ConnectionStatusDAOs = DataContext.ConnectionStatus.AsNoTracking();
            ConnectionStatusDAOs = await DynamicFilter(ConnectionStatusDAOs, filter);
            ConnectionStatusDAOs = await OrFilter(ConnectionStatusDAOs, filter);
            return await ConnectionStatusDAOs.CountAsync();
        }

        public async Task<List<ConnectionStatus>> List(ConnectionStatusFilter filter)
        {
            if (filter == null) return new List<ConnectionStatus>();
            IQueryable<ConnectionStatusDAO> ConnectionStatusDAOs = DataContext.ConnectionStatus.AsNoTracking();
            ConnectionStatusDAOs = await DynamicFilter(ConnectionStatusDAOs, filter);
            ConnectionStatusDAOs = await OrFilter(ConnectionStatusDAOs, filter);
            ConnectionStatusDAOs = DynamicOrder(ConnectionStatusDAOs, filter);
            List<ConnectionStatus> ConnectionStatuses = await DynamicSelect(ConnectionStatusDAOs, filter);
            return ConnectionStatuses;
        }

        public async Task<List<ConnectionStatus>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ConnectionStatusDAO> query = DataContext.ConnectionStatus.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<ConnectionStatus> ConnectionStatuses = await query.AsNoTracking()
            .Select(x => new ConnectionStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return ConnectionStatuses;
        }

    }
}
