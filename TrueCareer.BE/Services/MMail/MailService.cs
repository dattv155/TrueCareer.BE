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

namespace TrueCareer.Services.MMail
{
    public interface IMailService :  IServiceScoped
    {
        Task<int> Count(MailFilter MailFilter);
        Task<List<Mail>> List(MailFilter MailFilter);
        Task<Mail> Get(long Id);
        Task<Mail> Create(Mail Mail);
        Task<Mail> Update(Mail Mail);
        Task<Mail> Delete(Mail Mail);
        Task<List<Mail>> BulkDelete(List<Mail> Mails);
        Task<List<Mail>> Import(List<Mail> Mails);
        Task<MailFilter> ToFilter(MailFilter MailFilter);
    }

    public class MailService : BaseService, IMailService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IMailValidator MailValidator;

        public MailService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IMailValidator MailValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.MailValidator = MailValidator;
        }
        public async Task<int> Count(MailFilter MailFilter)
        {
            try
            {
                int result = await UOW.MailRepository.Count(MailFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MailService));
            }
            return 0;
        }

        public async Task<List<Mail>> List(MailFilter MailFilter)
        {
            try
            {
                List<Mail> Mails = await UOW.MailRepository.List(MailFilter);
                return Mails;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MailService));
            }
            return null;
        }

        public async Task<Mail> Get(long Id)
        {
            Mail Mail = await UOW.MailRepository.Get(Id);
            await MailValidator.Get(Mail);
            if (Mail == null)
                return null;
            return Mail;
        }
        
        public async Task<Mail> Create(Mail Mail)
        {
            if (!await MailValidator.Create(Mail))
                return Mail;

            try
            {
                await UOW.MailRepository.Create(Mail);
                Mail = await UOW.MailRepository.Get(Mail.Id);
                Logging.CreateAuditLog(Mail, new { }, nameof(MailService));
                return Mail;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MailService));
            }
            return null;
        }

        public async Task<Mail> Update(Mail Mail)
        {
            if (!await MailValidator.Update(Mail))
                return Mail;
            try
            {
                var oldData = await UOW.MailRepository.Get(Mail.Id);

                await UOW.MailRepository.Update(Mail);

                Mail = await UOW.MailRepository.Get(Mail.Id);
                Logging.CreateAuditLog(Mail, oldData, nameof(MailService));
                return Mail;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MailService));
            }
            return null;
        }

        public async Task<Mail> Delete(Mail Mail)
        {
            if (!await MailValidator.Delete(Mail))
                return Mail;

            try
            {
                await UOW.MailRepository.Delete(Mail);
                Logging.CreateAuditLog(new { }, Mail, nameof(MailService));
                return Mail;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MailService));
            }
            return null;
        }

        public async Task<List<Mail>> BulkDelete(List<Mail> Mails)
        {
            if (!await MailValidator.BulkDelete(Mails))
                return Mails;

            try
            {
                await UOW.MailRepository.BulkDelete(Mails);
                Logging.CreateAuditLog(new { }, Mails, nameof(MailService));
                return Mails;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MailService));
            }
            return null;

        }
        
        public async Task<List<Mail>> Import(List<Mail> Mails)
        {
            if (!await MailValidator.Import(Mails))
                return Mails;
            try
            {
                await UOW.MailRepository.BulkMerge(Mails);

                Logging.CreateAuditLog(Mails, new { }, nameof(MailService));
                return Mails;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MailService));
            }
            return null;
        }     
        
        public async Task<MailFilter> ToFilter(MailFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<MailFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                MailFilter subFilter = new MailFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Username))
                        subFilter.Username = FilterBuilder.Merge(subFilter.Username, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Password))
                        subFilter.Password = FilterBuilder.Merge(subFilter.Password, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Recipients))
                        subFilter.Recipients = FilterBuilder.Merge(subFilter.Recipients, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.BccRecipients))
                        subFilter.BccRecipients = FilterBuilder.Merge(subFilter.BccRecipients, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CcRecipients))
                        subFilter.CcRecipients = FilterBuilder.Merge(subFilter.CcRecipients, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Subject))
                        subFilter.Subject = FilterBuilder.Merge(subFilter.Subject, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Body))
                        subFilter.Body = FilterBuilder.Merge(subFilter.Body, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RetryCount))
                        subFilter.RetryCount = FilterBuilder.Merge(subFilter.RetryCount, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Error))
                        subFilter.Error = FilterBuilder.Merge(subFilter.Error, FilterPermissionDefinition.StringFilter);
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

        private void Sync(List<Mail> Mails)
        {
            
        }

    }
}
