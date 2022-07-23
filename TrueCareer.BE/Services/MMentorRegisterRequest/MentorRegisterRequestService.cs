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
using TrueCareer.Services.MAppUser;
using TrueCareer.Services;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;

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
        Task<MentorRegisterRequest> Approve(MentorRegisterRequest MentorRegisterRequest);
        Task<MentorRegisterRequest> Reject(MentorRegisterRequest MentorRegisterRequest);
    }


    public class MentorRegisterRequestService : IMentorRegisterRequestService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
   
        private INotificationService NotificationService;
        public MentorRegisterRequestService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            ILogging Logging,
            INotificationService NotificationService
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
            this.NotificationService = NotificationService;
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
        public async Task<MentorRegisterRequest> Approve(MentorRegisterRequest MentorRegisterRequest)
        {
            try
            {
                MentorRegisterRequest = await UOW.MentorRegisterRequestRepository.Get(MentorRegisterRequest.Id);
                MentorRegisterRequest.MentorApprovalStatusId = MentorApprovalStatusEnum.APPROVE.Id;
                await UOW.MentorRegisterRequestRepository.Update(MentorRegisterRequest);
                // change role id of mentee to mentor
                AppUser AppUser = await UOW.AppUserRepository.Get(MentorRegisterRequest.UserId);
                AppUser.RoleId = RoleEnum.MENTOR.Id;
                await UOW.AppUserRepository.Update(AppUser);
                // send notification to web and mobile
                TrueCareer.Entities.Notification UserNotification = new TrueCareer.Entities.Notification
                {
                    TitleWeb = "Hồ sơ Mentor được phê duyệt",
                    ContentWeb = "TrueCareer chúc mừng bạn đã được chấp nhận làm Mentor của hệ thống. " +
                    "Mong rằng bạn sẽ góp hết sức mình cho công tác hướng nghiệp!",
                    TitleMobile = "Hồ sơ Mentor được phê duyệt",
                    ContentMobile = "TrueCareer chúc mừng bạn đã được chấp nhận làm Mentor của hệ thống. " +
                    "Mong rằng bạn sẽ góp hết sức mình cho công tác hướng nghiệp!",
                    RecipientId = MentorRegisterRequest.UserId,
                    SenderId = 1,
                    Time = StaticParams.DateTimeNow,
                    Unread = false
                };
                await NotificationService.Create(UserNotification);

                // send push notification to mobile
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add(nameof(UserNotification.Id), UserNotification.Id.ToString());
                data.Add(nameof(UserNotification.ContentMobile), UserNotification.ContentMobile);
                data.Add(nameof(UserNotification.LinkMobile), UserNotification.LinkMobile);
                data.Add(nameof(UserNotification.Unread), UserNotification.Unread.ToString());
                data.Add(nameof(UserNotification.Time), UserNotification.Time.ToString("yyyy-MM-dd hh:mm:ss"));

                var message = new FirebaseAdmin.Messaging.Message()
                {
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = UserNotification.TitleMobile,
                        Body = UserNotification.ContentMobile,
                    },
                    Data = data,
                    Token = UserNotification.Recipient.Token,
                };

                var messaging = FirebaseMessaging.DefaultInstance;
                _ = messaging.SendAsync(message);
                
                return MentorRegisterRequest;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorRegisterRequestService));
            }
            return null;
            
        }

        public async Task<MentorRegisterRequest> Reject(MentorRegisterRequest MentorRegisterRequest)
        {
            try
            {
                MentorRegisterRequest = await UOW.MentorRegisterRequestRepository.Get(MentorRegisterRequest.Id);
                MentorRegisterRequest.MentorApprovalStatusId = MentorApprovalStatusEnum.REJECT.Id;
                await UOW.MentorRegisterRequestRepository.Update(MentorRegisterRequest);
                // send notification to web and mobile
                TrueCareer.Entities.Notification UserNotification = new TrueCareer.Entities.Notification
                {
                    TitleWeb = "Hồ sơ Mentor bị từ chối",
                    ContentWeb = "TrueCareer rất tiếc phải thông báo rằng hồ sơ của bạn chưa phù hợp để trở thành Mentor.",
                    TitleMobile = "Hồ sơ Mentor bị từ chối",
                    ContentMobile = "TrueCareer rất tiếc phải thông báo rằng hồ sơ của bạn chưa phù hợp để trở thành Mentor.",
                    RecipientId = MentorRegisterRequest.UserId,
                    SenderId = 1,
                    Time = StaticParams.DateTimeNow,
                    Unread = false
                };
                await NotificationService.Create(UserNotification);

                // send push notification to mobile
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add(nameof(UserNotification.Id), UserNotification.Id.ToString());
                data.Add(nameof(UserNotification.ContentMobile), UserNotification.ContentMobile);
                data.Add(nameof(UserNotification.LinkMobile), UserNotification.LinkMobile);
                data.Add(nameof(UserNotification.Unread), UserNotification.Unread.ToString());
                data.Add(nameof(UserNotification.Time), UserNotification.Time.ToString("yyyy-MM-dd hh:mm:ss"));

                var message = new FirebaseAdmin.Messaging.Message()
                {
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = UserNotification.TitleMobile,
                        Body = UserNotification.ContentMobile,
                    },
                    Data = data,
                    Token = UserNotification.Recipient.Token,
                };

                var messaging = FirebaseMessaging.DefaultInstance;
                _ = messaging.SendAsync(message);

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
