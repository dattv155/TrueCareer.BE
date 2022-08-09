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
    public interface IMbtiResultRepository
    {
        Task<int> CountAll(MbtiResultFilter MbtiResultFilter);
        Task<int> Count(MbtiResultFilter MbtiResultFilter);
        Task<List<MbtiResult>> List(MbtiResultFilter MbtiResultFilter);
        Task<List<MbtiResult>> List(List<long> Ids);
        Task<MbtiResult> Get(long Id);
        Task<bool> Create(MbtiResult MbtiResult);
        Task<bool> Update(MbtiResult MbtiResult);
        Task<bool> Delete(MbtiResult MbtiResult);
        Task<bool> BulkMerge(List<MbtiResult> MbtiResults);
        Task<bool> BulkDelete(List<MbtiResult> MbtiResults);
    }
    public class MbtiResultRepository : IMbtiResultRepository
    {
        private DataContext DataContext;
        public MbtiResultRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<MbtiResultDAO>> DynamicFilter(IQueryable<MbtiResultDAO> query, MbtiResultFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.MbtiPersonalTypeId, filter.MbtiPersonalTypeId);
            query = query.Where(q => q.UserId, filter.UserId);
            return query;
        }

        private async Task<IQueryable<MbtiResultDAO>> OrFilter(IQueryable<MbtiResultDAO> query, MbtiResultFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MbtiResultDAO> initQuery = query.Where(q => false);
            foreach (MbtiResultFilter MbtiResultFilter in filter.OrFilter)
            {
                IQueryable<MbtiResultDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, MbtiResultFilter.Id);
                queryable = queryable.Where(q => q.MbtiPersonalTypeId, MbtiResultFilter.MbtiPersonalTypeId);
                queryable = queryable.Where(q => q.UserId, MbtiResultFilter.UserId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<MbtiResultDAO> DynamicOrder(IQueryable<MbtiResultDAO> query, MbtiResultFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MbtiResultOrder.User:
                            query = query.OrderBy(q => q.UserId);
                            break;
                        case MbtiResultOrder.MbtiPersonalType:
                            query = query.OrderBy(q => q.MbtiPersonalTypeId);
                            break;
                        case MbtiResultOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MbtiResultOrder.User:
                            query = query.OrderByDescending(q => q.UserId);
                            break;
                        case MbtiResultOrder.MbtiPersonalType:
                            query = query.OrderByDescending(q => q.MbtiPersonalTypeId);
                            break;
                        case MbtiResultOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<MbtiResult>> DynamicSelect(IQueryable<MbtiResultDAO> query, MbtiResultFilter filter)
        {
            List<MbtiResult> MbtiResults = await query.Select(q => new MbtiResult()
            {
                UserId = filter.Selects.Contains(MbtiResultSelect.User) ? q.UserId : default(long),
                MbtiPersonalTypeId = filter.Selects.Contains(MbtiResultSelect.MbtiPersonalType) ? q.MbtiPersonalTypeId : default(long),
                Id = filter.Selects.Contains(MbtiResultSelect.Id) ? q.Id : default(long),
                MbtiPersonalType = filter.Selects.Contains(MbtiResultSelect.MbtiPersonalType) && q.MbtiPersonalType != null ? new MbtiPersonalType
                {
                    Id = q.MbtiPersonalType.Id,
                    Name = q.MbtiPersonalType.Name,
                    Code = q.MbtiPersonalType.Code,
                    Value = q.MbtiPersonalType.Value,
                } : null,
                User = filter.Selects.Contains(MbtiResultSelect.User) && q.User != null ? new AppUser
                {
                    Id = q.User.Id,
                    Username = q.User.Username,
                    Email = q.User.Email,
                    Phone = q.User.Phone,
                    Password = q.User.Password,
                    DisplayName = q.User.DisplayName,
                    SexId = q.User.SexId,
                    Birthday = q.User.Birthday,
                    Avatar = q.User.Avatar,
                    CoverImage = q.User.CoverImage,
                } : null,
            }).ToListAsync();
            return MbtiResults;
        }

        public async Task<int> CountAll(MbtiResultFilter filter)
        {
            IQueryable<MbtiResultDAO> MbtiResultDAOs = DataContext.MbtiResult.AsNoTracking();
            MbtiResultDAOs = await DynamicFilter(MbtiResultDAOs, filter);
            return await MbtiResultDAOs.CountAsync();
        }

        public async Task<int> Count(MbtiResultFilter filter)
        {
            IQueryable<MbtiResultDAO> MbtiResultDAOs = DataContext.MbtiResult.AsNoTracking();
            MbtiResultDAOs = await DynamicFilter(MbtiResultDAOs, filter);
            MbtiResultDAOs = await OrFilter(MbtiResultDAOs, filter);
            return await MbtiResultDAOs.CountAsync();
        }

        public async Task<List<MbtiResult>> List(MbtiResultFilter filter)
        {
            if (filter == null) return new List<MbtiResult>();
            IQueryable<MbtiResultDAO> MbtiResultDAOs = DataContext.MbtiResult.AsNoTracking();
            MbtiResultDAOs = await DynamicFilter(MbtiResultDAOs, filter);
            MbtiResultDAOs = await OrFilter(MbtiResultDAOs, filter);
            MbtiResultDAOs = DynamicOrder(MbtiResultDAOs, filter);
            List<MbtiResult> MbtiResults = await DynamicSelect(MbtiResultDAOs, filter);
            return MbtiResults;
        }

        public async Task<List<MbtiResult>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MbtiResultDAO> query = DataContext.MbtiResult.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<MbtiResult> MbtiResults = await query.AsNoTracking()
            .Select(x => new MbtiResult()
            {
                UserId = x.UserId,
                MbtiPersonalTypeId = x.MbtiPersonalTypeId,
                Id = x.Id,
                MbtiPersonalType = x.MbtiPersonalType == null ? null : new MbtiPersonalType
                {
                    Id = x.MbtiPersonalType.Id,
                    Name = x.MbtiPersonalType.Name,
                    Code = x.MbtiPersonalType.Code,
                    Value = x.MbtiPersonalType.Value,
                },
                User = x.User == null ? null : new AppUser
                {
                    Id = x.User.Id,
                    Username = x.User.Username,
                    Email = x.User.Email,
                    Phone = x.User.Phone,
                    Password = x.User.Password,
                    DisplayName = x.User.DisplayName,
                    SexId = x.User.SexId,
                    Birthday = x.User.Birthday,
                    Avatar = x.User.Avatar,
                    CoverImage = x.User.CoverImage,
                },
            }).ToListAsync();
            

            return MbtiResults;
        }

        public async Task<MbtiResult> Get(long Id)
        {
            MbtiResult MbtiResult = await DataContext.MbtiResult.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new MbtiResult()
            {
                UserId = x.UserId,
                MbtiPersonalTypeId = x.MbtiPersonalTypeId,
                Id = x.Id,
                MbtiPersonalType = x.MbtiPersonalType == null ? null : new MbtiPersonalType
                {
                    Id = x.MbtiPersonalType.Id,
                    Name = x.MbtiPersonalType.Name,
                    Code = x.MbtiPersonalType.Code,
                    Value = x.MbtiPersonalType.Value
                },
                User = x.User == null ? null : new AppUser
                {
                    Id = x.User.Id,
                    Username = x.User.Username,
                    Email = x.User.Email,
                    Phone = x.User.Phone,
                    Password = x.User.Password,
                    DisplayName = x.User.DisplayName,
                    SexId = x.User.SexId,
                    Birthday = x.User.Birthday,
                    Avatar = x.User.Avatar,
                    CoverImage = x.User.CoverImage,
                },
            }).FirstOrDefaultAsync();
            
            if (MbtiResult == null)
                return null;
            
            List<MbtiPersonalTypeMajorMapping> mbtiPersonalTypeMajorMappings = await DataContext.MbtiPersonalTypeMajorMapping.AsNoTracking()
                .Where(x => x.MbtiPersonalTypeId == MbtiResult.MbtiPersonalTypeId)
                .Select(x => new MbtiPersonalTypeMajorMapping
                {
                    MbtiPersonalTypeId = x.MbtiPersonalTypeId,
                    MajorId = x.MajorId,
                }).ToListAsync();

            List<long> majorIds = mbtiPersonalTypeMajorMappings.Select(x => x.MajorId).ToList();
            List<Major> majors = await DataContext.Major.AsNoTracking()
                .Where(x => majorIds.Contains(x.Id))
                .Select(x => new Major()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    MajorImage = x.MajorImage,
                }).ToListAsync();

            foreach (var smm in mbtiPersonalTypeMajorMappings)
            {
                smm.Major = majors.Where(x => x.Id == smm.MajorId).FirstOrDefault();
            }
            MbtiResult.MbtiPersonalType.MbtiPersonalTypeMajorMappings = mbtiPersonalTypeMajorMappings;

            return MbtiResult;
        }
        
        public async Task<bool> Create(MbtiResult MbtiResult)
        {
            MbtiResultDAO MbtiResultDAO = new MbtiResultDAO();
            MbtiResultDAO.UserId = MbtiResult.UserId;
            MbtiResultDAO.MbtiPersonalTypeId = MbtiResult.MbtiPersonalTypeId;
            MbtiResultDAO.Id = MbtiResult.Id;
            DataContext.MbtiResult.Add(MbtiResultDAO);
            await DataContext.SaveChangesAsync();
            MbtiResult.Id = MbtiResultDAO.Id;
            await SaveReference(MbtiResult);
            return true;
        }

        public async Task<bool> Update(MbtiResult MbtiResult)
        {
            MbtiResultDAO MbtiResultDAO = DataContext.MbtiResult
                .Where(x => x.Id == MbtiResult.Id)
                .FirstOrDefault();
            if (MbtiResultDAO == null)
                return false;
            MbtiResultDAO.UserId = MbtiResult.UserId;
            MbtiResultDAO.MbtiPersonalTypeId = MbtiResult.MbtiPersonalTypeId;
            MbtiResultDAO.Id = MbtiResult.Id;
            await DataContext.SaveChangesAsync();
            await SaveReference(MbtiResult);
            return true;
        }

        public async Task<bool> Delete(MbtiResult MbtiResult)
        {
            await DataContext.MbtiResult
                .Where(x => x.Id == MbtiResult.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<MbtiResult> MbtiResults)
        {
            IdFilter IdFilter = new IdFilter { In = MbtiResults.Select(x => x.Id).ToList() };
            List<MbtiResultDAO> MbtiResultDAOs = new List<MbtiResultDAO>();
            List<MbtiResultDAO> DbMbtiResultDAOs = await DataContext.MbtiResult
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (MbtiResult MbtiResult in MbtiResults)
            {
                MbtiResultDAO MbtiResultDAO = DbMbtiResultDAOs
                        .Where(x => x.Id == MbtiResult.Id)
                        .FirstOrDefault();
                if (MbtiResultDAO == null)
                {
                    MbtiResultDAO = new MbtiResultDAO();
                }
                MbtiResultDAO.UserId = MbtiResult.UserId;
                MbtiResultDAO.MbtiPersonalTypeId = MbtiResult.MbtiPersonalTypeId;
                MbtiResultDAOs.Add(MbtiResultDAO);
            }
            await DataContext.BulkMergeAsync(MbtiResultDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<MbtiResult> MbtiResults)
        {
            List<long> Ids = MbtiResults.Select(x => x.Id).ToList();
            await DataContext.MbtiResult
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(MbtiResult MbtiResult)
        {
        }
        
    }
}
