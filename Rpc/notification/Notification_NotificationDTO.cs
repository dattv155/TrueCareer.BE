using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.notification
{
    public class Notification_NotificationDTO : DataDTO
    {
        public long Id { get; set; }
        public string TitleWeb { get; set; }
        public string ContentWeb { get; set; }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public bool Unread { get; set; }
        public DateTime Time { get; set; }
        public string LinkWebsite { get; set; }
        public Notification_AppUserDTO Recipient { get; set; }
        public Notification_AppUserDTO Sender { get; set; }
        public Guid RowId { get; set; }
        public Notification_NotificationDTO() {}
        public Notification_NotificationDTO(Notification Notification)
        {
            this.Id = Notification.Id;
            this.TitleWeb = Notification.TitleWeb;
            this.ContentWeb = Notification.ContentWeb;
            this.SenderId = Notification.SenderId;
            this.RecipientId = Notification.RecipientId;
            this.Unread = Notification.Unread;
            this.Time = Notification.Time;
            this.LinkWebsite = Notification.LinkWebsite;
            this.Recipient = Notification.Recipient == null ? null : new Notification_AppUserDTO(Notification.Recipient);
            this.Sender = Notification.Sender == null ? null : new Notification_AppUserDTO(Notification.Sender);
            this.RowId = Notification.RowId;
            this.Informations = Notification.Informations;
            this.Warnings = Notification.Warnings;
            this.Errors = Notification.Errors;
        }
    }

    public class Notification_NotificationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter TitleWeb { get; set; }
        public StringFilter ContentWeb { get; set; }
        public IdFilter SenderId { get; set; }
        public IdFilter RecipientId { get; set; }
        public DateFilter Time { get; set; }
        public StringFilter LinkWebsite { get; set; }
        public NotificationOrder OrderBy { get; set; }
    }
}
