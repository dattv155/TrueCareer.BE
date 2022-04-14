using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer;
using TrueCareer.Common;
using TrueCareer.Enums;
using TrueCareer.Entities;
using TrueCareer.Repositories;

namespace TrueCareer.Services.MMail
{
    public interface IMailValidator : IServiceScoped
    {
        Task Get(Mail Mail);
        Task<bool> Create(Mail Mail);
        Task<bool> Update(Mail Mail);
        Task<bool> Delete(Mail Mail);
        Task<bool> BulkDelete(List<Mail> Mails);
        Task<bool> Import(List<Mail> Mails);
    }

    public class MailValidator : IMailValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private MailMessage MailMessage;

        public MailValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.MailMessage = new MailMessage();
        }

        public async Task Get(Mail Mail)
        {
        }

        public async Task<bool> Create(Mail Mail)
        {
            await ValidateUsername(Mail);
            await ValidatePassword(Mail);
            await ValidateRecipients(Mail);
            await ValidateBccRecipients(Mail);
            await ValidateCcRecipients(Mail);
            await ValidateSubject(Mail);
            await ValidateBody(Mail);
            await ValidateRetryCount(Mail);
            await ValidateError(Mail);
            return Mail.IsValidated;
        }

        public async Task<bool> Update(Mail Mail)
        {
            if (await ValidateId(Mail))
            {
                await ValidateUsername(Mail);
                await ValidatePassword(Mail);
                await ValidateRecipients(Mail);
                await ValidateBccRecipients(Mail);
                await ValidateCcRecipients(Mail);
                await ValidateSubject(Mail);
                await ValidateBody(Mail);
                await ValidateRetryCount(Mail);
                await ValidateError(Mail);
            }
            return Mail.IsValidated;
        }

        public async Task<bool> Delete(Mail Mail)
        {
            if (await ValidateId(Mail))
            {
            }
            return Mail.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Mail> Mails)
        {
            foreach (Mail Mail in Mails)
            {
                await Delete(Mail);
            }
            return Mails.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Mail> Mails)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Mail Mail)
        {
            MailFilter MailFilter = new MailFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Mail.Id },
                Selects = MailSelect.Id
            };

            int count = await UOW.MailRepository.CountAll(MailFilter);
            if (count == 0)
                Mail.AddError(nameof(MailValidator), nameof(Mail.Id), MailMessage.Error.IdNotExisted, MailMessage);
            return Mail.IsValidated;
        }

        private async Task<bool> ValidateUsername(Mail Mail)
        {
            if(string.IsNullOrEmpty(Mail.Username))
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Username), MailMessage.Error.UsernameEmpty, MailMessage);
            }
            else if(Mail.Username.Count() > 500)
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Username), MailMessage.Error.UsernameOverLength, MailMessage);
            }
            return Mail.IsValidated;
        }
        private async Task<bool> ValidatePassword(Mail Mail)
        {
            if(string.IsNullOrEmpty(Mail.Password))
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Password), MailMessage.Error.PasswordEmpty, MailMessage);
            }
            else if(Mail.Password.Count() > 500)
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Password), MailMessage.Error.PasswordOverLength, MailMessage);
            }
            return Mail.IsValidated;
        }
        private async Task<bool> ValidateRecipients(Mail Mail)
        {
            if(string.IsNullOrEmpty(Mail.Recipients))
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Recipients), MailMessage.Error.RecipientsEmpty, MailMessage);
            }
            else if(Mail.Recipients.Count() > 500)
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Recipients), MailMessage.Error.RecipientsOverLength, MailMessage);
            }
            return Mail.IsValidated;
        }
        private async Task<bool> ValidateBccRecipients(Mail Mail)
        {
            if(string.IsNullOrEmpty(Mail.BccRecipients))
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.BccRecipients), MailMessage.Error.BccRecipientsEmpty, MailMessage);
            }
            else if(Mail.BccRecipients.Count() > 500)
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.BccRecipients), MailMessage.Error.BccRecipientsOverLength, MailMessage);
            }
            return Mail.IsValidated;
        }
        private async Task<bool> ValidateCcRecipients(Mail Mail)
        {
            if(string.IsNullOrEmpty(Mail.CcRecipients))
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.CcRecipients), MailMessage.Error.CcRecipientsEmpty, MailMessage);
            }
            else if(Mail.CcRecipients.Count() > 500)
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.CcRecipients), MailMessage.Error.CcRecipientsOverLength, MailMessage);
            }
            return Mail.IsValidated;
        }
        private async Task<bool> ValidateSubject(Mail Mail)
        {
            if(string.IsNullOrEmpty(Mail.Subject))
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Subject), MailMessage.Error.SubjectEmpty, MailMessage);
            }
            else if(Mail.Subject.Count() > 500)
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Subject), MailMessage.Error.SubjectOverLength, MailMessage);
            }
            return Mail.IsValidated;
        }
        private async Task<bool> ValidateBody(Mail Mail)
        {
            if(string.IsNullOrEmpty(Mail.Body))
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Body), MailMessage.Error.BodyEmpty, MailMessage);
            }
            else if(Mail.Body.Count() > 500)
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Body), MailMessage.Error.BodyOverLength, MailMessage);
            }
            return Mail.IsValidated;
        }
        private async Task<bool> ValidateRetryCount(Mail Mail)
        {   
            return true;
        }
        private async Task<bool> ValidateError(Mail Mail)
        {
            if(string.IsNullOrEmpty(Mail.Error))
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Error), MailMessage.Error.ErrorEmpty, MailMessage);
            }
            else if(Mail.Error.Count() > 500)
            {
                Mail.AddError(nameof(MailValidator), nameof(Mail.Error), MailMessage.Error.ErrorOverLength, MailMessage);
            }
            return Mail.IsValidated;
        }
    }
}
