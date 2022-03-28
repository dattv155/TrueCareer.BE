using TrueSight.Common;
using TrueCareer.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueCareer.BE.Models;

namespace TrueCareer.Repositories
{
    public interface IGlobalUserRepository
    {
        Task<int> Count(GlobalUserFilter GlobalUserFilter);
        Task<List<GlobalUser>> List(GlobalUserFilter GlobalUserFilter);
        Task<List<GlobalUser>> List(List<long> Ids);
        Task<GlobalUser> Get(long Id);
        Task<GlobalUser> Get(Guid RowId);
        Task<bool> BulkMerge(List<GlobalUser> GlobalUsers);
        Task<bool> CreateToken(FirebaseToken FirebaseToken);
        Task<bool> DeleteToken(FirebaseToken FirebaseToken);
    }
    public class GlobalUserRepository : IGlobalUserRepository
    {
        private DataContext DataContext;
        public GlobalUserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<GlobalUserDAO> DynamicFilter(IQueryable<GlobalUserDAO> query, GlobalUserFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.RowId, filter.RowId);
            query = query.Where(q => q.Username, filter.Username);
            query = query.Where(q => q.DisplayName, filter.DisplayName);
            query = query.Where(q => q.Avatar, filter.Avatar);
            query = query.Where(q => q.GlobalUserTypeId, filter.GlobalUserTypeId);

            return query;
        }

        private IQueryable<GlobalUserDAO> DynamicOrder(IQueryable<GlobalUserDAO> query, GlobalUserFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case GlobalUserOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case GlobalUserOrder.RowId:
                            query = query.OrderBy(q => q.RowId);
                            break;
                        case GlobalUserOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case GlobalUserOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                        case GlobalUserOrder.Avatar:
                            query = query.OrderBy(q => q.Avatar);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case GlobalUserOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case GlobalUserOrder.RowId:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                        case GlobalUserOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case GlobalUserOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                        case GlobalUserOrder.Avatar:
                            query = query.OrderByDescending(q => q.Avatar);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<GlobalUser>> DynamicSelect(IQueryable<GlobalUserDAO> query, GlobalUserFilter filter)
        {
            List<GlobalUser> GlobalUsers = await query.Select(q => new GlobalUser()
            {
                Id = q.Id,
                RowId = q.RowId,
                Username = q.Username,
                DisplayName = q.DisplayName,
                Avatar = q.Avatar,
                GlobalUserTypeId = q.GlobalUserTypeId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return GlobalUsers;
        }

        public async Task<int> Count(GlobalUserFilter filter)
        {
            IQueryable<GlobalUserDAO> GlobalUsers = DataContext.GlobalUser;
            GlobalUsers = DynamicFilter(GlobalUsers, filter);
            return await GlobalUsers.CountAsync();
        }

        public async Task<List<GlobalUser>> List(GlobalUserFilter filter)
        {
            if (filter == null) return new List<GlobalUser>();
            IQueryable<GlobalUserDAO> GlobalUserDAOs = DataContext.GlobalUser.AsNoTracking();
            GlobalUserDAOs = DynamicFilter(GlobalUserDAOs, filter);
            GlobalUserDAOs = DynamicOrder(GlobalUserDAOs, filter);
            List<GlobalUser> GlobalUsers = await DynamicSelect(GlobalUserDAOs, filter);
            return GlobalUsers;
        }

        public async Task<GlobalUser> Get(long Id)
        {
            GlobalUser GlobalUser = await DataContext.GlobalUser.Where(x => x.Id == Id)
                .Select(x => new GlobalUser()
                {
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Id = x.Id,
                    RowId = x.RowId,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    Avatar = x.Avatar,
                    GlobalUserTypeId = x.GlobalUserTypeId,
                }).FirstOrDefaultAsync();

            if (GlobalUser == null)
                return null;

            return GlobalUser;
        }
        public async Task<List<GlobalUser>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };
            List<GlobalUser> GlobalUsers = await DataContext.GlobalUser.Where(x => x.Id, IdFilter)
                .Select(x => new GlobalUser()
                {
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Id = x.Id,
                    RowId = x.RowId,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    Avatar = x.Avatar,
                    GlobalUserTypeId = x.GlobalUserTypeId,
                }).ToListAsync();
            var FirebaseTokenQuery = DataContext.FirebaseToken.AsNoTracking()
                .Where(x => Ids.Contains(x.Id));
            List<FirebaseToken> FirebaseTokens = await FirebaseTokenQuery
                .Select(x => new FirebaseToken
                {
                    GlobalUserId = x.GlobalUserId,
                    Token = x.Token,
                }).ToListAsync();
            foreach (GlobalUser GlobalUser in GlobalUsers)
            {
                GlobalUser.Tokens = FirebaseTokens
                    .Where(x => x.GlobalUserId == GlobalUser.Id)
                    .Select(x => x.Token)
                    .Distinct()
                    .ToList();
            }
            return GlobalUsers;
        }

        public async Task<GlobalUser> Get(Guid RowId)
        {
            GlobalUser GlobalUser = await DataContext.GlobalUser.Where(x => x.RowId == RowId)
                .Select(x => new GlobalUser()
                {
                    Id = x.Id,
                    RowId = x.RowId,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    Avatar = x.Avatar,
                    GlobalUserTypeId = x.GlobalUserTypeId,
                }).FirstOrDefaultAsync();

            if (GlobalUser == null)
                return null;

            return GlobalUser;
        }

        public async Task<bool> BulkMerge(List<GlobalUser> GlobalUsers)
        {
            List<Guid> RowIds = GlobalUsers.Select(x => x.RowId).ToList();
            List<GlobalUserDAO> GlobalUserDAOs = await DataContext.GlobalUser.AsNoTracking()
                .Where(x => RowIds.Contains(x.RowId))
                .ToListAsync();
            foreach (GlobalUser GlobalUser in GlobalUsers)
            {
                GlobalUserDAO GlobalUserDAO = GlobalUserDAOs
                    .Where(x => x.RowId == GlobalUser.RowId)
                    .FirstOrDefault();
                if (GlobalUserDAO == null)
                {
                    GlobalUserDAO = new GlobalUserDAO();
                    GlobalUserDAOs.Add(GlobalUserDAO);
                }
                GlobalUserDAO.Username = GlobalUser.Username;
                GlobalUserDAO.DisplayName = GlobalUser.DisplayName;
                GlobalUserDAO.RowId = GlobalUser.RowId;
                GlobalUserDAO.Avatar = GlobalUser.Avatar;
                GlobalUserDAO.GlobalUserTypeId = GlobalUser.GlobalUserTypeId;
                GlobalUserDAO.CreatedAt = GlobalUser.CreatedAt;
                GlobalUserDAO.UpdatedAt = GlobalUser.UpdatedAt;
                GlobalUserDAO.DeletedAt = GlobalUser.DeletedAt;
            }
            await DataContext.BulkMergeAsync(GlobalUserDAOs);
            foreach (GlobalUser GlobalUser in GlobalUsers)
            {
                GlobalUser.Id = GlobalUserDAOs.Where(x => x.RowId == GlobalUser.RowId)
                    .Select(x => x.Id)
                    .FirstOrDefault();
            }
            return true;
        }
        public async Task<bool> CreateToken(FirebaseToken FirebaseToken)
        {
            FirebaseTokenDAO FirebaseTokenDAO = await DataContext.FirebaseToken.Where(x => x.Token == FirebaseToken.Token).FirstOrDefaultAsync();
            if (FirebaseTokenDAO == null)
            {
                FirebaseTokenDAO = new FirebaseTokenDAO
                {
                    GlobalUserId = FirebaseToken.GlobalUserId,
                    Token = FirebaseToken.Token,
                    DeviceModel = FirebaseToken.DeviceModel,
                    OsName = FirebaseToken.OsName,
                    OsVersion = FirebaseToken.OsVersion,
                    UpdatedAt = StaticParams.DateTimeNow,
                };
                DataContext.FirebaseToken.Add(FirebaseTokenDAO);

            }
            else
            {
                FirebaseTokenDAO.GlobalUserId = FirebaseToken.GlobalUserId;
                FirebaseTokenDAO.DeviceModel = FirebaseToken.DeviceModel;
                FirebaseTokenDAO.OsName = FirebaseToken.OsName;
                FirebaseTokenDAO.OsVersion = FirebaseToken.OsVersion;
                FirebaseTokenDAO.UpdatedAt = StaticParams.DateTimeNow;
            }
            await DataContext.SaveChangesAsync();

            await DataContext.FirebaseToken.Where(x => x.UpdatedAt < StaticParams.DateTimeNow.AddHours(-12)).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> DeleteToken(FirebaseToken FirebaseToken)
        {
            await DataContext.FirebaseToken.Where(x => x.Token == FirebaseToken.Token).DeleteFromQueryAsync();

            return true;
        }
    }
}
