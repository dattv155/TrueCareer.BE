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
    public interface ICommentRepository
    {
        Task<int> CountAll(CommentFilter CommentFilter);
        Task<int> Count(CommentFilter CommentFilter);
        Task<List<Comment>> List(CommentFilter CommentFilter);
        Task<List<Comment>> List(List<long> Ids);
        Task<Comment> Get(long Id);
        Task<bool> Create(Comment Comment);
        Task<bool> Update(Comment Comment);
        Task<bool> Delete(Comment Comment);
        Task<bool> BulkMerge(List<Comment> Comments);
        Task<bool> BulkDelete(List<Comment> Comments);
    }
    public class CommentRepository : ICommentRepository
    {
        private DataContext DataContext;
        public CommentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<CommentDAO>> DynamicFilter(IQueryable<CommentDAO> query, CommentFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Content, filter.Content);
            query = query.Where(q => q.CreatorId, filter.CreatorId);
            return query;
        }

        private async Task<IQueryable<CommentDAO>> OrFilter(IQueryable<CommentDAO> query, CommentFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<CommentDAO> initQuery = query.Where(q => false);
            foreach (CommentFilter CommentFilter in filter.OrFilter)
            {
                IQueryable<CommentDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, CommentFilter.Id);
                queryable = queryable.Where(q => q.Content, CommentFilter.Content);
                queryable = queryable.Where(q => q.CreatorId, CommentFilter.CreatorId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<CommentDAO> DynamicOrder(IQueryable<CommentDAO> query, CommentFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case CommentOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case CommentOrder.Content:
                            query = query.OrderBy(q => q.Content);
                            break;
                        case CommentOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case CommentOrder.Discussion:
                            query = query.OrderBy(q => q.DiscussionId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case CommentOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case CommentOrder.Content:
                            query = query.OrderByDescending(q => q.Content);
                            break;
                        case CommentOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case CommentOrder.Discussion:
                            query = query.OrderByDescending(q => q.DiscussionId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Comment>> DynamicSelect(IQueryable<CommentDAO> query, CommentFilter filter)
        {
            List<Comment> Comments = await query.Select(q => new Comment()
            {
                Id = filter.Selects.Contains(CommentSelect.Id) ? q.Id : default(long),
                Content = filter.Selects.Contains(CommentSelect.Content) ? q.Content : default(string),
                CreatorId = filter.Selects.Contains(CommentSelect.Creator) ? q.CreatorId : default(long),
                DiscussionId = filter.Selects.Contains(CommentSelect.Discussion) ? q.DiscussionId : default(Guid),
                Creator = filter.Selects.Contains(CommentSelect.Creator) && q.Creator != null ? new AppUser
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
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return Comments;
        }

        public async Task<int> CountAll(CommentFilter filter)
        {
            IQueryable<CommentDAO> CommentDAOs = DataContext.Comment.AsNoTracking();
            CommentDAOs = await DynamicFilter(CommentDAOs, filter);
            return await CommentDAOs.CountAsync();
        }

        public async Task<int> Count(CommentFilter filter)
        {
            IQueryable<CommentDAO> CommentDAOs = DataContext.Comment.AsNoTracking();
            CommentDAOs = await DynamicFilter(CommentDAOs, filter);
            CommentDAOs = await OrFilter(CommentDAOs, filter);
            return await CommentDAOs.CountAsync();
        }

        public async Task<List<Comment>> List(CommentFilter filter)
        {
            if (filter == null) return new List<Comment>();
            IQueryable<CommentDAO> CommentDAOs = DataContext.Comment.AsNoTracking();
            CommentDAOs = await DynamicFilter(CommentDAOs, filter);
            CommentDAOs = await OrFilter(CommentDAOs, filter);
            CommentDAOs = DynamicOrder(CommentDAOs, filter);
            List<Comment> Comments = await DynamicSelect(CommentDAOs, filter);
            return Comments;
        }

        public async Task<List<Comment>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<CommentDAO> query = DataContext.Comment.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Comment> Comments = await query.AsNoTracking()
            .Select(x => new Comment()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Content = x.Content,
                CreatorId = x.CreatorId,
                DiscussionId = x.DiscussionId,
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
            }).ToListAsync();
            

            return Comments;
        }

        public async Task<Comment> Get(long Id)
        {
            Comment Comment = await DataContext.Comment.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Comment()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Content = x.Content,
                CreatorId = x.CreatorId,
                DiscussionId = x.DiscussionId,
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
            }).FirstOrDefaultAsync();

            if (Comment == null)
                return null;

            return Comment;
        }
        
        public async Task<bool> Create(Comment Comment)
        {
            CommentDAO CommentDAO = new CommentDAO();
            CommentDAO.Id = Comment.Id;
            CommentDAO.Content = Comment.Content;
            CommentDAO.CreatorId = Comment.CreatorId;
            CommentDAO.DiscussionId = Comment.DiscussionId;
            CommentDAO.CreatedAt = StaticParams.DateTimeNow;
            CommentDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Comment.Add(CommentDAO);
            await DataContext.SaveChangesAsync();
            Comment.Id = CommentDAO.Id;
            await SaveReference(Comment);
            return true;
        }

        public async Task<bool> Update(Comment Comment)
        {
            CommentDAO CommentDAO = DataContext.Comment
                .Where(x => x.Id == Comment.Id)
                .FirstOrDefault();
            if (CommentDAO == null)
                return false;
            CommentDAO.Id = Comment.Id;
            CommentDAO.Content = Comment.Content;
            CommentDAO.CreatorId = Comment.CreatorId;
            CommentDAO.DiscussionId = Comment.DiscussionId;
            CommentDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Comment);
            return true;
        }

        public async Task<bool> Delete(Comment Comment)
        {
            await DataContext.Comment
                .Where(x => x.Id == Comment.Id)
                .UpdateFromQueryAsync(x => new CommentDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Comment> Comments)
        {
            IdFilter IdFilter = new IdFilter { In = Comments.Select(x => x.Id).ToList() };
            List<CommentDAO> CommentDAOs = new List<CommentDAO>();
            List<CommentDAO> DbCommentDAOs = await DataContext.Comment
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Comment Comment in Comments)
            {
                CommentDAO CommentDAO = DbCommentDAOs
                        .Where(x => x.Id == Comment.Id)
                        .FirstOrDefault();
                if (CommentDAO == null)
                {
                    CommentDAO = new CommentDAO();
                    CommentDAO.CreatedAt = StaticParams.DateTimeNow;
                }
                CommentDAO.Content = Comment.Content;
                CommentDAO.CreatorId = Comment.CreatorId;
                CommentDAO.DiscussionId = Comment.DiscussionId;
                CommentDAO.UpdatedAt = StaticParams.DateTimeNow;
                CommentDAOs.Add(CommentDAO);
            }
            await DataContext.BulkMergeAsync(CommentDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Comment> Comments)
        {
            List<long> Ids = Comments.Select(x => x.Id).ToList();
            await DataContext.Comment
                .WhereBulkContains(Ids, x => x.Id)
                .UpdateFromQueryAsync(x => new CommentDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(Comment Comment)
        {
        }
        
    }
}
