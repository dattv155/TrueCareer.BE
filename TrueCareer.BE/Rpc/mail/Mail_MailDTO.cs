using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.mail
{
    public class Mail_MailDTO : DataDTO
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
        public Mail_MailDTO() {}
        public Mail_MailDTO(Mail Mail)
        {
            this.Id = Mail.Id;
            this.Username = Mail.Username;
            this.Password = Mail.Password;
            this.Recipients = Mail.Recipients;
            this.BccRecipients = Mail.BccRecipients;
            this.CcRecipients = Mail.CcRecipients;
            this.Subject = Mail.Subject;
            this.Body = Mail.Body;
            this.RetryCount = Mail.RetryCount;
            this.Error = Mail.Error;
            this.RowId = Mail.RowId;
            this.CreatedAt = Mail.CreatedAt;
            this.UpdatedAt = Mail.UpdatedAt;
            this.Informations = Mail.Informations;
            this.Warnings = Mail.Warnings;
            this.Errors = Mail.Errors;
        }
    }

    public class Mail_MailFilterDTO : FilterDTO
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
        public MailOrder OrderBy { get; set; }
    }
}
