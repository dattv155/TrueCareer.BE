using TrueSight.Common;
using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using TrueCareer.Repositories;
using TrueCareer.Entities;
using TrueCareer.Enums;

namespace TrueCareer.Services.MComment
{
    public interface ICommentService :  IServiceScoped
    {
        Task<int> Count(CommentFilter CommentFilter);
        Task<List<Comment>> List(CommentFilter CommentFilter);
        Task<List<Comment>> List(Guid DiscussionId, OrderType OrderType);
        Task<Comment> Get(long Id);
        Task<Comment> Create(Comment Comment);
        Task<Comment> Update(Comment Comment);
        Task<Comment> Delete(Comment Comment);
        Task<List<Comment>> BulkDelete(List<Comment> Comments);
        Task<List<Comment>> Import(List<Comment> Comments);
        Task<CommentFilter> ToFilter(CommentFilter CommentFilter);
    }

    public class CommentService : BaseService, ICommentService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private ICommentValidator CommentValidator;

        public CommentService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            ICommentValidator CommentValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.CommentValidator = CommentValidator;
        }
        public async Task<int> Count(CommentFilter CommentFilter)
        {
            try
            {
                int result = await UOW.CommentRepository.Count(CommentFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(CommentService));
            }
            return 0;
        }

        public async Task<List<Comment>> List(CommentFilter CommentFilter)
        {
            try
            {
                List<Comment> Comments = await UOW.CommentRepository.List(CommentFilter);
                return Comments;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(CommentService));
            }
            return null;
        }
        
        public async Task<List<Comment>> List(Guid DiscussionId, OrderType OrderType)
        {
            try
            {
                List<Comment> Comments = await UOW.CommentRepository.List(DiscussionId, OrderType);
                return Comments;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(CommentService));
            }

            return null;
        }

        public async Task<Comment> Get(long Id)
        {
            Comment Comment = await UOW.CommentRepository.Get(Id);
            await CommentValidator.Get(Comment);
            if (Comment == null)
                return null;
            return Comment;
        }
        
        public async Task<Comment> Create(Comment Comment)
        {
            if (!await CommentValidator.Create(Comment))
                return Comment;

            try
            {
                await UOW.CommentRepository.Create(Comment);
                Comment = await UOW.CommentRepository.Get(Comment.Id);
                Logging.CreateAuditLog(Comment, new { }, nameof(CommentService));
                return Comment;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(CommentService));
            }
            return null;
        }

        public async Task<Comment> Update(Comment Comment)
        {
            if (!await CommentValidator.Update(Comment))
                return Comment;
            try
            {
                var oldData = await UOW.CommentRepository.Get(Comment.Id);

                await UOW.CommentRepository.Update(Comment);

                Comment = await UOW.CommentRepository.Get(Comment.Id);
                Logging.CreateAuditLog(Comment, oldData, nameof(CommentService));
                return Comment;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(CommentService));
            }
            return null;
        }

        public async Task<Comment> Delete(Comment Comment)
        {
            if (!await CommentValidator.Delete(Comment))
                return Comment;

            try
            {
                await UOW.CommentRepository.Delete(Comment);
                Logging.CreateAuditLog(new { }, Comment, nameof(CommentService));
                return Comment;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(CommentService));
            }
            return null;
        }

        public async Task<List<Comment>> BulkDelete(List<Comment> Comments)
        {
            if (!await CommentValidator.BulkDelete(Comments))
                return Comments;

            try
            {
                await UOW.CommentRepository.BulkDelete(Comments);
                Logging.CreateAuditLog(new { }, Comments, nameof(CommentService));
                return Comments;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(CommentService));
            }
            return null;

        }
        
        public async Task<List<Comment>> Import(List<Comment> Comments)
        {
            if (!await CommentValidator.Import(Comments))
                return Comments;
            try
            {
                await UOW.CommentRepository.BulkMerge(Comments);

                Logging.CreateAuditLog(Comments, new { }, nameof(CommentService));
                return Comments;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(CommentService));
            }
            return null;
        }     
        
        public async Task<CommentFilter> ToFilter(CommentFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CommentFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CommentFilter subFilter = new CommentFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Content))
                        subFilter.Content = FilterBuilder.Merge(subFilter.Content, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = FilterBuilder.Merge(subFilter.CreatorId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscussionId))
                        subFilter.DiscussionId = FilterBuilder.Merge(subFilter.DiscussionId, FilterPermissionDefinition.GuidFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private void Sync(List<Comment> Comments)
        {
            List<AppUser> AppUsers = new List<AppUser>();
            AppUsers.AddRange(Comments.Select(x => new AppUser { Id = x.CreatorId }));
            
            AppUsers = AppUsers.Distinct().ToList();
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
        }

    }
}
