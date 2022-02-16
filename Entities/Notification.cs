using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Notification : DataEntity,  IEquatable<Notification>
    {
        public long Id { get; set; }
        public string TitleWeb { get; set; }
        public string ContentWeb { get; set; }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public bool Unread { get; set; }
        public DateTime Time { get; set; }
        public string LinkWebsite { get; set; }
        public AppUser Recipient { get; set; }
        public AppUser Sender { get; set; }
        public Guid RowId { get; set; }
        
        public bool Equals(Notification other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.TitleWeb != other.TitleWeb) return false;
            if (this.ContentWeb != other.ContentWeb) return false;
            if (this.SenderId != other.SenderId) return false;
            if (this.RecipientId != other.RecipientId) return false;
            if (this.Unread != other.Unread) return false;
            if (this.Time != other.Time) return false;
            if (this.LinkWebsite != other.LinkWebsite) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class NotificationFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter TitleWeb { get; set; }
        public StringFilter ContentWeb { get; set; }
        public IdFilter SenderId { get; set; }
        public IdFilter RecipientId { get; set; }
        public bool? Unread { get; set; }
        public DateFilter Time { get; set; }
        public StringFilter LinkWebsite { get; set; }
        public List<NotificationFilter> OrFilter { get; set; }
        public NotificationOrder OrderBy {get; set;}
        public NotificationSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationOrder
    {
        Id = 0,
        TitleWeb = 1,
        ContentWeb = 2,
        Sender = 3,
        Recipient = 4,
        Unread = 5,
        Time = 6,
        LinkWebsite = 7,
    }

    [Flags]
    public enum NotificationSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        TitleWeb = E._1,
        ContentWeb = E._2,
        Sender = E._3,
        Recipient = E._4,
        Unread = E._5,
        Time = E._6,
        LinkWebsite = E._7,
    }
}
