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

namespace TrueCareer.Services.MNotification
{
    public interface INotificationService :  IServiceScoped
    {
        Task<int> Count(NotificationFilter NotificationFilter);
        Task<List<Notification>> List(NotificationFilter NotificationFilter);
        Task<Notification> Get(long Id);
        Task<Notification> Create(Notification Notification);
        Task<Notification> Update(Notification Notification);
        Task<Notification> Delete(Notification Notification);
        Task<List<Notification>> BulkDelete(List<Notification> Notifications);
        Task<List<Notification>> Import(List<Notification> Notifications);
        Task<NotificationFilter> ToFilter(NotificationFilter NotificationFilter);
    }

    public class NotificationService : BaseService, INotificationService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private INotificationValidator NotificationValidator;

        public NotificationService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            INotificationValidator NotificationValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.NotificationValidator = NotificationValidator;
        }
        public async Task<int> Count(NotificationFilter NotificationFilter)
        {
            try
            {
                int result = await UOW.NotificationRepository.Count(NotificationFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }
            return 0;
        }

        public async Task<List<Notification>> List(NotificationFilter NotificationFilter)
        {
            try
            {
                List<Notification> Notifications = await UOW.NotificationRepository.List(NotificationFilter);
                return Notifications;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }
            return null;
        }

        public async Task<Notification> Get(long Id)
        {
            Notification Notification = await UOW.NotificationRepository.Get(Id);
            await NotificationValidator.Get(Notification);
            if (Notification == null)
                return null;
            return Notification;
        }
        
        public async Task<Notification> Create(Notification Notification)
        {
            if (!await NotificationValidator.Create(Notification))
                return Notification;

            try
            {
                await UOW.NotificationRepository.Create(Notification);
                Notification = await UOW.NotificationRepository.Get(Notification.Id);
                Logging.CreateAuditLog(Notification, new { }, nameof(NotificationService));
                return Notification;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }
            return null;
        }

        public async Task<Notification> Update(Notification Notification)
        {
            if (!await NotificationValidator.Update(Notification))
                return Notification;
            try
            {
                var oldData = await UOW.NotificationRepository.Get(Notification.Id);

                await UOW.NotificationRepository.Update(Notification);

                Notification = await UOW.NotificationRepository.Get(Notification.Id);
                Logging.CreateAuditLog(Notification, oldData, nameof(NotificationService));
                return Notification;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }
            return null;
        }

        public async Task<Notification> Delete(Notification Notification)
        {
            if (!await NotificationValidator.Delete(Notification))
                return Notification;

            try
            {
                await UOW.NotificationRepository.Delete(Notification);
                Logging.CreateAuditLog(new { }, Notification, nameof(NotificationService));
                return Notification;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }
            return null;
        }

        public async Task<List<Notification>> BulkDelete(List<Notification> Notifications)
        {
            if (!await NotificationValidator.BulkDelete(Notifications))
                return Notifications;

            try
            {
                await UOW.NotificationRepository.BulkDelete(Notifications);
                Logging.CreateAuditLog(new { }, Notifications, nameof(NotificationService));
                return Notifications;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }
            return null;

        }
        
        public async Task<List<Notification>> Import(List<Notification> Notifications)
        {
            if (!await NotificationValidator.Import(Notifications))
                return Notifications;
            try
            {
                await UOW.NotificationRepository.BulkMerge(Notifications);

                Logging.CreateAuditLog(Notifications, new { }, nameof(NotificationService));
                return Notifications;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }
            return null;
        }     
        
        public async Task<NotificationFilter> ToFilter(NotificationFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<NotificationFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                NotificationFilter subFilter = new NotificationFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TitleWeb))
                        subFilter.TitleWeb = FilterBuilder.Merge(subFilter.TitleWeb, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ContentWeb))
                        subFilter.ContentWeb = FilterBuilder.Merge(subFilter.ContentWeb, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SenderId))
                        subFilter.SenderId = FilterBuilder.Merge(subFilter.SenderId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RecipientId))
                        subFilter.RecipientId = FilterBuilder.Merge(subFilter.RecipientId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Time))
                        subFilter.Time = FilterBuilder.Merge(subFilter.Time, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.LinkWebsite))
                        subFilter.LinkWebsite = FilterBuilder.Merge(subFilter.LinkWebsite, FilterPermissionDefinition.StringFilter);
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

        private void Sync(List<Notification> Notifications)
        {
            List<AppUser> AppUsers = new List<AppUser>();
            AppUsers.AddRange(Notifications.Select(x => new AppUser { Id = x.RecipientId }));
            AppUsers.AddRange(Notifications.Select(x => new AppUser { Id = x.SenderId }));
            
            AppUsers = AppUsers.Distinct().ToList();
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
        }

    }
}
