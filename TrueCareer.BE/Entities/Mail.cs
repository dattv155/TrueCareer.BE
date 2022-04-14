using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Mail : DataEntity,  IEquatable<Mail>
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Recipients { get; set; }
        public string BccRecipients { get; set; }
        public string CcRecipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public long RetryCount { get; set; }
        public string Error { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Mail other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Username != other.Username) return false;
            if (this.Password != other.Password) return false;
            if (this.Recipients != other.Recipients) return false;
            if (this.BccRecipients != other.BccRecipients) return false;
            if (this.CcRecipients != other.CcRecipients) return false;
            if (this.Subject != other.Subject) return false;
            if (this.Body != other.Body) return false;
            if (this.RetryCount != other.RetryCount) return false;
            if (this.Error != other.Error) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MailFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter Recipients { get; set; }
        public StringFilter BccRecipients { get; set; }
        public StringFilter CcRecipients { get; set; }
        public StringFilter Subject { get; set; }
        public StringFilter Body { get; set; }
        public LongFilter RetryCount { get; set; }
        public StringFilter Error { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<MailFilter> OrFilter { get; set; }
        public MailOrder OrderBy {get; set;}
        public MailSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MailOrder
    {
        Id = 0,
        Username = 1,
        Password = 2,
        Recipients = 3,
        BccRecipients = 4,
        CcRecipients = 5,
        Subject = 6,
        Body = 7,
        RetryCount = 8,
        Error = 9,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum MailSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Username = E._1,
        Password = E._2,
        Recipients = E._3,
        BccRecipients = E._4,
        CcRecipients = E._5,
        Subject = E._6,
        Body = E._7,
        RetryCount = E._8,
        Error = E._9,
    }
}
