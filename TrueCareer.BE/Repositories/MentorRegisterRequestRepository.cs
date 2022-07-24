using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.BE.Models;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Repositories
{
    public interface IMentorRegisterRequestRepository
    {
        Task<int> CountAll(MentorRegisterRequestFilter MentorRegisterRequestFilter);
        Task<int> Count(MentorRegisterRequestFilter MentorRegisterRequestFilter);
        Task<List<MentorRegisterRequest>> List(MentorRegisterRequestFilter MentorRegisterRequestFilter);
        Task<List<MentorRegisterRequest>> List(List<long> Ids);
        Task<MentorRegisterRequest> Get(long Id);
        Task<bool> Create(MentorRegisterRequest MentorRegisterRequest);
        Task<bool> Update(MentorRegisterRequest MentorRegisterRequest);
        Task<bool> Delete(MentorRegisterRequest MentorRegisterRequest);
        Task<bool> BulkMerge(List<MentorRegisterRequest> MentorRegisterRequests);
        Task<bool> BulkDelete(List<MentorRegisterRequest> MentorRegisterRequests);
    }
    public class MentorRegisterRequestRepository : IMentorRegisterRequestRepository
    {
        private DataContext DataContext;
        public MentorRegisterRequestRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }
        private async Task<IQueryable<MentorRegisterRequestDAO>> DynamicFilter(IQueryable<MentorRegisterRequestDAO> query, MentorRegisterRequestFilter filter)
        {
            if (filter == null)
            {
                return query.Where(q => false);
            }
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.UserId, filter.UserId);
            query = query.Where(q => q.MentorApprovalStatusId, filter.MentorApprovalStatusId);
            query = query.Where(q => q.TopicId, filter.TopicId);
            return query;
        }

        private async Task<IQueryable<MentorRegisterRequestDAO>> OrFilter(IQueryable<MentorRegisterRequestDAO> query, MentorRegisterRequestFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MentorRegisterRequestDAO> initQuery = query.Where(q => false);
            foreach (MentorRegisterRequestFilter MentorRegisterRequestFilter in filter.OrFilter)
            {
                IQueryable<MentorRegisterRequestDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.UserId, filter.UserId);
                queryable = queryable.Where(q => q.MentorApprovalStatusId, filter.MentorApprovalStatusId);
                queryable = queryable.Where(q => q.TopicId, filter.TopicId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<MentorRegisterRequestDAO> DynamicOrder(IQueryable<MentorRegisterRequestDAO> query, MentorRegisterRequestFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MentorRegisterRequestOrder.User:
                            query = query.OrderBy(q => q.UserId);
                            break;

                        case MentorRegisterRequestOrder.MentorApprovalStatus:
                            query = query.OrderBy(q => q.MentorApprovalStatusId);
                            break;

                        case MentorRegisterRequestOrder.Topic:
                            query = query.OrderBy(q => q.TopicId);
                            break;

                        case MentorRegisterRequestOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MentorRegisterRequestOrder.User:
                            query = query.OrderByDescending(q => q.UserId);
                            break;

                        case MentorRegisterRequestOrder.MentorApprovalStatus:
                            query = query.OrderByDescending(q => q.MentorApprovalStatusId);
                            break;

                        case MentorRegisterRequestOrder.Topic:
                            query = query.OrderByDescending(q => q.TopicId);
                            break;

                        case MentorRegisterRequestOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<MentorRegisterRequest>> DynamicSelect(IQueryable<MentorRegisterRequestDAO> query, MentorRegisterRequestFilter filter)
        {
            List<MentorRegisterRequest> MentorRegisterRequests = await query.Select(q => new MentorRegisterRequest()
            {
                UserId = filter.Selects.Contains(MentorRegisterRequestSelect.User) ? q.UserId : default(long),
                TopicId = filter.Selects.Contains(MentorRegisterRequestSelect.Topic) ? q.TopicId : default(long),
                MentorApprovalStatusId = filter.Selects.Contains(MentorRegisterRequestSelect.MentorApprovalStatus) ? q.MentorApprovalStatusId : default(long),
                Id = filter.Selects.Contains(MentorRegisterRequestSelect.Id) ? q.Id : default(long),
                Topic = filter.Selects.Contains(MentorRegisterRequestSelect.Topic) && q.Topic != null ? new Topic
                {
                    Id = q.Topic.Id,
                    Title = q.Topic.Title,
                    Description = q.Topic.Description,
                    Cost = q.Topic.Cost,
                } : null,
                MentorApprovalStatus = filter.Selects.Contains(MentorRegisterRequestSelect.MentorApprovalStatus) && q.MentorApprovalStatus != null ? new MentorApprovalStatus
                {
                    Id = q.MentorApprovalStatus.Id,
                    Code = q.MentorApprovalStatus.Code,
                    Name = q.MentorApprovalStatus.Name,
                } : null,
                User = filter.Selects.Contains(MentorRegisterRequestSelect.User) && q.User != null ? new AppUser
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
            return MentorRegisterRequests;
        }
        public Task<bool> BulkDelete(List<MentorRegisterRequest> MentorRegisterRequests)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkMerge(List<MentorRegisterRequest> MentorRegisterRequests)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Count(MentorRegisterRequestFilter filter)
        {
            IQueryable<MentorRegisterRequestDAO> MentorRegisterRequestDAOs = DataContext.MentorRegisterRequest.AsNoTracking();
            MentorRegisterRequestDAOs = await DynamicFilter(MentorRegisterRequestDAOs, filter);
            MentorRegisterRequestDAOs = await OrFilter(MentorRegisterRequestDAOs, filter);
            return await MentorRegisterRequestDAOs.CountAsync();
        }

        public async Task<int> CountAll(MentorRegisterRequestFilter filter)
        {
            IQueryable<MentorRegisterRequestDAO> MentorRegisterRequestDAOs = DataContext.MentorRegisterRequest.AsNoTracking();
            MentorRegisterRequestDAOs = await DynamicFilter(MentorRegisterRequestDAOs, filter);
            return await MentorRegisterRequestDAOs.CountAsync();
        }

        public async Task<bool> Create(MentorRegisterRequest MentorRegisterRequest)
        {
            MentorRegisterRequestDAO MentorRegisterRequestDAO = new MentorRegisterRequestDAO();
            MentorRegisterRequestDAO.UserId = MentorRegisterRequest.UserId;
            MentorRegisterRequestDAO.TopicId = MentorRegisterRequest.TopicId;
            MentorRegisterRequestDAO.MentorApprovalStatusId = MentorRegisterRequest.MentorApprovalStatusId;
            MentorRegisterRequestDAO.Id = MentorRegisterRequest.Id;
            DataContext.MentorRegisterRequest.Add(MentorRegisterRequestDAO);
            await DataContext.SaveChangesAsync();
            MentorRegisterRequest.Id = MentorRegisterRequestDAO.Id;
            await SaveReference(MentorRegisterRequest);
            return true;
        }

        public async Task<bool> Delete(MentorRegisterRequest MentorRegisterRequest)
        {
            await DataContext.MentorRegisterRequest
    .Where(x => x.Id == MentorRegisterRequest.Id)
    .DeleteFromQueryAsync();
            return true;
        }

        public async Task<MentorRegisterRequest> Get(long Id)
        {

            MentorRegisterRequest MentorRegisterRequest = await DataContext.MentorRegisterRequest.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new MentorRegisterRequest()
            {
                UserId = x.UserId,
                TopicId = x.TopicId,
                MentorApprovalStatusId = x.MentorApprovalStatusId,
                Id = x.Id,
                Topic = x.Topic == null ? null : new Topic
                {
                    Id = x.Topic.Id,
                    Title = x.Topic.Title,
                    Description = x.Topic.Description,
                    Cost = x.Topic.Cost,
                },
                MentorApprovalStatus = x.MentorApprovalStatus == null ? null : new MentorApprovalStatus
                {
                    Id = x.MentorApprovalStatus.Id,
                    Code = x.MentorApprovalStatus.Code,
                    Name = x.MentorApprovalStatus.Name,
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

            if (MentorRegisterRequest == null)
                return null;

            return MentorRegisterRequest;
        }

        public async Task<List<MentorRegisterRequest>> List(MentorRegisterRequestFilter filter)
        {
            if (filter == null) return new List<MentorRegisterRequest>();
            IQueryable<MentorRegisterRequestDAO> MentorRegisterRequestDAOs = DataContext.MentorRegisterRequest.AsNoTracking();
            MentorRegisterRequestDAOs = await DynamicFilter(MentorRegisterRequestDAOs, filter);
            MentorRegisterRequestDAOs = await OrFilter(MentorRegisterRequestDAOs, filter);
            MentorRegisterRequestDAOs = DynamicOrder(MentorRegisterRequestDAOs, filter);
            List<MentorRegisterRequest> MentorRegisterRequests = await DynamicSelect(MentorRegisterRequestDAOs, filter);
            return MentorRegisterRequests;
        }

        public async Task<List<MentorRegisterRequest>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MentorRegisterRequestDAO> query = DataContext.MentorRegisterRequest.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<MentorRegisterRequest> MentorRegisterRequests = await query.AsNoTracking()
            .Select(x => new MentorRegisterRequest()
            {
                UserId = x.UserId,
                TopicId = x.TopicId,

                MentorApprovalStatusId = x.MentorApprovalStatusId,

                Id = x.Id,
                Topic = x.Topic == null ? null : new Topic
                {
                    Id = x.Topic.Id,
                    Title = x.Topic.Title,
                    Description = x.Topic.Description,
                    Cost = x.Topic.Cost,
                },
                MentorApprovalStatus = x.MentorApprovalStatus == null ? null : new MentorApprovalStatus
                {
                    Id = x.MentorApprovalStatus.Id,
                    Code = x.MentorApprovalStatus.Code,
                    Name = x.MentorApprovalStatus.Name,
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


            return MentorRegisterRequests;
        }

        public async Task<bool> Update(MentorRegisterRequest MentorRegisterRequest)
        {
            MentorRegisterRequestDAO MentorRegisterRequestDAO = DataContext.MentorRegisterRequest
              .Where(x => x.Id == MentorRegisterRequest.Id)
              .FirstOrDefault();
            if (MentorRegisterRequestDAO == null)
                return false;
            MentorRegisterRequestDAO.UserId = MentorRegisterRequest.UserId;
            MentorRegisterRequestDAO.TopicId = MentorRegisterRequest.TopicId;
            MentorRegisterRequestDAO.MentorApprovalStatusId = MentorRegisterRequest.MentorApprovalStatusId;
            MentorRegisterRequestDAO.Id = MentorRegisterRequest.Id;
            await DataContext.SaveChangesAsync();
            return true;
        }
        private async Task SaveReference(MentorRegisterRequest MentorRegisterRequest)
        {
        }
    }

}
