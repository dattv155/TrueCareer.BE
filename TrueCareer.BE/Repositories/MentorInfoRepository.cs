using TrueSight.Common;
using TrueCareer.Entities;
using TrueCareer.BE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using TrueCareer.BE.Entities;

namespace TrueCareer.Repositories
{
    public interface IMentorInfoRepository
    {
        Task<int> CountAll(MentorInfoFilter MentorInfoFilter);
        Task<int> Count(MentorInfoFilter MentorInfoFilter);
        Task<List<MentorInfo>> List(MentorInfoFilter MentorInfoFilter);
        Task<List<MentorInfo>> List(List<long> Ids);
        Task<MentorInfo> Get(long Id);
        Task<bool> Create(MentorInfo MentorInfo);
        Task<bool> Update(MentorInfo MentorInfo);
        Task<bool> Delete(MentorInfo MentorInfo);
        Task<bool> BulkMerge(List<MentorInfo> MentorInfos);
        Task<bool> BulkDelete(List<MentorInfo> MentorInfos);
    }
    public class MentorInfoRepository : IMentorInfoRepository
    {
        private DataContext DataContext;
        public MentorInfoRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<MentorInfoDAO>> DynamicFilter(IQueryable<MentorInfoDAO> query, MentorInfoFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.AppUserId, filter.AppUserId);
            query = query.Where(q => q.MajorId, filter.MajorId);
            return query;
        }

        private IQueryable<MentorInfoDAO> DynamicOrder(IQueryable<MentorInfoDAO> query, MentorInfoFilter filter)
        {
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<MentorInfo>> DynamicSelect(IQueryable<MentorInfoDAO> query, MentorInfoFilter filter)
        {
            List<MentorInfo> MentorInfos = await query.Select(q => new MentorInfo()
            {
                Id = q.Id,
                AppUserId = q.AppUserId,
                ConnectionId = q.ConnectionId,
                ConnectionUrl = q.ConnectionUrl,
                MajorId = q.MajorId,
                TopicDescription = q.TopicDescription,
            }).ToListAsync();

            List<long> AppUserIds = await query.Select(q => q.Id).ToListAsync();
            List<ActiveTime> AppUserActiveTimes = await DataContext.ActiveTime.AsNoTracking()
                .Where(x => AppUserIds.Contains(x.MentorId))
                .Select(x => new ActiveTime
                {
                    ActiveDate = x.ActiveDate,
                    MentorId = x.MentorId,
                    UnitOfTimeId = x.UnitOfTimeId,
                    UnitOfTime = new UnitOfTime
                    {
                        Code = x.UnitOfTime.Code,
                        Name = x.UnitOfTime.Name,
                        StartAt = x.UnitOfTime.StartAt,
                        EndAt = x.UnitOfTime.EndAt,
                    }
                }).ToListAsync();

            foreach (MentorInfo m in MentorInfos)
            {
                m.ActiveTimes = AppUserActiveTimes
                    .Where(x => x.MentorId == m.AppUserId)
                    .ToList();
            }
            return MentorInfos;
        }

        public async Task<int> CountAll(MentorInfoFilter filter)
        {
            IQueryable<MentorInfoDAO> MentorInfoDAOs = DataContext.MentorInfo.AsNoTracking();
            MentorInfoDAOs = await DynamicFilter(MentorInfoDAOs, filter);
            return await MentorInfoDAOs.CountAsync();
        }

        public async Task<int> Count(MentorInfoFilter filter)
        {
            IQueryable<MentorInfoDAO> MentorInfoDAOs = DataContext.MentorInfo.AsNoTracking();
            MentorInfoDAOs = await DynamicFilter(MentorInfoDAOs, filter);
            return await MentorInfoDAOs.CountAsync();
        }

        public async Task<List<MentorInfo>> List(MentorInfoFilter filter)
        {
            if (filter == null) return new List<MentorInfo>();
            IQueryable<MentorInfoDAO> MentorInfoDAOs = DataContext.MentorInfo.AsNoTracking();
            MentorInfoDAOs = await DynamicFilter(MentorInfoDAOs, filter);
            MentorInfoDAOs = DynamicOrder(MentorInfoDAOs, filter);
            List<MentorInfo> MentorInfos = await DynamicSelect(MentorInfoDAOs, filter);
            return MentorInfos;
        }

        public async Task<List<MentorInfo>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MentorInfoDAO> query = DataContext.MentorInfo.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<MentorInfo> MentorInfos = await query.AsNoTracking()
            .Select(x => new MentorInfo()
            {
                Id = x.Id,
                AppUserId = x.AppUserId,
                ConnectionId = x.ConnectionId,
                ConnectionUrl = x.ConnectionUrl,
                MajorId = x.MajorId,
                TopicDescription = x.TopicDescription,
            }).ToListAsync();

            return MentorInfos;
        }

        public async Task<MentorInfo> Get(long AppUserId)
        {
            MentorInfo MentorInfo = await DataContext.MentorInfo.AsNoTracking()
            .Where(x => x.AppUserId == AppUserId)
            .Select(x => new MentorInfo()
            {
                Id = x.Id,
                AppUserId = x.AppUserId,
                ConnectionId = x.ConnectionId,
                ConnectionUrl = x.ConnectionUrl,
                MajorId = x.MajorId,
                TopicDescription = x.TopicDescription,
            }).FirstOrDefaultAsync();

            if (MentorInfo == null)
                return null;

            return MentorInfo;
        }

        public async Task<bool> Create(MentorInfo MentorInfo)
        {
            MentorInfoDAO MentorInfoDAO = new MentorInfoDAO();
            MentorInfoDAO.Id = MentorInfo.Id;
            MentorInfoDAO.AppUserId = MentorInfo.AppUserId;
            MentorInfoDAO.ConnectionId = MentorInfo.ConnectionId;
            MentorInfoDAO.ConnectionUrl = MentorInfo.ConnectionUrl;
            MentorInfoDAO.MajorId = MentorInfo.MajorId;
            MentorInfoDAO.TopicDescription = MentorInfo.TopicDescription;
            DataContext.MentorInfo.Add(MentorInfoDAO);
            await DataContext.SaveChangesAsync();
            MentorInfo.Id = MentorInfoDAO.Id;
            await SaveReference(MentorInfo);
            return true;
        }

        public async Task<bool> Update(MentorInfo MentorInfo)
        {
            MentorInfoDAO MentorInfoDAO = DataContext.MentorInfo
                .Where(x => x.Id == MentorInfo.Id)
                .FirstOrDefault();
            if (MentorInfoDAO == null)
                return false;
            MentorInfoDAO.Id = MentorInfo.Id;
            MentorInfoDAO.AppUserId = MentorInfo.AppUserId;
            MentorInfoDAO.ConnectionId = MentorInfo.ConnectionId;
            MentorInfoDAO.ConnectionUrl = MentorInfo.ConnectionUrl;
            MentorInfoDAO.MajorId = MentorInfo.MajorId;
            MentorInfoDAO.TopicDescription = MentorInfo.TopicDescription;
            await DataContext.SaveChangesAsync();
            await SaveReference(MentorInfo);
            return true;
        }

        public async Task<bool> Delete(MentorInfo MentorInfo)
        {
            await DataContext.MentorInfo
                .Where(x => x.Id == MentorInfo.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<MentorInfo> MentorInfos)
        {
            IdFilter IdFilter = new IdFilter { In = MentorInfos.Select(x => x.Id).ToList() };
            List<MentorInfoDAO> MentorInfoDAOs = new List<MentorInfoDAO>();
            List<MentorInfoDAO> DbMentorInfoDAOs = await DataContext.MentorInfo
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (MentorInfo MentorInfo in MentorInfos)
            {
                MentorInfoDAO MentorInfoDAO = DbMentorInfoDAOs
                        .Where(x => x.Id == MentorInfo.Id)
                        .FirstOrDefault();
                if (MentorInfoDAO == null)
                {
                    MentorInfoDAO = new MentorInfoDAO();
                }
                MentorInfoDAO.AppUserId = MentorInfo.AppUserId;
                MentorInfoDAO.ConnectionId = MentorInfo.ConnectionId;
                MentorInfoDAO.ConnectionUrl = MentorInfo.ConnectionUrl;
                MentorInfoDAO.MajorId = MentorInfo.MajorId;
                MentorInfoDAO.TopicDescription = MentorInfo.TopicDescription;
                MentorInfoDAOs.Add(MentorInfoDAO);
            }
            await DataContext.BulkMergeAsync(MentorInfoDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<MentorInfo> MentorInfos)
        {
            List<long> Ids = MentorInfos.Select(x => x.Id).ToList();
            await DataContext.MentorInfo
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(MentorInfo MentorInfo)
        {
            List<ActiveTimeDAO> ActiveTimeDAOs = new List<ActiveTimeDAO>();
            if (MentorInfo.ActiveTimes != null)
            {
                foreach (ActiveTime ActiveTime in MentorInfo.ActiveTimes)
                {
                    ActiveTimeDAO ActiveTimeDAO = new ActiveTimeDAO();
                    ActiveTimeDAO.MentorId = MentorInfo.AppUserId;
                    ActiveTimeDAO.ActiveDate = ActiveTime.ActiveDate;
                    ActiveTimeDAO.UnitOfTimeId = ActiveTime.UnitOfTimeId;
                    ActiveTimeDAOs.Add(ActiveTimeDAO);
                }
                await DataContext.ActiveTime.BulkMergeAsync(ActiveTimeDAOs);
            }
        }

    }
}

