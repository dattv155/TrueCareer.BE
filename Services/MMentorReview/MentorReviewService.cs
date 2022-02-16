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

namespace TrueCareer.Services.MMentorReview
{
    public interface IMentorReviewService :  IServiceScoped
    {
        Task<int> Count(MentorReviewFilter MentorReviewFilter);
        Task<List<MentorReview>> List(MentorReviewFilter MentorReviewFilter);
        Task<MentorReview> Get(long Id);
        Task<MentorReview> Create(MentorReview MentorReview);
        Task<MentorReview> Update(MentorReview MentorReview);
        Task<MentorReview> Delete(MentorReview MentorReview);
        Task<List<MentorReview>> BulkDelete(List<MentorReview> MentorReviews);
        Task<List<MentorReview>> Import(List<MentorReview> MentorReviews);
        Task<MentorReviewFilter> ToFilter(MentorReviewFilter MentorReviewFilter);
    }

    public class MentorReviewService : BaseService, IMentorReviewService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IMentorReviewValidator MentorReviewValidator;

        public MentorReviewService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IMentorReviewValidator MentorReviewValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.MentorReviewValidator = MentorReviewValidator;
        }
        public async Task<int> Count(MentorReviewFilter MentorReviewFilter)
        {
            try
            {
                int result = await UOW.MentorReviewRepository.Count(MentorReviewFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorReviewService));
            }
            return 0;
        }

        public async Task<List<MentorReview>> List(MentorReviewFilter MentorReviewFilter)
        {
            try
            {
                List<MentorReview> MentorReviews = await UOW.MentorReviewRepository.List(MentorReviewFilter);
                return MentorReviews;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorReviewService));
            }
            return null;
        }

        public async Task<MentorReview> Get(long Id)
        {
            MentorReview MentorReview = await UOW.MentorReviewRepository.Get(Id);
            await MentorReviewValidator.Get(MentorReview);
            if (MentorReview == null)
                return null;
            return MentorReview;
        }
        
        public async Task<MentorReview> Create(MentorReview MentorReview)
        {
            if (!await MentorReviewValidator.Create(MentorReview))
                return MentorReview;

            try
            {
                await UOW.MentorReviewRepository.Create(MentorReview);
                MentorReview = await UOW.MentorReviewRepository.Get(MentorReview.Id);
                Logging.CreateAuditLog(MentorReview, new { }, nameof(MentorReviewService));
                return MentorReview;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorReviewService));
            }
            return null;
        }

        public async Task<MentorReview> Update(MentorReview MentorReview)
        {
            if (!await MentorReviewValidator.Update(MentorReview))
                return MentorReview;
            try
            {
                var oldData = await UOW.MentorReviewRepository.Get(MentorReview.Id);

                await UOW.MentorReviewRepository.Update(MentorReview);

                MentorReview = await UOW.MentorReviewRepository.Get(MentorReview.Id);
                Logging.CreateAuditLog(MentorReview, oldData, nameof(MentorReviewService));
                return MentorReview;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorReviewService));
            }
            return null;
        }

        public async Task<MentorReview> Delete(MentorReview MentorReview)
        {
            if (!await MentorReviewValidator.Delete(MentorReview))
                return MentorReview;

            try
            {
                await UOW.MentorReviewRepository.Delete(MentorReview);
                Logging.CreateAuditLog(new { }, MentorReview, nameof(MentorReviewService));
                return MentorReview;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorReviewService));
            }
            return null;
        }

        public async Task<List<MentorReview>> BulkDelete(List<MentorReview> MentorReviews)
        {
            if (!await MentorReviewValidator.BulkDelete(MentorReviews))
                return MentorReviews;

            try
            {
                await UOW.MentorReviewRepository.BulkDelete(MentorReviews);
                Logging.CreateAuditLog(new { }, MentorReviews, nameof(MentorReviewService));
                return MentorReviews;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorReviewService));
            }
            return null;

        }
        
        public async Task<List<MentorReview>> Import(List<MentorReview> MentorReviews)
        {
            if (!await MentorReviewValidator.Import(MentorReviews))
                return MentorReviews;
            try
            {
                await UOW.MentorReviewRepository.BulkMerge(MentorReviews);

                Logging.CreateAuditLog(MentorReviews, new { }, nameof(MentorReviewService));
                return MentorReviews;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorReviewService));
            }
            return null;
        }     
        
        public async Task<MentorReviewFilter> ToFilter(MentorReviewFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<MentorReviewFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                MentorReviewFilter subFilter = new MentorReviewFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = FilterBuilder.Merge(subFilter.Description, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ContentReview))
                        subFilter.ContentReview = FilterBuilder.Merge(subFilter.ContentReview, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Star))
                        subFilter.Star = FilterBuilder.Merge(subFilter.Star, FilterPermissionDefinition.IntFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.MentorId))
                        subFilter.MentorId = FilterBuilder.Merge(subFilter.MentorId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = FilterBuilder.Merge(subFilter.CreatorId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Time))
                        subFilter.Time = FilterBuilder.Merge(subFilter.Time, FilterPermissionDefinition.DateFilter);
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

        private void Sync(List<MentorReview> MentorReviews)
        {
            List<AppUser> AppUsers = new List<AppUser>();
            AppUsers.AddRange(MentorReviews.Select(x => new AppUser { Id = x.CreatorId }));
            AppUsers.AddRange(MentorReviews.Select(x => new AppUser { Id = x.MentorId }));
            
            AppUsers = AppUsers.Distinct().ToList();
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
        }

    }
}
