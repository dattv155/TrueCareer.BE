using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class MailDAO
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
        public DateTime CreatedAt { get; set; }
        public Guid RowId { get; set; }
    }
}
