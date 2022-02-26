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
    public interface IMentorReviewRepository
    {
        Task<int> CountAll(MentorReviewFilter MentorReviewFilter);
        Task<int> Count(MentorReviewFilter MentorReviewFilter);
        Task<List<MentorReview>> List(MentorReviewFilter MentorReviewFilter);
        Task<List<MentorReview>> List(List<long> Ids);
        Task<MentorReview> Get(long Id);
        Task<bool> Create(MentorReview MentorReview);
        Task<bool> Update(MentorReview MentorReview);
        Task<bool> Delete(MentorReview MentorReview);
        Task<bool> BulkMerge(List<MentorReview> MentorReviews);
        Task<bool> BulkDelete(List<MentorReview> MentorReviews);
    }
    public class MentorReviewRepository : IMentorReviewRepository
    {
        private DataContext DataContext;
        public MentorReviewRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<MentorReviewDAO>> DynamicFilter(IQueryable<MentorReviewDAO> query, MentorReviewFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Description, filter.Description);
            query = query.Where(q => q.ContentReview, filter.ContentReview);
            query = query.Where(q => q.Star, filter.Star);
            query = query.Where(q => q.Time, filter.Time);
            query = query.Where(q => q.CreatorId, filter.CreatorId);
            query = query.Where(q => q.MentorId, filter.MentorId);
            return query;
        }

        private async Task<IQueryable<MentorReviewDAO>> OrFilter(IQueryable<MentorReviewDAO> query, MentorReviewFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MentorReviewDAO> initQuery = query.Where(q => false);
            foreach (MentorReviewFilter MentorReviewFilter in filter.OrFilter)
            {
                IQueryable<MentorReviewDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, MentorReviewFilter.Id);
                queryable = queryable.Where(q => q.Description, MentorReviewFilter.Description);
                queryable = queryable.Where(q => q.ContentReview, MentorReviewFilter.ContentReview);
                queryable = queryable.Where(q => q.Star, MentorReviewFilter.Star);
                queryable = queryable.Where(q => q.Time, MentorReviewFilter.Time);
                queryable = queryable.Where(q => q.CreatorId, MentorReviewFilter.CreatorId);
                queryable = queryable.Where(q => q.MentorId, MentorReviewFilter.MentorId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<MentorReviewDAO> DynamicOrder(IQueryable<MentorReviewDAO> query, MentorReviewFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MentorReviewOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case MentorReviewOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case MentorReviewOrder.ContentReview:
                            query = query.OrderBy(q => q.ContentReview);
                            break;
                        case MentorReviewOrder.Star:
                            query = query.OrderBy(q => q.Star);
                            break;
                        case MentorReviewOrder.Mentor:
                            query = query.OrderBy(q => q.MentorId);
                            break;
                        case MentorReviewOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case MentorReviewOrder.Time:
                            query = query.OrderBy(q => q.Time);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MentorReviewOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case MentorReviewOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case MentorReviewOrder.ContentReview:
                            query = query.OrderByDescending(q => q.ContentReview);
                            break;
                        case MentorReviewOrder.Star:
                            query = query.OrderByDescending(q => q.Star);
                            break;
                        case MentorReviewOrder.Mentor:
                            query = query.OrderByDescending(q => q.MentorId);
                            break;
                        case MentorReviewOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case MentorReviewOrder.Time:
                            query = query.OrderByDescending(q => q.Time);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<MentorReview>> DynamicSelect(IQueryable<MentorReviewDAO> query, MentorReviewFilter filter)
        {
            List<MentorReview> MentorReviews = await query.Select(q => new MentorReview()
            {
                Id = filter.Selects.Contains(MentorReviewSelect.Id) ? q.Id : default(long),
                Description = filter.Selects.Contains(MentorReviewSelect.Description) ? q.Description : default(string),
                ContentReview = filter.Selects.Contains(MentorReviewSelect.ContentReview) ? q.ContentReview : default(string),
                Star = filter.Selects.Contains(MentorReviewSelect.Star) ? q.Star : default(long),
                MentorId = filter.Selects.Contains(MentorReviewSelect.Mentor) ? q.MentorId : default(long),
                CreatorId = filter.Selects.Contains(MentorReviewSelect.Creator) ? q.CreatorId : default(long),
                Time = filter.Selects.Contains(MentorReviewSelect.Time) ? q.Time : default(DateTime),
                Creator = filter.Selects.Contains(MentorReviewSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    Email = q.Creator.Email,
                    Phone = q.Creator.Phone,
                    Password = q.Creator.Password,
                    DisplayName = q.Creator.DisplayName,
                    SexId = q.Creator.SexId,
                    Birthday = q.Creator.Birthday,
                    Avatar = q.Creator.Avatar,
                    CoverImage = q.Creator.CoverImage,
                } : null,
                Mentor = filter.Selects.Contains(MentorReviewSelect.Mentor) && q.Mentor != null ? new AppUser
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
            return MentorReviews;
        }

        public async Task<int> CountAll(MentorReviewFilter filter)
        {
            IQueryable<MentorReviewDAO> MentorReviewDAOs = DataContext.MentorReview.AsNoTracking();
            MentorReviewDAOs = await DynamicFilter(MentorReviewDAOs, filter);
            return await MentorReviewDAOs.CountAsync();
        }

        public async Task<int> Count(MentorReviewFilter filter)
        {
            IQueryable<MentorReviewDAO> MentorReviewDAOs = DataContext.MentorReview.AsNoTracking();
            MentorReviewDAOs = await DynamicFilter(MentorReviewDAOs, filter);
            MentorReviewDAOs = await OrFilter(MentorReviewDAOs, filter);
            return await MentorReviewDAOs.CountAsync();
        }

        public async Task<List<MentorReview>> List(MentorReviewFilter filter)
        {
            if (filter == null) return new List<MentorReview>();
            IQueryable<MentorReviewDAO> MentorReviewDAOs = DataContext.MentorReview.AsNoTracking();
            MentorReviewDAOs = await DynamicFilter(MentorReviewDAOs, filter);
            MentorReviewDAOs = await OrFilter(MentorReviewDAOs, filter);
            MentorReviewDAOs = DynamicOrder(MentorReviewDAOs, filter);
            List<MentorReview> MentorReviews = await DynamicSelect(MentorReviewDAOs, filter);
            return MentorReviews;
        }

        public async Task<List<MentorReview>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MentorReviewDAO> query = DataContext.MentorReview.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<MentorReview> MentorReviews = await query.AsNoTracking()
            .Select(x => new MentorReview()
            {
                Id = x.Id,
                Description = x.Description,
                ContentReview = x.ContentReview,
                Star = x.Star,
                MentorId = x.MentorId,
                CreatorId = x.CreatorId,
                Time = x.Time,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    Password = x.Creator.Password,
                    DisplayName = x.Creator.DisplayName,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                    Avatar = x.Creator.Avatar,
                    CoverImage = x.Creator.CoverImage,
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
            

            return MentorReviews;
        }

        public async Task<MentorReview> Get(long Id)
        {
            MentorReview MentorReview = await DataContext.MentorReview.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new MentorReview()
            {
                Id = x.Id,
                Description = x.Description,
                ContentReview = x.ContentReview,
                Star = x.Star,
                MentorId = x.MentorId,
                CreatorId = x.CreatorId,
                Time = x.Time,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    Password = x.Creator.Password,
                    DisplayName = x.Creator.DisplayName,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                    Avatar = x.Creator.Avatar,
                    CoverImage = x.Creator.CoverImage,
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

            if (MentorReview == null)
                return null;

            return MentorReview;
        }
        
        public async Task<bool> Create(MentorReview MentorReview)
        {
            MentorReviewDAO MentorReviewDAO = new MentorReviewDAO();
            MentorReviewDAO.Id = MentorReview.Id;
            MentorReviewDAO.Description = MentorReview.Description;
            MentorReviewDAO.ContentReview = MentorReview.ContentReview;
            MentorReviewDAO.Star = MentorReview.Star;
            MentorReviewDAO.MentorId = MentorReview.MentorId;
            MentorReviewDAO.CreatorId = MentorReview.CreatorId;
            MentorReviewDAO.Time = MentorReview.Time;
            DataContext.MentorReview.Add(MentorReviewDAO);
            await DataContext.SaveChangesAsync();
            MentorReview.Id = MentorReviewDAO.Id;
            await SaveReference(MentorReview);
            return true;
        }

        public async Task<bool> Update(MentorReview MentorReview)
        {
            MentorReviewDAO MentorReviewDAO = DataContext.MentorReview
                .Where(x => x.Id == MentorReview.Id)
                .FirstOrDefault();
            if (MentorReviewDAO == null)
                return false;
            MentorReviewDAO.Id = MentorReview.Id;
            MentorReviewDAO.Description = MentorReview.Description;
            MentorReviewDAO.ContentReview = MentorReview.ContentReview;
            MentorReviewDAO.Star = MentorReview.Star;
            MentorReviewDAO.MentorId = MentorReview.MentorId;
            MentorReviewDAO.CreatorId = MentorReview.CreatorId;
            MentorReviewDAO.Time = MentorReview.Time;
            await DataContext.SaveChangesAsync();
            await SaveReference(MentorReview);
            return true;
        }

        public async Task<bool> Delete(MentorReview MentorReview)
        {
            await DataContext.MentorReview
                .Where(x => x.Id == MentorReview.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<MentorReview> MentorReviews)
        {
            IdFilter IdFilter = new IdFilter { In = MentorReviews.Select(x => x.Id).ToList() };
            List<MentorReviewDAO> MentorReviewDAOs = new List<MentorReviewDAO>();
            List<MentorReviewDAO> DbMentorReviewDAOs = await DataContext.MentorReview
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (MentorReview MentorReview in MentorReviews)
            {
                MentorReviewDAO MentorReviewDAO = DbMentorReviewDAOs
                        .Where(x => x.Id == MentorReview.Id)
                        .FirstOrDefault();
                if (MentorReviewDAO == null)
                {
                    MentorReviewDAO = new MentorReviewDAO();
                }
                MentorReviewDAO.Description = MentorReview.Description;
                MentorReviewDAO.ContentReview = MentorReview.ContentReview;
                MentorReviewDAO.Star = MentorReview.Star;
                MentorReviewDAO.MentorId = MentorReview.MentorId;
                MentorReviewDAO.CreatorId = MentorReview.CreatorId;
                MentorReviewDAO.Time = MentorReview.Time;
                MentorReviewDAOs.Add(MentorReviewDAO);
            }
            await DataContext.BulkMergeAsync(MentorReviewDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<MentorReview> MentorReviews)
        {
            List<long> Ids = MentorReviews.Select(x => x.Id).ToList();
            await DataContext.MentorReview
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(MentorReview MentorReview)
        {
        }
        
    }
}
