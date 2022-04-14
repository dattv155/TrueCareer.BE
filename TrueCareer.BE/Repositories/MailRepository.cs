using TrueSight.Common;
using TrueCareer.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueCareer.BE.Models;

namespace TrueCareer.Repositories
{
    public interface IMailRepository
    {
        Task<int> Count(MailFilter filter);
        Task<List<Mail>> List(MailFilter filter);
        Task<Mail> Get(long Id);
        Task<bool> Create(Mail mail);
        Task<bool> Update(Mail mail);
        Task<bool> Delete(long Id);
        Task<bool> BulkDelete(List<long> Ids);
        Task<bool> BulkMerge(List<Mail> Mails);
    }
    public class MailRepository : IMailRepository
    {
        private readonly DataContext DataContext;
        public MailRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<MailDAO> DynamicFilter(IQueryable<MailDAO> query, MailFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);

            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Username, filter.Username);
            query = query.Where(q => q.Password, filter.Password);
            query = query.Where(q => q.Body, filter.Body);
            query = query.Where(q => q.Recipients, filter.Recipients);
            query = query.Where(q => q.Subject, filter.Subject);
            query = query.Where(q => q.RetryCount, filter.RetryCount);
            return query;
        }

        public async Task<int> Count(MailFilter filter)
        {
            if (filter == null) return 0;
            IQueryable<MailDAO> mailDAOs = DataContext.Mail;
            mailDAOs = DynamicFilter(mailDAOs, filter);
            int count = await mailDAOs.CountAsync();
            return count;
        }

        public async Task<List<Mail>> List(MailFilter filter)
        {
            if (filter == null) return new List<Mail>();
            IQueryable<MailDAO> mailDAOs = DataContext.Mail;
            mailDAOs = DynamicFilter(mailDAOs, filter);

            var mails = await mailDAOs.ToListAsync();

            var mailIds = mails.Select(x => x.Id).ToList();

            List<Mail> Mails = new List<Mail>();
            foreach (var mail in mails)
            {
                Mail Mail = new Mail
                {
                    Id = mail.Id,
                    Username = mail.Username,
                    Password = mail.Password,
                    Body = mail.Body,
                    Recipients = new List<string>(),
                    CcRecipients = new List<string>(),
                    BccRecipients = new List<string>(),
                    Subject = mail.Subject,
                    RetryCount = mail.RetryCount,
                    Error = mail.Error,
                };
                Mail.Recipients = mail.Recipients != null ? JsonConvert.DeserializeObject<List<string>>(mail.Recipients) : null;
                Mail.CcRecipients = mail.CcRecipients != null ? JsonConvert.DeserializeObject<List<string>>(mail.CcRecipients) : null;
                Mail.BccRecipients = mail.BccRecipients != null ? JsonConvert.DeserializeObject<List<string>>(mail.BccRecipients) : null;
                Mails.Add(Mail);
            }
            return Mails;
        }
        public async Task<Mail> Get(long Id)
        {
            MailDAO mailDAO = await DataContext.Mail.Where(m => m.Id == Id).FirstOrDefaultAsync();
            Mail mail = new Mail
            {
                Id = mailDAO.Id,
                Body = mailDAO.Body,
                Subject = mailDAO.Subject,
                RetryCount = mailDAO.RetryCount,
                Error = mailDAO.Error,
            };
            mail.Recipients = mailDAO.Recipients != null ? JsonConvert.DeserializeObject<List<string>>(mailDAO.Recipients) : null;
            mail.CcRecipients = mailDAO.CcRecipients != null ? JsonConvert.DeserializeObject<List<string>>(mailDAO.CcRecipients) : null;
            mail.BccRecipients = mailDAO.BccRecipients != null ? JsonConvert.DeserializeObject<List<string>>(mailDAO.BccRecipients) : null;
            
            return mail;
        }
        public async Task<bool> Create(Mail mail)
        {
            if (mail.Recipients == null) mail.Recipients = new List<string>();
            if (mail.BccRecipients == null) mail.CcRecipients = new List<string>();
            if (mail.BccRecipients == null) mail.BccRecipients = new List<string>();
            MailDAO mailDAO = new MailDAO
            {
                Username = mail.Username,
                Password = mail.Password,
                Body = mail.Body,
                Subject = mail.Subject,
                RetryCount = 1,
                Recipients = JsonConvert.SerializeObject(mail.Recipients),
                CcRecipients = JsonConvert.SerializeObject(mail.CcRecipients),
                BccRecipients = JsonConvert.SerializeObject(mail.BccRecipients),
                CreatedAt = StaticParams.DateTimeNow,
            };
            DataContext.Mail.Add(mailDAO);
            await DataContext.SaveChangesAsync();
            
            await DataContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Update(Mail mail)
        {
            await DataContext.Mail.Where(m => m.Id == mail.Id).UpdateFromQueryAsync(n => new MailDAO
            {
                RetryCount = mail.RetryCount,
                Error = mail.Error
            });
            return true;
        }
        public async Task<bool> Delete(long Id)
        {
    
            await DataContext.Mail.Where(m => m.Id == Id).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkDelete(List<long> Ids)
        {
            await DataContext.Mail.WhereBulkContains(Ids, x => x.Id).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<Mail> Mails)
        {
            Mails.ForEach(x => x.RowId = Guid.NewGuid());
            List<MailDAO> MailDAOs = Mails.Select(x => new MailDAO
            {
                Username = x.Username,
                Password = x.Password,
                Subject = x.Subject,
                Body = x.Body,
                Recipients = JsonConvert.SerializeObject(x.Recipients),
                CcRecipients = JsonConvert.SerializeObject(x.CcRecipients),
                BccRecipients = JsonConvert.SerializeObject(x.BccRecipients),
                RetryCount = 0,
                CreatedAt = StaticParams.DateTimeNow,
                RowId = x.RowId
            }).ToList();
            DataContext.Mail.AddRange(MailDAOs);
            await DataContext.SaveChangesAsync();

            await DataContext.SaveChangesAsync();
            return true;
        }
    }
}
