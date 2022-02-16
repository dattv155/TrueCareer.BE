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
    public interface IMentorConnectionRepository
    {
        Task<int> CountAll(MentorConnectionFilter MentorConnectionFilter);
        Task<int> Count(MentorConnectionFilter MentorConnectionFilter);
        Task<List<MentorConnection>> List(MentorConnectionFilter MentorConnectionFilter);
        Task<List<MentorConnection>> List(List<long> Ids);
        Task<MentorConnection> Get(long Id);
        Task<bool> Create(MentorConnection MentorConnection);
        Task<bool> Update(MentorConnection MentorConnection);
        Task<bool> Delete(MentorConnection MentorConnection);
        Task<bool> BulkMerge(List<MentorConnection> MentorConnections);
        Task<bool> BulkDelete(List<MentorConnection> MentorConnections);
    }
    public class MentorConnectionRepository : IMentorConnectionRepository
    {
        private DataContext DataContext;
        public MentorConnectionRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<MentorConnectionDAO>> DynamicFilter(IQueryable<MentorConnectionDAO> query, MentorConnectionFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Url, filter.Url);
            query = query.Where(q => q.ConnectionTypeId, filter.ConnectionTypeId);
            query = query.Where(q => q.MentorId, filter.MentorId);
            return query;
        }

        private async Task<IQueryable<MentorConnectionDAO>> OrFilter(IQueryable<MentorConnectionDAO> query, MentorConnectionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MentorConnectionDAO> initQuery = query.Where(q => false);
            foreach (MentorConnectionFilter MentorConnectionFilter in filter.OrFilter)
            {
                IQueryable<MentorConnectionDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, MentorConnectionFilter.Id);
                queryable = queryable.Where(q => q.Url, MentorConnectionFilter.Url);
                queryable = queryable.Where(q => q.ConnectionTypeId, MentorConnectionFilter.ConnectionTypeId);
                queryable = queryable.Where(q => q.MentorId, MentorConnectionFilter.MentorId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<MentorConnectionDAO> DynamicOrder(IQueryable<MentorConnectionDAO> query, MentorConnectionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MentorConnectionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case MentorConnectionOrder.Mentor:
                            query = query.OrderBy(q => q.MentorId);
                            break;
                        case MentorConnectionOrder.Url:
                            query = query.OrderBy(q => q.Url);
                            break;
                        case MentorConnectionOrder.ConnectionType:
                            query = query.OrderBy(q => q.ConnectionTypeId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MentorConnectionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case MentorConnectionOrder.Mentor:
                            query = query.OrderByDescending(q => q.MentorId);
                            break;
                        case MentorConnectionOrder.Url:
                            query = query.OrderByDescending(q => q.Url);
                            break;
                        case MentorConnectionOrder.ConnectionType:
                            query = query.OrderByDescending(q => q.ConnectionTypeId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<MentorConnection>> DynamicSelect(IQueryable<MentorConnectionDAO> query, MentorConnectionFilter filter)
        {
            List<MentorConnection> MentorConnections = await query.Select(q => new MentorConnection()
            {
                Id = filter.Selects.Contains(MentorConnectionSelect.Id) ? q.Id : default(long),
                MentorId = filter.Selects.Contains(MentorConnectionSelect.Mentor) ? q.MentorId : default(long),
                Url = filter.Selects.Contains(MentorConnectionSelect.Url) ? q.Url : default(string),
                ConnectionTypeId = filter.Selects.Contains(MentorConnectionSelect.ConnectionType) ? q.ConnectionTypeId : default(long),
                ConnectionType = filter.Selects.Contains(MentorConnectionSelect.ConnectionType) && q.ConnectionType != null ? new ConnectionType
                {
                    Id = q.ConnectionType.Id,
                    Name = q.ConnectionType.Name,
                    Code = q.ConnectionType.Code,
                } : null,
                Mentor = filter.Selects.Contains(MentorConnectionSelect.Mentor) && q.Mentor != null ? new AppUser
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
            return MentorConnections;
        }

        public async Task<int> CountAll(MentorConnectionFilter filter)
        {
            IQueryable<MentorConnectionDAO> MentorConnectionDAOs = DataContext.MentorConnection.AsNoTracking();
            MentorConnectionDAOs = await DynamicFilter(MentorConnectionDAOs, filter);
            return await MentorConnectionDAOs.CountAsync();
        }

        public async Task<int> Count(MentorConnectionFilter filter)
        {
            IQueryable<MentorConnectionDAO> MentorConnectionDAOs = DataContext.MentorConnection.AsNoTracking();
            MentorConnectionDAOs = await DynamicFilter(MentorConnectionDAOs, filter);
            MentorConnectionDAOs = await OrFilter(MentorConnectionDAOs, filter);
            return await MentorConnectionDAOs.CountAsync();
        }

        public async Task<List<MentorConnection>> List(MentorConnectionFilter filter)
        {
            if (filter == null) return new List<MentorConnection>();
            IQueryable<MentorConnectionDAO> MentorConnectionDAOs = DataContext.MentorConnection.AsNoTracking();
            MentorConnectionDAOs = await DynamicFilter(MentorConnectionDAOs, filter);
            MentorConnectionDAOs = await OrFilter(MentorConnectionDAOs, filter);
            MentorConnectionDAOs = DynamicOrder(MentorConnectionDAOs, filter);
            List<MentorConnection> MentorConnections = await DynamicSelect(MentorConnectionDAOs, filter);
            return MentorConnections;
        }

        public async Task<List<MentorConnection>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MentorConnectionDAO> query = DataContext.MentorConnection.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<MentorConnection> MentorConnections = await query.AsNoTracking()
            .Select(x => new MentorConnection()
            {
                Id = x.Id,
                MentorId = x.MentorId,
                Url = x.Url,
                ConnectionTypeId = x.ConnectionTypeId,
                ConnectionType = x.ConnectionType == null ? null : new ConnectionType
                {
                    Id = x.ConnectionType.Id,
                    Name = x.ConnectionType.Name,
                    Code = x.ConnectionType.Code,
                },
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
            

            return MentorConnections;
        }

        public async Task<MentorConnection> Get(long Id)
        {
            MentorConnection MentorConnection = await DataContext.MentorConnection.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new MentorConnection()
            {
                Id = x.Id,
                MentorId = x.MentorId,
                Url = x.Url,
                ConnectionTypeId = x.ConnectionTypeId,
                ConnectionType = x.ConnectionType == null ? null : new ConnectionType
                {
                    Id = x.ConnectionType.Id,
                    Name = x.ConnectionType.Name,
                    Code = x.ConnectionType.Code,
                },
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

            if (MentorConnection == null)
                return null;

            return MentorConnection;
        }
        
        public async Task<bool> Create(MentorConnection MentorConnection)
        {
            MentorConnectionDAO MentorConnectionDAO = new MentorConnectionDAO();
            MentorConnectionDAO.Id = MentorConnection.Id;
            MentorConnectionDAO.MentorId = MentorConnection.MentorId;
            MentorConnectionDAO.Url = MentorConnection.Url;
            MentorConnectionDAO.ConnectionTypeId = MentorConnection.ConnectionTypeId;
            DataContext.MentorConnection.Add(MentorConnectionDAO);
            await DataContext.SaveChangesAsync();
            MentorConnection.Id = MentorConnectionDAO.Id;
            await SaveReference(MentorConnection);
            return true;
        }

        public async Task<bool> Update(MentorConnection MentorConnection)
        {
            MentorConnectionDAO MentorConnectionDAO = DataContext.MentorConnection
                .Where(x => x.Id == MentorConnection.Id)
                .FirstOrDefault();
            if (MentorConnectionDAO == null)
                return false;
            MentorConnectionDAO.Id = MentorConnection.Id;
            MentorConnectionDAO.MentorId = MentorConnection.MentorId;
            MentorConnectionDAO.Url = MentorConnection.Url;
            MentorConnectionDAO.ConnectionTypeId = MentorConnection.ConnectionTypeId;
            await DataContext.SaveChangesAsync();
            await SaveReference(MentorConnection);
            return true;
        }

        public async Task<bool> Delete(MentorConnection MentorConnection)
        {
            await DataContext.MentorConnection
                .Where(x => x.Id == MentorConnection.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<MentorConnection> MentorConnections)
        {
            IdFilter IdFilter = new IdFilter { In = MentorConnections.Select(x => x.Id).ToList() };
            List<MentorConnectionDAO> MentorConnectionDAOs = new List<MentorConnectionDAO>();
            List<MentorConnectionDAO> DbMentorConnectionDAOs = await DataContext.MentorConnection
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (MentorConnection MentorConnection in MentorConnections)
            {
                MentorConnectionDAO MentorConnectionDAO = DbMentorConnectionDAOs
                        .Where(x => x.Id == MentorConnection.Id)
                        .FirstOrDefault();
                if (MentorConnectionDAO == null)
                {
                    MentorConnectionDAO = new MentorConnectionDAO();
                }
                MentorConnectionDAO.MentorId = MentorConnection.MentorId;
                MentorConnectionDAO.Url = MentorConnection.Url;
                MentorConnectionDAO.ConnectionTypeId = MentorConnection.ConnectionTypeId;
                MentorConnectionDAOs.Add(MentorConnectionDAO);
            }
            await DataContext.BulkMergeAsync(MentorConnectionDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<MentorConnection> MentorConnections)
        {
            List<long> Ids = MentorConnections.Select(x => x.Id).ToList();
            await DataContext.MentorConnection
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(MentorConnection MentorConnection)
        {
        }
        
    }
}
