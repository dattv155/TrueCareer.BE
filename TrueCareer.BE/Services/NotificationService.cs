using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Entities;
using TrueCareer.Helpers;
using TrueCareer.Hub;
using TrueCareer.Repositories;
using TrueSight.Common;

namespace TrueCareer.Services
{
    public interface INotificationService : IServiceScoped
    {
        Task<int> Count(NotificationFilter filter);
        Task<List<Notification>> List(NotificationFilter filter);
        Task<Notification> Get(long Id);
        Task<Notification> Create(Notification notification);
        Task<List<Notification>> BulkCreate(List<Notification> notification);
        Task Read(long UserNotificationId);
        Task<bool> Delete(long Id);
    }
    public class NotificationService : INotificationService
    {
        private readonly IUOW UOW;
        private ICurrentContext CurrentContext;
        protected IHubContext<UserNotificationHub> signalR;
        private ILogging Logging;

        public NotificationService(
           IUOW UOW,
           IHubContext<UserNotificationHub> signalR,
           ICurrentContext CurrentContext,
           ILogging Logging)

        {
            this.signalR = signalR;
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
        }

        public async Task<int> Count(NotificationFilter filter)
        {
            return await UOW.NotificationRepository.Count(filter);
        }

        public async Task<List<Notification>> List(NotificationFilter filter)
        {
            return await UOW.NotificationRepository.List(filter);
        }

        public async Task<Notification> Get(long Id)
        {
            if (Id == 0) return null;
            return await UOW.NotificationRepository.Get(Id);
        }

        public async Task<Notification> Create(Notification notification)
        {
            if (notification == null) return null;
            try
            {
                await UOW.NotificationRepository.Create(notification);
                notification = await Get(notification.Id);
                _ = signalR.Clients.User(notification.Recipient.Id.ToString()).SendAsync("Receive", notification);
                return notification;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }
            return null;
        }

        public async Task<List<Notification>> BulkCreate(List<Notification> notifications)
        {
            if (notifications == null || notifications.Count == 0) return null;
            try
            {
                await UOW.NotificationRepository.BulkCreate(notifications);
                List<long> Ids = notifications.Select(u => u.Id).ToList();
                notifications = await UOW.NotificationRepository.List(new NotificationFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids }
                });
                var Sender = await UOW.AppUserRepository.Get(notifications.Select(x => x.SenderId).FirstOrDefault());
                List<long> RecipientIds = notifications.Select(x => x.RecipientId).ToList();
                List<AppUser> Recipients = await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.ALL,
                    Id = new IdFilter { In = RecipientIds }
                });
                foreach (Notification notification in notifications)
                {
                    AppUser Recipient = Recipients.Where(x => x.Id == notification.RecipientId).FirstOrDefault();
                    _ = signalR.Clients.User(Recipient.RowId.ToString()).SendAsync("Receive", notification);
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add(nameof(notification.TitleMobile), notification.TitleMobile);
                    data.Add(nameof(notification.Id), notification.Id.ToString());
                    data.Add(nameof(notification.ContentMobile), notification.ContentMobile);
                    data.Add(nameof(notification.ContentWeb), notification.ContentWeb);
                    data.Add(nameof(notification.LinkWebsite), notification.LinkWebsite);
                    data.Add(nameof(notification.LinkMobile), notification.LinkMobile);
                    data.Add(nameof(notification.Sender), notification.Sender?.DisplayName);
                    data.Add(nameof(notification.Unread), notification.Unread.ToString());
                    data.Add(nameof(notification.Time), notification.Time.ToString("yyyy-MM-dd hh:mm:ss"));



                }

                return notifications;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }
            return null;
        }

        public async Task Read(long UserNotificationId)
        {
            try
            {
                await UOW.NotificationRepository.Read(UserNotificationId);
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }

        }

        public async Task<bool> Delete(long Id)
        {
            if (Id == 0) return false;
            try
            {
                await UOW.NotificationRepository.Delete(Id);
                return true;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NotificationService));
            }
            return false;
        }
    }
}
