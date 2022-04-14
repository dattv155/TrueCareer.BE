using TrueSight.Common;
using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueCareer.Entities
{
    public class Mail
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Recipients { get; set; }
        public List<string> BccRecipients { get; set; }
        public List<string> CcRecipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public long RetryCount { get; set; }
        public string Error { get; set; }
        public Guid RowId { get; set; }
        public Mail() { }
    }

    public class MailFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter Recipients { get; set; }
        public StringFilter Subject { get; set; }
        public StringFilter Body { get; set; }
        public LongFilter RetryCount { get; set; }

    }

    public enum MailOrder
    {
        Rece
    }
}
