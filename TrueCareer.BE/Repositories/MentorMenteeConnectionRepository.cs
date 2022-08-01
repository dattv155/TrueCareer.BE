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
    public interface IMentorMenteeConnectionRepository
    {
        Task<int> CountAll(MentorMenteeConnectionFilter MentorMenteeConnectionFilter);
        Task<int> Count(MentorMenteeConnectionFilter MentorMenteeConnectionFilter);
        Task<List<MentorMenteeConnection>> List(MentorMenteeConnectionFilter MentorMenteeConnectionFilter);
        Task<List<MentorMenteeConnection>> List(List<long> Ids);
        Task<MentorMenteeConnection> Get(long Id);
        Task<bool> Create(MentorMenteeConnection MentorMenteeConnection);
        Task<bool> Update(MentorMenteeConnection MentorMenteeConnection);
        Task<bool> Delete(MentorMenteeConnection MentorMenteeConnection);
        Task<bool> BulkMerge(List<MentorMenteeConnection> MentorMenteeConnections);
        Task<bool> BulkDelete(List<MentorMenteeConnection> MentorMenteeConnections);
    }
    public class MentorMenteeConnectionRepository : IMentorMenteeConnectionRepository
    {
        private DataContext DataContext;
        public MentorMenteeConnectionRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<MentorMenteeConnectionDAO>> DynamicFilter(IQueryable<MentorMenteeConnectionDAO> query, MentorMenteeConnectionFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.FirstMessage, filter.FirstMessage);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.ConnectionId, filter.ConnectionId);
            query = query.Where(q => q.ConnectionStatusId, filter.ConnectionStatusId);
            query = query.Where(q => q.MenteeId, filter.MenteeId);
            query = query.Where(q => q.MentorId, filter.MentorId);
            return query;
        }

        private async Task<IQueryable<MentorMenteeConnectionDAO>> OrFilter(IQueryable<MentorMenteeConnectionDAO> query, MentorMenteeConnectionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MentorMenteeConnectionDAO> initQuery = query.Where(q => false);
            foreach (MentorMenteeConnectionFilter MentorMenteeConnectionFilter in filter.OrFilter)
            {
                IQueryable<MentorMenteeConnectionDAO> queryable = query;
                queryable = queryable.Where(q => q.FirstMessage, MentorMenteeConnectionFilter.FirstMessage);
                queryable = queryable.Where(q => q.Id, MentorMenteeConnectionFilter.Id);
                queryable = queryable.Where(q => q.ConnectionId, MentorMenteeConnectionFilter.ConnectionId);
                queryable = queryable.Where(q => q.ConnectionStatusId, MentorMenteeConnectionFilter.ConnectionStatusId);
                queryable = queryable.Where(q => q.MenteeId, MentorMenteeConnectionFilter.MenteeId);
                queryable = queryable.Where(q => q.MentorId, MentorMenteeConnectionFilter.MentorId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<MentorMenteeConnectionDAO> DynamicOrder(IQueryable<MentorMenteeConnectionDAO> query, MentorMenteeConnectionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MentorMenteeConnectionOrder.Mentor:
                            query = query.OrderBy(q => q.MentorId);
                            break;
                        case MentorMenteeConnectionOrder.Mentee:
                            query = query.OrderBy(q => q.MenteeId);
                            break;
                        case MentorMenteeConnectionOrder.Connection:
                            query = query.OrderBy(q => q.ConnectionId);
                            break;
                        case MentorMenteeConnectionOrder.FirstMessage:
                            query = query.OrderBy(q => q.FirstMessage);
                            break;
                        case MentorMenteeConnectionOrder.ConnectionStatus:
                            query = query.OrderBy(q => q.ConnectionStatusId);
                            break;
                        case MentorMenteeConnectionOrder.ActiveTime:
                            query = query.OrderBy(q => q.ActiveTimeId);
                            break;
                        case MentorMenteeConnectionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MentorMenteeConnectionOrder.Mentor:
                            query = query.OrderByDescending(q => q.MentorId);
                            break;
                        case MentorMenteeConnectionOrder.Mentee:
                            query = query.OrderByDescending(q => q.MenteeId);
                            break;
                        case MentorMenteeConnectionOrder.Connection:
                            query = query.OrderByDescending(q => q.ConnectionId);
                            break;
                        case MentorMenteeConnectionOrder.FirstMessage:
                            query = query.OrderByDescending(q => q.FirstMessage);
                            break;
                        case MentorMenteeConnectionOrder.ConnectionStatus:
                            query = query.OrderByDescending(q => q.ConnectionStatusId);
                            break;
                        case MentorMenteeConnectionOrder.ActiveTime:
                            query = query.OrderByDescending(q => q.ActiveTimeId);
                            break;
                        case MentorMenteeConnectionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<MentorMenteeConnection>> DynamicSelect(IQueryable<MentorMenteeConnectionDAO> query, MentorMenteeConnectionFilter filter)
        {
            List<MentorMenteeConnection> MentorMenteeConnections = await query.Select(q => new MentorMenteeConnection()
            {
                MentorId = filter.Selects.Contains(MentorMenteeConnectionSelect.Mentor) ? q.MentorId : default(long),
                MenteeId = filter.Selects.Contains(MentorMenteeConnectionSelect.Mentee) ? q.MenteeId : default(long),
                ConnectionId = filter.Selects.Contains(MentorMenteeConnectionSelect.Connection) ? q.ConnectionId : default(long),
                FirstMessage = filter.Selects.Contains(MentorMenteeConnectionSelect.FirstMessage) ? q.FirstMessage : default(string),
                ConnectionStatusId = filter.Selects.Contains(MentorMenteeConnectionSelect.ConnectionStatus) ? q.ConnectionStatusId : default(long),
                ActiveTimeId = filter.Selects.Contains(MentorMenteeConnectionSelect.ActiveTime) ? q.ActiveTimeId : default(long),
                Id = filter.Selects.Contains(MentorMenteeConnectionSelect.Id) ? q.Id : default(long),
                Connection = filter.Selects.Contains(MentorMenteeConnectionSelect.Connection) && q.Connection != null ? new MentorConnection
                {
                    Id = q.Connection.Id,
                    MentorId = q.Connection.MentorId,
                    Url = q.Connection.Url,
                    ConnectionTypeId = q.Connection.ConnectionTypeId,
                } : null,
                ConnectionStatus = filter.Selects.Contains(MentorMenteeConnectionSelect.ConnectionStatus) && q.ConnectionStatus != null ? new ConnectionStatus
                {
                    Id = q.ConnectionStatus.Id,
                    Code = q.ConnectionStatus.Code,
                    Name = q.ConnectionStatus.Name,
                } : null,
                Mentee = filter.Selects.Contains(MentorMenteeConnectionSelect.Mentee) && q.Mentee != null ? new AppUser
                {
                    Id = q.Mentee.Id,
                    Username = q.Mentee.Username,
                    Email = q.Mentee.Email,
                    Phone = q.Mentee.Phone,
                    Password = q.Mentee.Password,
                    DisplayName = q.Mentee.DisplayName,
                    SexId = q.Mentee.SexId,
                    Birthday = q.Mentee.Birthday,
                    Avatar = q.Mentee.Avatar,
                    CoverImage = q.Mentee.CoverImage,
                } : null,
                Mentor = filter.Selects.Contains(MentorMenteeConnectionSelect.Mentor) && q.Mentor != null ? new AppUser
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
            return MentorMenteeConnections;
        }

        public async Task<int> CountAll(MentorMenteeConnectionFilter filter)
        {
            IQueryable<MentorMenteeConnectionDAO> MentorMenteeConnectionDAOs = DataContext.MentorMenteeConnection.AsNoTracking();
            MentorMenteeConnectionDAOs = await DynamicFilter(MentorMenteeConnectionDAOs, filter);
            return await MentorMenteeConnectionDAOs.CountAsync();
        }

        public async Task<int> Count(MentorMenteeConnectionFilter filter)
        {
            IQueryable<MentorMenteeConnectionDAO> MentorMenteeConnectionDAOs = DataContext.MentorMenteeConnection.AsNoTracking();
            MentorMenteeConnectionDAOs = await DynamicFilter(MentorMenteeConnectionDAOs, filter);
            MentorMenteeConnectionDAOs = await OrFilter(MentorMenteeConnectionDAOs, filter);
            return await MentorMenteeConnectionDAOs.CountAsync();
        }

        public async Task<List<MentorMenteeConnection>> List(MentorMenteeConnectionFilter filter)
        {
            if (filter == null) return new List<MentorMenteeConnection>();
            IQueryable<MentorMenteeConnectionDAO> MentorMenteeConnectionDAOs = DataContext.MentorMenteeConnection.AsNoTracking();
            MentorMenteeConnectionDAOs = await DynamicFilter(MentorMenteeConnectionDAOs, filter);
            MentorMenteeConnectionDAOs = await OrFilter(MentorMenteeConnectionDAOs, filter);
            MentorMenteeConnectionDAOs = DynamicOrder(MentorMenteeConnectionDAOs, filter);
            List<MentorMenteeConnection> MentorMenteeConnections = await DynamicSelect(MentorMenteeConnectionDAOs, filter);
            return MentorMenteeConnections;
        }

        public async Task<List<MentorMenteeConnection>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MentorMenteeConnectionDAO> query = DataContext.MentorMenteeConnection.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<MentorMenteeConnection> MentorMenteeConnections = await query.AsNoTracking()
            .Select(x => new MentorMenteeConnection()
            {
                MentorId = x.MentorId,
                MenteeId = x.MenteeId,
                ConnectionId = x.ConnectionId,
                FirstMessage = x.FirstMessage,
                ConnectionStatusId = x.ConnectionStatusId,
                ActiveTimeId = x.ActiveTimeId,
                Id = x.Id,
                Connection = x.Connection == null ? null : new MentorConnection
                {
                    Id = x.Connection.Id,
                    MentorId = x.Connection.MentorId,
                    Url = x.Connection.Url,
                    ConnectionTypeId = x.Connection.ConnectionTypeId,
                },
                ConnectionStatus = x.ConnectionStatus == null ? null : new ConnectionStatus
                {
                    Id = x.ConnectionStatus.Id,
                    Code = x.ConnectionStatus.Code,
                    Name = x.ConnectionStatus.Name,
                },
                Mentee = x.Mentee == null ? null : new AppUser
                {
                    Id = x.Mentee.Id,
                    Username = x.Mentee.Username,
                    Email = x.Mentee.Email,
                    Phone = x.Mentee.Phone,
                    Password = x.Mentee.Password,
                    DisplayName = x.Mentee.DisplayName,
                    SexId = x.Mentee.SexId,
                    Birthday = x.Mentee.Birthday,
                    Avatar = x.Mentee.Avatar,
                    CoverImage = x.Mentee.CoverImage,
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


            return MentorMenteeConnections;
        }

        public async Task<MentorMenteeConnection> Get(long Id)
        {
            MentorMenteeConnection MentorMenteeConnection = await DataContext.MentorMenteeConnection.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new MentorMenteeConnection()
            {
                MentorId = x.MentorId,
                MenteeId = x.MenteeId,
                ConnectionId = x.ConnectionId,
                FirstMessage = x.FirstMessage,
                ConnectionStatusId = x.ConnectionStatusId,
                ActiveTimeId = x.ActiveTimeId,
                Id = x.Id,
                Connection = x.Connection == null ? null : new MentorConnection
                {
                    Id = x.Connection.Id,
                    MentorId = x.Connection.MentorId,
                    Url = x.Connection.Url,
                    ConnectionTypeId = x.Connection.ConnectionTypeId,
                },
                ConnectionStatus = x.ConnectionStatus == null ? null : new ConnectionStatus
                {
                    Id = x.ConnectionStatus.Id,
                    Code = x.ConnectionStatus.Code,
                    Name = x.ConnectionStatus.Name,
                },
                Mentee = x.Mentee == null ? null : new AppUser
                {
                    Id = x.Mentee.Id,
                    Username = x.Mentee.Username,
                    Email = x.Mentee.Email,
                    Phone = x.Mentee.Phone,
                    Password = x.Mentee.Password,
                    DisplayName = x.Mentee.DisplayName,
                    SexId = x.Mentee.SexId,
                    Birthday = x.Mentee.Birthday,
                    Avatar = x.Mentee.Avatar,
                    CoverImage = x.Mentee.CoverImage,
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

            if (MentorMenteeConnection == null)
                return null;

            return MentorMenteeConnection;
        }

        public async Task<bool> Create(MentorMenteeConnection MentorMenteeConnection)
        {
            MentorMenteeConnectionDAO MentorMenteeConnectionDAO = new MentorMenteeConnectionDAO();
            MentorMenteeConnectionDAO.MentorId = MentorMenteeConnection.MentorId;
            MentorMenteeConnectionDAO.MenteeId = MentorMenteeConnection.MenteeId;
            MentorMenteeConnectionDAO.ConnectionId = MentorMenteeConnection.ConnectionId;
            MentorMenteeConnectionDAO.FirstMessage = MentorMenteeConnection.FirstMessage;
            MentorMenteeConnectionDAO.ConnectionStatusId = MentorMenteeConnection.ConnectionStatusId;
            MentorMenteeConnectionDAO.ActiveTimeId = MentorMenteeConnection.ActiveTimeId;
            MentorMenteeConnectionDAO.Id = MentorMenteeConnection.Id;
            DataContext.MentorMenteeConnection.Add(MentorMenteeConnectionDAO);
            await DataContext.SaveChangesAsync();
            MentorMenteeConnection.Id = MentorMenteeConnectionDAO.Id;
            await SaveReference(MentorMenteeConnection);
            return true;
        }

        public async Task<bool> Update(MentorMenteeConnection MentorMenteeConnection)
        {
            MentorMenteeConnectionDAO MentorMenteeConnectionDAO = DataContext.MentorMenteeConnection
                .Where(x => x.Id == MentorMenteeConnection.Id)
                .FirstOrDefault();
            if (MentorMenteeConnectionDAO == null)
                return false;
            MentorMenteeConnectionDAO.MentorId = MentorMenteeConnection.MentorId;
            MentorMenteeConnectionDAO.MenteeId = MentorMenteeConnection.MenteeId;
            MentorMenteeConnectionDAO.ConnectionId = MentorMenteeConnection.ConnectionId;
            MentorMenteeConnectionDAO.FirstMessage = MentorMenteeConnection.FirstMessage;
            MentorMenteeConnectionDAO.ConnectionStatusId = MentorMenteeConnection.ConnectionStatusId;
            MentorMenteeConnectionDAO.ActiveTimeId = MentorMenteeConnection.ActiveTimeId;
            MentorMenteeConnectionDAO.Id = MentorMenteeConnection.Id;
            await DataContext.SaveChangesAsync();
            await SaveReference(MentorMenteeConnection);
            return true;
        }

        public async Task<bool> Delete(MentorMenteeConnection MentorMenteeConnection)
        {
            await DataContext.MentorMenteeConnection
                .Where(x => x.Id == MentorMenteeConnection.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<MentorMenteeConnection> MentorMenteeConnections)
        {
            IdFilter IdFilter = new IdFilter { In = MentorMenteeConnections.Select(x => x.Id).ToList() };
            List<MentorMenteeConnectionDAO> MentorMenteeConnectionDAOs = new List<MentorMenteeConnectionDAO>();
            List<MentorMenteeConnectionDAO> DbMentorMenteeConnectionDAOs = await DataContext.MentorMenteeConnection
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (MentorMenteeConnection MentorMenteeConnection in MentorMenteeConnections)
            {
                MentorMenteeConnectionDAO MentorMenteeConnectionDAO = DbMentorMenteeConnectionDAOs
                        .Where(x => x.Id == MentorMenteeConnection.Id)
                        .FirstOrDefault();
                if (MentorMenteeConnectionDAO == null)
                {
                    MentorMenteeConnectionDAO = new MentorMenteeConnectionDAO();
                }
                MentorMenteeConnectionDAO.MentorId = MentorMenteeConnection.MentorId;
                MentorMenteeConnectionDAO.MenteeId = MentorMenteeConnection.MenteeId;
                MentorMenteeConnectionDAO.ConnectionId = MentorMenteeConnection.ConnectionId;
                MentorMenteeConnectionDAO.FirstMessage = MentorMenteeConnection.FirstMessage;
                MentorMenteeConnectionDAO.ConnectionStatusId = MentorMenteeConnection.ConnectionStatusId;
                MentorMenteeConnectionDAO.ActiveTimeId = MentorMenteeConnection.ActiveTimeId;
                MentorMenteeConnectionDAOs.Add(MentorMenteeConnectionDAO);
            }
            await DataContext.BulkMergeAsync(MentorMenteeConnectionDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<MentorMenteeConnection> MentorMenteeConnections)
        {
            List<long> Ids = MentorMenteeConnections.Select(x => x.Id).ToList();
            await DataContext.MentorMenteeConnection
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(MentorMenteeConnection MentorMenteeConnection)
        {
        }

    }
}
