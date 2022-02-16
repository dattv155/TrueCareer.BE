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
    public interface IActiveTimeRepository
    {
        Task<int> CountAll(ActiveTimeFilter ActiveTimeFilter);
        Task<int> Count(ActiveTimeFilter ActiveTimeFilter);
        Task<List<ActiveTime>> List(ActiveTimeFilter ActiveTimeFilter);
        Task<List<ActiveTime>> List(List<long> Ids);
        Task<ActiveTime> Get(long Id);
        Task<bool> Create(ActiveTime ActiveTime);
        Task<bool> Update(ActiveTime ActiveTime);
        Task<bool> Delete(ActiveTime ActiveTime);
        Task<bool> BulkMerge(List<ActiveTime> ActiveTimes);
        Task<bool> BulkDelete(List<ActiveTime> ActiveTimes);
    }
    public class ActiveTimeRepository : IActiveTimeRepository
    {
        private DataContext DataContext;
        public ActiveTimeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ActiveTimeDAO>> DynamicFilter(IQueryable<ActiveTimeDAO> query, ActiveTimeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.StartAt, filter.StartAt);
            query = query.Where(q => q.EndAt, filter.EndAt);
            query = query.Where(q => q.MentorId, filter.MentorId);
            return query;
        }

        private async Task<IQueryable<ActiveTimeDAO>> OrFilter(IQueryable<ActiveTimeDAO> query, ActiveTimeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ActiveTimeDAO> initQuery = query.Where(q => false);
            foreach (ActiveTimeFilter ActiveTimeFilter in filter.OrFilter)
            {
                IQueryable<ActiveTimeDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ActiveTimeFilter.Id);
                queryable = queryable.Where(q => q.StartAt, ActiveTimeFilter.StartAt);
                queryable = queryable.Where(q => q.EndAt, ActiveTimeFilter.EndAt);
                queryable = queryable.Where(q => q.MentorId, ActiveTimeFilter.MentorId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ActiveTimeDAO> DynamicOrder(IQueryable<ActiveTimeDAO> query, ActiveTimeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ActiveTimeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ActiveTimeOrder.StartAt:
                            query = query.OrderBy(q => q.StartAt);
                            break;
                        case ActiveTimeOrder.EndAt:
                            query = query.OrderBy(q => q.EndAt);
                            break;
                        case ActiveTimeOrder.Mentor:
                            query = query.OrderBy(q => q.MentorId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ActiveTimeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ActiveTimeOrder.StartAt:
                            query = query.OrderByDescending(q => q.StartAt);
                            break;
                        case ActiveTimeOrder.EndAt:
                            query = query.OrderByDescending(q => q.EndAt);
                            break;
                        case ActiveTimeOrder.Mentor:
                            query = query.OrderByDescending(q => q.MentorId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ActiveTime>> DynamicSelect(IQueryable<ActiveTimeDAO> query, ActiveTimeFilter filter)
        {
            List<ActiveTime> ActiveTimes = await query.Select(q => new ActiveTime()
            {
                Id = filter.Selects.Contains(ActiveTimeSelect.Id) ? q.Id : default(long),
                StartAt = filter.Selects.Contains(ActiveTimeSelect.StartAt) ? q.StartAt : default(DateTime),
                EndAt = filter.Selects.Contains(ActiveTimeSelect.EndAt) ? q.EndAt : default(DateTime),
                MentorId = filter.Selects.Contains(ActiveTimeSelect.Mentor) ? q.MentorId : default(long),
                Mentor = filter.Selects.Contains(ActiveTimeSelect.Mentor) && q.Mentor != null ? new AppUser
                {
                    Id = q.Mentor.Id,
                    Username = q.Mentor.Username,
                    Email = q.Mentor.Email,
                    Phone = q.Mentor.Phone,
                    Password = q.Mentor.Password,
                    DisplayName = q.Mentor.DisplayName,
                    SexId = q.Mentor.SexId,
                    Birthday = q.Mentor.Birthday,
                    Avatar = q.Mentor.Avatar,
                    CoverImage = q.Mentor.CoverImage,
                } : null,
            }).ToListAsync();
            return ActiveTimes;
        }

        public async Task<int> CountAll(ActiveTimeFilter filter)
        {
            IQueryable<ActiveTimeDAO> ActiveTimeDAOs = DataContext.ActiveTime.AsNoTracking();
            ActiveTimeDAOs = await DynamicFilter(ActiveTimeDAOs, filter);
            return await ActiveTimeDAOs.CountAsync();
        }

        public async Task<int> Count(ActiveTimeFilter filter)
        {
            IQueryable<ActiveTimeDAO> ActiveTimeDAOs = DataContext.ActiveTime.AsNoTracking();
            ActiveTimeDAOs = await DynamicFilter(ActiveTimeDAOs, filter);
            ActiveTimeDAOs = await OrFilter(ActiveTimeDAOs, filter);
            return await ActiveTimeDAOs.CountAsync();
        }

        public async Task<List<ActiveTime>> List(ActiveTimeFilter filter)
        {
            if (filter == null) return new List<ActiveTime>();
            IQueryable<ActiveTimeDAO> ActiveTimeDAOs = DataContext.ActiveTime.AsNoTracking();
            ActiveTimeDAOs = await DynamicFilter(ActiveTimeDAOs, filter);
            ActiveTimeDAOs = await OrFilter(ActiveTimeDAOs, filter);
            ActiveTimeDAOs = DynamicOrder(ActiveTimeDAOs, filter);
            List<ActiveTime> ActiveTimes = await DynamicSelect(ActiveTimeDAOs, filter);
            return ActiveTimes;
        }

        public async Task<List<ActiveTime>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ActiveTimeDAO> query = DataContext.ActiveTime.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<ActiveTime> ActiveTimes = await query.AsNoTracking()
            .Select(x => new ActiveTime()
            {
                Id = x.Id,
                StartAt = x.StartAt,
                EndAt = x.EndAt,
                MentorId = x.MentorId,
                Mentor = x.Mentor == null ? null : new AppUser
                {
                    Id = x.Mentor.Id,
                    Username = x.Mentor.Username,
                    Email = x.Mentor.Email,
                    Phone = x.Mentor.Phone,
                    Password = x.Mentor.Password,
                    DisplayName = x.Mentor.DisplayName,
                    SexId = x.Mentor.SexId,
                    Birthday = x.Mentor.Birthday,
                    Avatar = x.Mentor.Avatar,
                    CoverImage = x.Mentor.CoverImage,
                },
            }).ToListAsync();
            

            return ActiveTimes;
        }

        public async Task<ActiveTime> Get(long Id)
        {
            ActiveTime ActiveTime = await DataContext.ActiveTime.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new ActiveTime()
            {
                Id = x.Id,
                StartAt = x.StartAt,
                EndAt = x.EndAt,
                MentorId = x.MentorId,
                Mentor = x.Mentor == null ? null : new AppUser
                {
                    Id = x.Mentor.Id,
                    Username = x.Mentor.Username,
                    Email = x.Mentor.Email,
                    Phone = x.Mentor.Phone,
                    Password = x.Mentor.Password,
                    DisplayName = x.Mentor.DisplayName,
                    SexId = x.Mentor.SexId,
                    Birthday = x.Mentor.Birthday,
                    Avatar = x.Mentor.Avatar,
                    CoverImage = x.Mentor.CoverImage,
                },
            }).FirstOrDefaultAsync();

            if (ActiveTime == null)
                return null;

            return ActiveTime;
        }
        
        public async Task<bool> Create(ActiveTime ActiveTime)
        {
            ActiveTimeDAO ActiveTimeDAO = new ActiveTimeDAO();
            ActiveTimeDAO.Id = ActiveTime.Id;
            ActiveTimeDAO.StartAt = ActiveTime.StartAt;
            ActiveTimeDAO.EndAt = ActiveTime.EndAt;
            ActiveTimeDAO.MentorId = ActiveTime.MentorId;
            DataContext.ActiveTime.Add(ActiveTimeDAO);
            await DataContext.SaveChangesAsync();
            ActiveTime.Id = ActiveTimeDAO.Id;
            await SaveReference(ActiveTime);
            return true;
        }

        public async Task<bool> Update(ActiveTime ActiveTime)
        {
            ActiveTimeDAO ActiveTimeDAO = DataContext.ActiveTime
                .Where(x => x.Id == ActiveTime.Id)
                .FirstOrDefault();
            if (ActiveTimeDAO == null)
                return false;
            ActiveTimeDAO.Id = ActiveTime.Id;
            ActiveTimeDAO.StartAt = ActiveTime.StartAt;
            ActiveTimeDAO.EndAt = ActiveTime.EndAt;
            ActiveTimeDAO.MentorId = ActiveTime.MentorId;
            await DataContext.SaveChangesAsync();
            await SaveReference(ActiveTime);
            return true;
        }

        public async Task<bool> Delete(ActiveTime ActiveTime)
        {
            await DataContext.ActiveTime
                .Where(x => x.Id == ActiveTime.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ActiveTime> ActiveTimes)
        {
            IdFilter IdFilter = new IdFilter { In = ActiveTimes.Select(x => x.Id).ToList() };
            List<ActiveTimeDAO> ActiveTimeDAOs = new List<ActiveTimeDAO>();
            List<ActiveTimeDAO> DbActiveTimeDAOs = await DataContext.ActiveTime
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (ActiveTime ActiveTime in ActiveTimes)
            {
                ActiveTimeDAO ActiveTimeDAO = DbActiveTimeDAOs
                        .Where(x => x.Id == ActiveTime.Id)
                        .FirstOrDefault();
                if (ActiveTimeDAO == null)
                {
                    ActiveTimeDAO = new ActiveTimeDAO();
                }
                ActiveTimeDAO.StartAt = ActiveTime.StartAt;
                ActiveTimeDAO.EndAt = ActiveTime.EndAt;
                ActiveTimeDAO.MentorId = ActiveTime.MentorId;
                ActiveTimeDAOs.Add(ActiveTimeDAO);
            }
            await DataContext.BulkMergeAsync(ActiveTimeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ActiveTime> ActiveTimes)
        {
            List<long> Ids = ActiveTimes.Select(x => x.Id).ToList();
            await DataContext.ActiveTime
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(ActiveTime ActiveTime)
        {
        }
        
    }
}
