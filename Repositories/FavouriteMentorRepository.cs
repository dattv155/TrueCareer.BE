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
    public interface IFavouriteMentorRepository
    {
        Task<int> CountAll(FavouriteMentorFilter FavouriteMentorFilter);
        Task<int> Count(FavouriteMentorFilter FavouriteMentorFilter);
        Task<List<FavouriteMentor>> List(FavouriteMentorFilter FavouriteMentorFilter);
        Task<List<FavouriteMentor>> List(List<long> Ids);
        Task<FavouriteMentor> Get(long Id);
        Task<bool> Create(FavouriteMentor FavouriteMentor);
        Task<bool> Update(FavouriteMentor FavouriteMentor);
        Task<bool> Delete(FavouriteMentor FavouriteMentor);
        Task<bool> BulkMerge(List<FavouriteMentor> FavouriteMentors);
        Task<bool> BulkDelete(List<FavouriteMentor> FavouriteMentors);
    }
    public class FavouriteMentorRepository : IFavouriteMentorRepository
    {
        private DataContext DataContext;
        public FavouriteMentorRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<FavouriteMentorDAO>> DynamicFilter(IQueryable<FavouriteMentorDAO> query, FavouriteMentorFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.MentorId, filter.MentorId);
            query = query.Where(q => q.UserId, filter.UserId);
            return query;
        }

        private async Task<IQueryable<FavouriteMentorDAO>> OrFilter(IQueryable<FavouriteMentorDAO> query, FavouriteMentorFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<FavouriteMentorDAO> initQuery = query.Where(q => false);
            foreach (FavouriteMentorFilter FavouriteMentorFilter in filter.OrFilter)
            {
                IQueryable<FavouriteMentorDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, FavouriteMentorFilter.Id);
                queryable = queryable.Where(q => q.MentorId, FavouriteMentorFilter.MentorId);
                queryable = queryable.Where(q => q.UserId, FavouriteMentorFilter.UserId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<FavouriteMentorDAO> DynamicOrder(IQueryable<FavouriteMentorDAO> query, FavouriteMentorFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case FavouriteMentorOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case FavouriteMentorOrder.User:
                            query = query.OrderBy(q => q.UserId);
                            break;
                        case FavouriteMentorOrder.Mentor:
                            query = query.OrderBy(q => q.MentorId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case FavouriteMentorOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case FavouriteMentorOrder.User:
                            query = query.OrderByDescending(q => q.UserId);
                            break;
                        case FavouriteMentorOrder.Mentor:
                            query = query.OrderByDescending(q => q.MentorId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<FavouriteMentor>> DynamicSelect(IQueryable<FavouriteMentorDAO> query, FavouriteMentorFilter filter)
        {
            List<FavouriteMentor> FavouriteMentors = await query.Select(q => new FavouriteMentor()
            {
                Id = filter.Selects.Contains(FavouriteMentorSelect.Id) ? q.Id : default(long),
                UserId = filter.Selects.Contains(FavouriteMentorSelect.User) ? q.UserId : default(long),
                MentorId = filter.Selects.Contains(FavouriteMentorSelect.Mentor) ? q.MentorId : default(long),
                Mentor = filter.Selects.Contains(FavouriteMentorSelect.Mentor) && q.Mentor != null ? new AppUser
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
                User = filter.Selects.Contains(FavouriteMentorSelect.User) && q.User != null ? new AppUser
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
            return FavouriteMentors;
        }

        public async Task<int> CountAll(FavouriteMentorFilter filter)
        {
            IQueryable<FavouriteMentorDAO> FavouriteMentorDAOs = DataContext.FavouriteMentor.AsNoTracking();
            FavouriteMentorDAOs = await DynamicFilter(FavouriteMentorDAOs, filter);
            return await FavouriteMentorDAOs.CountAsync();
        }

        public async Task<int> Count(FavouriteMentorFilter filter)
        {
            IQueryable<FavouriteMentorDAO> FavouriteMentorDAOs = DataContext.FavouriteMentor.AsNoTracking();
            FavouriteMentorDAOs = await DynamicFilter(FavouriteMentorDAOs, filter);
            FavouriteMentorDAOs = await OrFilter(FavouriteMentorDAOs, filter);
            return await FavouriteMentorDAOs.CountAsync();
        }

        public async Task<List<FavouriteMentor>> List(FavouriteMentorFilter filter)
        {
            if (filter == null) return new List<FavouriteMentor>();
            IQueryable<FavouriteMentorDAO> FavouriteMentorDAOs = DataContext.FavouriteMentor.AsNoTracking();
            FavouriteMentorDAOs = await DynamicFilter(FavouriteMentorDAOs, filter);
            FavouriteMentorDAOs = await OrFilter(FavouriteMentorDAOs, filter);
            FavouriteMentorDAOs = DynamicOrder(FavouriteMentorDAOs, filter);
            List<FavouriteMentor> FavouriteMentors = await DynamicSelect(FavouriteMentorDAOs, filter);
            return FavouriteMentors;
        }

        public async Task<List<FavouriteMentor>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<FavouriteMentorDAO> query = DataContext.FavouriteMentor.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<FavouriteMentor> FavouriteMentors = await query.AsNoTracking()
            .Select(x => new FavouriteMentor()
            {
                Id = x.Id,
                UserId = x.UserId,
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
            

            return FavouriteMentors;
        }

        public async Task<FavouriteMentor> Get(long Id)
        {
            FavouriteMentor FavouriteMentor = await DataContext.FavouriteMentor.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new FavouriteMentor()
            {
                Id = x.Id,
                UserId = x.UserId,
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

            if (FavouriteMentor == null)
                return null;

            return FavouriteMentor;
        }
        
        public async Task<bool> Create(FavouriteMentor FavouriteMentor)
        {
            FavouriteMentorDAO FavouriteMentorDAO = new FavouriteMentorDAO();
            FavouriteMentorDAO.Id = FavouriteMentor.Id;
            FavouriteMentorDAO.UserId = FavouriteMentor.UserId;
            FavouriteMentorDAO.MentorId = FavouriteMentor.MentorId;
            DataContext.FavouriteMentor.Add(FavouriteMentorDAO);
            await DataContext.SaveChangesAsync();
            FavouriteMentor.Id = FavouriteMentorDAO.Id;
            await SaveReference(FavouriteMentor);
            return true;
        }

        public async Task<bool> Update(FavouriteMentor FavouriteMentor)
        {
            FavouriteMentorDAO FavouriteMentorDAO = DataContext.FavouriteMentor
                .Where(x => x.Id == FavouriteMentor.Id)
                .FirstOrDefault();
            if (FavouriteMentorDAO == null)
                return false;
            FavouriteMentorDAO.Id = FavouriteMentor.Id;
            FavouriteMentorDAO.UserId = FavouriteMentor.UserId;
            FavouriteMentorDAO.MentorId = FavouriteMentor.MentorId;
            await DataContext.SaveChangesAsync();
            await SaveReference(FavouriteMentor);
            return true;
        }

        public async Task<bool> Delete(FavouriteMentor FavouriteMentor)
        {
            await DataContext.FavouriteMentor
                .Where(x => x.Id == FavouriteMentor.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<FavouriteMentor> FavouriteMentors)
        {
            IdFilter IdFilter = new IdFilter { In = FavouriteMentors.Select(x => x.Id).ToList() };
            List<FavouriteMentorDAO> FavouriteMentorDAOs = new List<FavouriteMentorDAO>();
            List<FavouriteMentorDAO> DbFavouriteMentorDAOs = await DataContext.FavouriteMentor
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (FavouriteMentor FavouriteMentor in FavouriteMentors)
            {
                FavouriteMentorDAO FavouriteMentorDAO = DbFavouriteMentorDAOs
                        .Where(x => x.Id == FavouriteMentor.Id)
                        .FirstOrDefault();
                if (FavouriteMentorDAO == null)
                {
                    FavouriteMentorDAO = new FavouriteMentorDAO();
                }
                FavouriteMentorDAO.UserId = FavouriteMentor.UserId;
                FavouriteMentorDAO.MentorId = FavouriteMentor.MentorId;
                FavouriteMentorDAOs.Add(FavouriteMentorDAO);
            }
            await DataContext.BulkMergeAsync(FavouriteMentorDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<FavouriteMentor> FavouriteMentors)
        {
            List<long> Ids = FavouriteMentors.Select(x => x.Id).ToList();
            await DataContext.FavouriteMentor
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(FavouriteMentor FavouriteMentor)
        {
        }
        
    }
}
