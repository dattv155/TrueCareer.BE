using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueCareer.Common;
using Microsoft.AspNetCore.Mvc;
using TrueCareer.Entities;
using TrueCareer.Services;

namespace TrueCareer.Rpc.notification
{
    public class NotificationRoute : Root
    {
        public const string Base = Rpc + Module + "/notification";
        public const string Create = Base + "/create";
        public const string Read = Base + "/Read";
        public const string Count = Base + "/count";
        public const string CountUnread = Base + "/count-unread";
        public const string CountRead = Base + "/count-read";
        public const string Get = Base + "/get";
        public const string List = Base + "/list";
        public const string ListUnread = Base + "/list-unread";
        public const string ListRead = Base + "/list-read";
        public const string Delete = Base + "/delete";
        public const string BulkCreate = Base + "/bulk-create";
    }
    public class NotificationController : RpcController
    {
        private readonly INotificationService UserNotificationService;
        private readonly ICurrentContext CurrentContext;
        public NotificationController(INotificationService UserNotificationService, ICurrentContext CurrentContext)
        {
            this.UserNotificationService = UserNotificationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(NotificationRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] NotificationFilter filter)
        {
            if (filter == null) filter = new NotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            return await UserNotificationService.Count(filter);
        }

        [Route(NotificationRoute.CountUnread), HttpPost]
        public async Task<int> CountUnread([FromBody] NotificationFilter filter)
        {
            if (filter == null) filter = new NotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            filter.Unread = true;
            return await UserNotificationService.Count(filter);
        }

        [Route(NotificationRoute.CountRead), HttpPost]
        public async Task<int> CountRead([FromBody] NotificationFilter filter)
        {
            if (filter == null) filter = new NotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            filter.Unread = false;
            return await UserNotificationService.Count(filter);
        }

        [Route(NotificationRoute.List), HttpPost]
        public async Task<List<Notification>> List([FromBody] NotificationFilter filter)
        {
            if (filter == null) filter = new NotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            filter.OrderBy = NotificationOrder.Time;
            filter.OrderType = OrderType.DESC;
            filter.Selects = NotificationSelect.ALL; // fucking this make me crazy all day :)
            return await UserNotificationService.List(filter);
        }

        [Route(NotificationRoute.ListUnread), HttpPost]
        public async Task<List<Notification>> ListUnread([FromBody] NotificationFilter filter)
        {
            if (filter == null) filter = new NotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            filter.Unread = true;
            filter.OrderBy = NotificationOrder.Time;
            filter.OrderType = OrderType.DESC;
            return await UserNotificationService.List(filter);
        }

        [Route(NotificationRoute.ListRead), HttpPost]
        public async Task<List<Notification>> ListRead([FromBody] NotificationFilter filter)
        {
            if (filter == null) filter = new NotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            filter.Unread = false;
            filter.OrderBy = NotificationOrder.Id;
            filter.OrderType = OrderType.DESC;
            return await UserNotificationService.List(filter);
        }

        [Route(NotificationRoute.Get), HttpPost]
        public async Task<Notification> Get([FromBody] Notification UserNotification)
        {
            if (!ModelState.IsValid)
                throw new BindException(UserNotification);
            await CheckPermission(UserNotification);
            return await UserNotificationService.Get(UserNotification.Id);
        }

        [Route(NotificationRoute.Create), HttpPost]
        public async Task<ActionResult<Notification>> Create([FromBody] Notification UserNotification)
        {
            if (UserNotification == null) UserNotification = new Notification();
            UserNotification = await UserNotificationService.Create(UserNotification);
            if (UserNotification == null)
                return BadRequest(UserNotification);
            return Ok(UserNotification);
        }

        [Route(NotificationRoute.Read), HttpPost]
        public async Task<ActionResult> Read([FromBody] Notification UserNotification)
        {
            await UserNotificationService.Read(UserNotification.Id);
            return Ok();
        }


        [Route(NotificationRoute.Delete), HttpPost]
        public async Task<ActionResult<bool>> Delete([FromBody] Notification notification)
        {
            if (notification == null) notification = new Notification();
            await CheckPermission(notification);
            return await UserNotificationService.Delete(notification.Id);
        }

        [Route(NotificationRoute.BulkCreate), HttpPost]
        public async Task<ActionResult<Notification>> BulkCreate([FromBody] List<Notification> UserNotifications)
        {
            if (UserNotifications == null) UserNotifications = new List<Notification>();
            UserNotifications = await UserNotificationService.BulkCreate(UserNotifications);
            if (UserNotifications == null)
                return BadRequest(UserNotifications);
            return Ok(UserNotifications);
        }

        private async Task CheckPermission(Notification notification)
        {
            NotificationFilter filter = new NotificationFilter
            {
                Id = new IdFilter { Equal = notification.Id },
                SenderId = new IdFilter { Equal = CurrentContext.UserId },
            };
            int count = await UserNotificationService.Count(filter);
            if (count == 0)
                throw new Exception();
        }
    }
}

