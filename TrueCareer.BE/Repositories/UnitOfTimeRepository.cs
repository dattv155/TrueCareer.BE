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
    public interface IUnitOfTimeRepository
    {
        Task<int> CountAll(UnitOfTimeFilter UnitOfTimeFilter);
        Task<int> Count(UnitOfTimeFilter UnitOfTimeFilter);
        Task<List<UnitOfTime>> List(UnitOfTimeFilter UnitOfTimeFilter);
        Task<List<UnitOfTime>> List(List<long> Ids);
        Task<UnitOfTime> Get(long Id);
        Task<bool> Create(UnitOfTime UnitOfTime);
        Task<bool> Update(UnitOfTime UnitOfTime);
        Task<bool> Delete(UnitOfTime UnitOfTime);
        Task<bool> BulkMerge(List<UnitOfTime> UnitOfTimes);
        Task<bool> BulkDelete(List<UnitOfTime> UnitOfTimes);
    }
    public class UnitOfTimeRepository : IUnitOfTimeRepository
    {
        private DataContext DataContext;
        public UnitOfTimeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<UnitOfTimeDAO>> DynamicFilter(IQueryable<UnitOfTimeDAO> query, UnitOfTimeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.StartAt, filter.StartAt);
            query = query.Where(q => q.EndAt, filter.EndAt);
            return query;
        }

        private async Task<IQueryable<UnitOfTimeDAO>> OrFilter(IQueryable<UnitOfTimeDAO> query, UnitOfTimeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<UnitOfTimeDAO> initQuery = query.Where(q => false);
            foreach (UnitOfTimeFilter UnitOfTimeFilter in filter.OrFilter)
            {
                IQueryable<UnitOfTimeDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, UnitOfTimeFilter.Id);
                queryable = queryable.Where(q => q.Code, UnitOfTimeFilter.Code);
                queryable = queryable.Where(q => q.Name, UnitOfTimeFilter.Name);
                queryable = queryable.Where(q => q.StartAt, UnitOfTimeFilter.StartAt);
                queryable = queryable.Where(q => q.EndAt, UnitOfTimeFilter.EndAt);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<UnitOfTimeDAO> DynamicOrder(IQueryable<UnitOfTimeDAO> query, UnitOfTimeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case UnitOfTimeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case UnitOfTimeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case UnitOfTimeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case UnitOfTimeOrder.StartAt:
                            query = query.OrderBy(q => q.StartAt);
                            break;
                        case UnitOfTimeOrder.EndAt:
                            query = query.OrderBy(q => q.EndAt);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case UnitOfTimeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case UnitOfTimeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case UnitOfTimeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case UnitOfTimeOrder.StartAt:
                            query = query.OrderByDescending(q => q.StartAt);
                            break;
                        case UnitOfTimeOrder.EndAt:
                            query = query.OrderByDescending(q => q.EndAt);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<UnitOfTime>> DynamicSelect(IQueryable<UnitOfTimeDAO> query, UnitOfTimeFilter filter)
        {
            List<UnitOfTime> UnitOfTimes = await query.Select(q => new UnitOfTime()
            {
                Id = filter.Selects.Contains(UnitOfTimeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(UnitOfTimeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(UnitOfTimeSelect.Name) ? q.Name : default(string),
                StartAt = filter.Selects.Contains(UnitOfTimeSelect.StartAt) ? q.StartAt : default(long?),
                EndAt = filter.Selects.Contains(UnitOfTimeSelect.EndAt) ? q.EndAt : default(long?),
            }).ToListAsync();
            return UnitOfTimes;
        }

        public async Task<int> CountAll(UnitOfTimeFilter filter)
        {
            IQueryable<UnitOfTimeDAO> UnitOfTimeDAOs = DataContext.UnitOfTime.AsNoTracking();
            UnitOfTimeDAOs = await DynamicFilter(UnitOfTimeDAOs, filter);
            return await UnitOfTimeDAOs.CountAsync();
        }

        public async Task<int> Count(UnitOfTimeFilter filter)
        {
            IQueryable<UnitOfTimeDAO> UnitOfTimeDAOs = DataContext.UnitOfTime.AsNoTracking();
            UnitOfTimeDAOs = await DynamicFilter(UnitOfTimeDAOs, filter);
            UnitOfTimeDAOs = await OrFilter(UnitOfTimeDAOs, filter);
            return await UnitOfTimeDAOs.CountAsync();
        }

        public async Task<List<UnitOfTime>> List(UnitOfTimeFilter filter)
        {
            if (filter == null) return new List<UnitOfTime>();
            IQueryable<UnitOfTimeDAO> UnitOfTimeDAOs = DataContext.UnitOfTime.AsNoTracking();
            UnitOfTimeDAOs = await DynamicFilter(UnitOfTimeDAOs, filter);
            UnitOfTimeDAOs = await OrFilter(UnitOfTimeDAOs, filter);
            UnitOfTimeDAOs = DynamicOrder(UnitOfTimeDAOs, filter);
            List<UnitOfTime> UnitOfTimes = await DynamicSelect(UnitOfTimeDAOs, filter);
            return UnitOfTimes;
        }

        public async Task<List<UnitOfTime>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<UnitOfTimeDAO> query = DataContext.UnitOfTime.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<UnitOfTime> UnitOfTimes = await query.AsNoTracking()
            .Select(x => new UnitOfTime()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StartAt = x.StartAt,
                EndAt = x.EndAt,
            }).ToListAsync();
            

            return UnitOfTimes;
        }

        public async Task<UnitOfTime> Get(long Id)
        {
            UnitOfTime UnitOfTime = await DataContext.UnitOfTime.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new UnitOfTime()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StartAt = x.StartAt,
                EndAt = x.EndAt,
            }).FirstOrDefaultAsync();

            if (UnitOfTime == null)
                return null;

            return UnitOfTime;
        }
        
        public async Task<bool> Create(UnitOfTime UnitOfTime)
        {
            UnitOfTimeDAO UnitOfTimeDAO = new UnitOfTimeDAO();
            UnitOfTimeDAO.Id = UnitOfTime.Id;
            UnitOfTimeDAO.Code = UnitOfTime.Code;
            UnitOfTimeDAO.Name = UnitOfTime.Name;
            UnitOfTimeDAO.StartAt = UnitOfTime.StartAt;
            UnitOfTimeDAO.EndAt = UnitOfTime.EndAt;
            DataContext.UnitOfTime.Add(UnitOfTimeDAO);
            await DataContext.SaveChangesAsync();
            UnitOfTime.Id = UnitOfTimeDAO.Id;
            await SaveReference(UnitOfTime);
            return true;
        }

        public async Task<bool> Update(UnitOfTime UnitOfTime)
        {
            UnitOfTimeDAO UnitOfTimeDAO = DataContext.UnitOfTime
                .Where(x => x.Id == UnitOfTime.Id)
                .FirstOrDefault();
            if (UnitOfTimeDAO == null)
                return false;
            UnitOfTimeDAO.Id = UnitOfTime.Id;
            UnitOfTimeDAO.Code = UnitOfTime.Code;
            UnitOfTimeDAO.Name = UnitOfTime.Name;
            UnitOfTimeDAO.StartAt = UnitOfTime.StartAt;
            UnitOfTimeDAO.EndAt = UnitOfTime.EndAt;
            await DataContext.SaveChangesAsync();
            await SaveReference(UnitOfTime);
            return true;
        }

        public async Task<bool> Delete(UnitOfTime UnitOfTime)
        {
            await DataContext.UnitOfTime
                .Where(x => x.Id == UnitOfTime.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<UnitOfTime> UnitOfTimes)
        {
            IdFilter IdFilter = new IdFilter { In = UnitOfTimes.Select(x => x.Id).ToList() };
            List<UnitOfTimeDAO> UnitOfTimeDAOs = new List<UnitOfTimeDAO>();
            List<UnitOfTimeDAO> DbUnitOfTimeDAOs = await DataContext.UnitOfTime
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (UnitOfTime UnitOfTime in UnitOfTimes)
            {
                UnitOfTimeDAO UnitOfTimeDAO = DbUnitOfTimeDAOs
                        .Where(x => x.Id == UnitOfTime.Id)
                        .FirstOrDefault();
                if (UnitOfTimeDAO == null)
                {
                    UnitOfTimeDAO = new UnitOfTimeDAO();
                }
                UnitOfTimeDAO.Code = UnitOfTime.Code;
                UnitOfTimeDAO.Name = UnitOfTime.Name;
                UnitOfTimeDAO.StartAt = UnitOfTime.StartAt;
                UnitOfTimeDAO.EndAt = UnitOfTime.EndAt;
                UnitOfTimeDAOs.Add(UnitOfTimeDAO);
            }
            await DataContext.BulkMergeAsync(UnitOfTimeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<UnitOfTime> UnitOfTimes)
        {
            List<long> Ids = UnitOfTimes.Select(x => x.Id).ToList();
            await DataContext.UnitOfTime
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(UnitOfTime UnitOfTime)
        {
        }
        
    }
}
