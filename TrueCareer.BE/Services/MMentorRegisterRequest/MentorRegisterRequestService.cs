using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Entities;
using TrueCareer.Enums;
using TrueCareer.Helpers;
using TrueCareer.Repositories;
using TrueSight.Common;
using TrueSight.Handlers;

namespace TrueCareer.Services.MMentorRegisterRequest
{
    public interface IMentorRegisterRequestService : IServiceScoped
    {
        Task<int> Count(MentorRegisterRequestFilter MentorRegisterRequestFilter);
        Task<List<MentorRegisterRequest>> List(MentorRegisterRequestFilter MentorRegisterRequestFilter);
        Task<MentorRegisterRequest> Get(long Id);
        Task<MentorRegisterRequest> Create(MentorRegisterRequest MentorRegisterRequest);
        Task<MentorRegisterRequest> Update(MentorRegisterRequest MentorRegisterRequest);
        Task<MentorRegisterRequest> Delete(MentorRegisterRequest MentorRegisterRequest);
        Task<List<MentorRegisterRequest>> BulkDelete(List<MentorRegisterRequest> MentorRegisterRequests);
        Task<List<MentorRegisterRequest>> Import(List<MentorRegisterRequest> MentorRegisterRequests);
        Task<MentorRegisterRequestFilter> ToFilter(MentorRegisterRequestFilter MentorRegisterRequestFilter);
    }


    public class MentorRegisterRequestService : IMentorRegisterRequestService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        public MentorRegisterRequestService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
        }
        public Task<List<MentorRegisterRequest>> BulkDelete(List<MentorRegisterRequest> MentorRegisterRequests)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Count(MentorRegisterRequestFilter MentorRegisterRequestFilter)
        {
            try
            {
                int result = await UOW.MentorRegisterRequestRepository.Count(MentorRegisterRequestFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorRegisterRequestService));
            }
            return 0;
        }

        public async Task<MentorRegisterRequest> Create(MentorRegisterRequest MentorRegisterRequest)
        {
            try
            {
                await UOW.MentorRegisterRequestRepository.Create(MentorRegisterRequest);
                MentorRegisterRequest = await UOW.MentorRegisterRequestRepository.Get(MentorRegisterRequest.Id);
                Logging.CreateAuditLog(MentorRegisterRequest, new { }, nameof(MentorRegisterRequestService));
                return MentorRegisterRequest;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorRegisterRequestService));
            }
            return null;
        }

        public async Task<MentorRegisterRequest> Delete(MentorRegisterRequest MentorRegisterRequest)
        {
            try
            {
                await UOW.MentorRegisterRequestRepository.Delete(MentorRegisterRequest);
                Logging.CreateAuditLog(new { }, MentorRegisterRequest, nameof(MentorRegisterRequestService));
                return MentorRegisterRequest;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorRegisterRequestService));
            }
            return null;
        }

        public async Task<MentorRegisterRequest> Get(long Id)
        {
            MentorRegisterRequest MentorRegisterRequest = await UOW.MentorRegisterRequestRepository.Get(Id);
            if (MentorRegisterRequest == null)
                return null;
            return MentorRegisterRequest;
        }

        public Task<List<MentorRegisterRequest>> Import(List<MentorRegisterRequest> MentorRegisterRequests)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MentorRegisterRequest>> List(MentorRegisterRequestFilter MentorRegisterRequestFilter)
        {
            try
            {
                List<MentorRegisterRequest> MentorRegisterRequests = await UOW.MentorRegisterRequestRepository.List(MentorRegisterRequestFilter);
                return MentorRegisterRequests;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorRegisterRequest));
            }
            return null;
        }

        public async Task<MentorRegisterRequestFilter> ToFilter(MentorRegisterRequestFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<MentorRegisterRequestFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                MentorRegisterRequestFilter subFilter = new MentorRegisterRequestFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UserId))
                        subFilter.UserId = FilterBuilder.Merge(subFilter.UserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TopicId))
                        subFilter.TopicId = FilterBuilder.Merge(subFilter.TopicId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.MentorApprovalStatusId))
                        subFilter.MentorApprovalStatusId = FilterBuilder.Merge(subFilter.MentorApprovalStatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
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

        public async Task<MentorRegisterRequest> Update(MentorRegisterRequest MentorRegisterRequest)
        {
            try
            {
                var oldData = await UOW.MentorRegisterRequestRepository.Get(MentorRegisterRequest.Id);

                await UOW.MentorRegisterRequestRepository.Update(MentorRegisterRequest);

                MentorRegisterRequest = await UOW.MentorRegisterRequestRepository.Get(MentorRegisterRequest.Id);
                Logging.CreateAuditLog(MentorRegisterRequest, oldData, nameof(MentorRegisterRequestService));
                return MentorRegisterRequest;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorRegisterRequestService));
            }
            return null;
        }
    }
}
