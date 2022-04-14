using TrueSight.Common;
using TrueCareer.Common;
using TrueCareer.Helpers;
using TrueCareer.Entities;
using TrueCareer.Models;
using TrueCareer.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace TrueCareer.Repositories
{
    public interface IMailRepository
    {
        Task<int> CountAll(MailFilter MailFilter);
        Task<int> Count(MailFilter MailFilter);
        Task<List<Mail>> List(MailFilter MailFilter);
        Task<List<Mail>> List(List<long> Ids);
        Task<Mail> Get(long Id);
        Task<bool> Create(Mail Mail);
        Task<bool> Update(Mail Mail);
        Task<bool> Delete(Mail Mail);
        Task<bool> BulkMerge(List<Mail> Mails);
        Task<bool> BulkDelete(List<Mail> Mails);
    }
    public class MailRepository : IMailRepository
    {
        private DataContext DataContext;
        public MailRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<MailDAO>> DynamicFilter(IQueryable<MailDAO> query, MailFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Username, filter.Username);
            query = query.Where(q => q.Password, filter.Password);
            query = query.Where(q => q.Recipients, filter.Recipients);
            query = query.Where(q => q.BccRecipients, filter.BccRecipients);
            query = query.Where(q => q.CcRecipients, filter.CcRecipients);
            query = query.Where(q => q.Subject, filter.Subject);
            query = query.Where(q => q.Body, filter.Body);
            query = query.Where(q => q.RetryCount, filter.RetryCount);
            query = query.Where(q => q.Error, filter.Error);
            return query;
        }

        private async Task<IQueryable<MailDAO>> OrFilter(IQueryable<MailDAO> query, MailFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MailDAO> initQuery = query.Where(q => false);
            foreach (MailFilter MailFilter in filter.OrFilter)
            {
                IQueryable<MailDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, MailFilter.Id);
                queryable = queryable.Where(q => q.Username, MailFilter.Username);
                queryable = queryable.Where(q => q.Password, MailFilter.Password);
                queryable = queryable.Where(q => q.Recipients, MailFilter.Recipients);
                queryable = queryable.Where(q => q.BccRecipients, MailFilter.BccRecipients);
                queryable = queryable.Where(q => q.CcRecipients, MailFilter.CcRecipients);
                queryable = queryable.Where(q => q.Subject, MailFilter.Subject);
                queryable = queryable.Where(q => q.Body, MailFilter.Body);
                queryable = queryable.Where(q => q.RetryCount, MailFilter.RetryCount);
                queryable = queryable.Where(q => q.Error, MailFilter.Error);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<MailDAO> DynamicOrder(IQueryable<MailDAO> query, MailFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MailOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case MailOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case MailOrder.Password:
                            query = query.OrderBy(q => q.Password);
                            break;
                        case MailOrder.Recipients:
                            query = query.OrderBy(q => q.Recipients);
                            break;
                        case MailOrder.BccRecipients:
                            query = query.OrderBy(q => q.BccRecipients);
                            break;
                        case MailOrder.CcRecipients:
                            query = query.OrderBy(q => q.CcRecipients);
                            break;
                        case MailOrder.Subject:
                            query = query.OrderBy(q => q.Subject);
                            break;
                        case MailOrder.Body:
                            query = query.OrderBy(q => q.Body);
                            break;
                        case MailOrder.RetryCount:
                            query = query.OrderBy(q => q.RetryCount);
                            break;
                        case MailOrder.Error:
                            query = query.OrderBy(q => q.Error);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MailOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case MailOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case MailOrder.Password:
                            query = query.OrderByDescending(q => q.Password);
                            break;
                        case MailOrder.Recipients:
                            query = query.OrderByDescending(q => q.Recipients);
                            break;
                        case MailOrder.BccRecipients:
                            query = query.OrderByDescending(q => q.BccRecipients);
                            break;
                        case MailOrder.CcRecipients:
                            query = query.OrderByDescending(q => q.CcRecipients);
                            break;
                        case MailOrder.Subject:
                            query = query.OrderByDescending(q => q.Subject);
                            break;
                        case MailOrder.Body:
                            query = query.OrderByDescending(q => q.Body);
                            break;
                        case MailOrder.RetryCount:
                            query = query.OrderByDescending(q => q.RetryCount);
                            break;
                        case MailOrder.Error:
                            query = query.OrderByDescending(q => q.Error);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Mail>> DynamicSelect(IQueryable<MailDAO> query, MailFilter filter)
        {
            List<Mail> Mails = await query.Select(q => new Mail()
            {
                Id = filter.Selects.Contains(MailSelect.Id) ? q.Id : default(long),
                Username = filter.Selects.Contains(MailSelect.Username) ? q.Username : default(string),
                Password = filter.Selects.Contains(MailSelect.Password) ? q.Password : default(string),
                Recipients = filter.Selects.Contains(MailSelect.Recipients) ? q.Recipients : default(string),
                BccRecipients = filter.Selects.Contains(MailSelect.BccRecipients) ? q.BccRecipients : default(string),
                CcRecipients = filter.Selects.Contains(MailSelect.CcRecipients) ? q.CcRecipients : default(string),
                Subject = filter.Selects.Contains(MailSelect.Subject) ? q.Subject : default(string),
                Body = filter.Selects.Contains(MailSelect.Body) ? q.Body : default(string),
                RetryCount = filter.Selects.Contains(MailSelect.RetryCount) ? q.RetryCount : default(long),
                Error = filter.Selects.Contains(MailSelect.Error) ? q.Error : default(string),
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return Mails;
        }

        public async Task<int> CountAll(MailFilter filter)
        {
            IQueryable<MailDAO> MailDAOs = DataContext.Mail.AsNoTracking();
            MailDAOs = await DynamicFilter(MailDAOs, filter);
            return await MailDAOs.CountAsync();
        }

        public async Task<int> Count(MailFilter filter)
        {
            IQueryable<MailDAO> MailDAOs = DataContext.Mail.AsNoTracking();
            MailDAOs = await DynamicFilter(MailDAOs, filter);
            MailDAOs = await OrFilter(MailDAOs, filter);
            return await MailDAOs.CountAsync();
        }

        public async Task<List<Mail>> List(MailFilter filter)
        {
            if (filter == null) return new List<Mail>();
            IQueryable<MailDAO> MailDAOs = DataContext.Mail.AsNoTracking();
            MailDAOs = await DynamicFilter(MailDAOs, filter);
            MailDAOs = await OrFilter(MailDAOs, filter);
            MailDAOs = DynamicOrder(MailDAOs, filter);
            List<Mail> Mails = await DynamicSelect(MailDAOs, filter);
            return Mails;
        }

        public async Task<List<Mail>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MailDAO> query = DataContext.Mail.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Mail> Mails = await query.AsNoTracking()
            .Select(x => new Mail()
            {
                RowId = x.RowId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Username = x.Username,
                Password = x.Password,
                Recipients = x.Recipients,
                BccRecipients = x.BccRecipients,
                CcRecipients = x.CcRecipients,
                Subject = x.Subject,
                Body = x.Body,
                RetryCount = x.RetryCount,
                Error = x.Error,
            }).ToListAsync();
            

            return Mails;
        }

        public async Task<Mail> Get(long Id)
        {
            Mail Mail = await DataContext.Mail.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Mail()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Username = x.Username,
                Password = x.Password,
                Recipients = x.Recipients,
                BccRecipients = x.BccRecipients,
                CcRecipients = x.CcRecipients,
                Subject = x.Subject,
                Body = x.Body,
                RetryCount = x.RetryCount,
                Error = x.Error,
            }).FirstOrDefaultAsync();

            if (Mail == null)
                return null;

            return Mail;
        }
        
        public async Task<bool> Create(Mail Mail)
        {
            MailDAO MailDAO = new MailDAO();
            MailDAO.Id = Mail.Id;
            MailDAO.Username = Mail.Username;
            MailDAO.Password = Mail.Password;
            MailDAO.Recipients = Mail.Recipients;
            MailDAO.BccRecipients = Mail.BccRecipients;
            MailDAO.CcRecipients = Mail.CcRecipients;
            MailDAO.Subject = Mail.Subject;
            MailDAO.Body = Mail.Body;
            MailDAO.RetryCount = Mail.RetryCount;
            MailDAO.Error = Mail.Error;
            MailDAO.RowId = Guid.NewGuid();
            MailDAO.CreatedAt = StaticParams.DateTimeNow;
            MailDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Mail.Add(MailDAO);
            await DataContext.SaveChangesAsync();
            Mail.Id = MailDAO.Id;
            await SaveReference(Mail);
            return true;
        }

        public async Task<bool> Update(Mail Mail)
        {
            MailDAO MailDAO = DataContext.Mail
                .Where(x => x.Id == Mail.Id)
                .FirstOrDefault();
            if (MailDAO == null)
                return false;
            MailDAO.Id = Mail.Id;
            MailDAO.Username = Mail.Username;
            MailDAO.Password = Mail.Password;
            MailDAO.Recipients = Mail.Recipients;
            MailDAO.BccRecipients = Mail.BccRecipients;
            MailDAO.CcRecipients = Mail.CcRecipients;
            MailDAO.Subject = Mail.Subject;
            MailDAO.Body = Mail.Body;
            MailDAO.RetryCount = Mail.RetryCount;
            MailDAO.Error = Mail.Error;
            MailDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Mail);
            return true;
        }

        public async Task<bool> Delete(Mail Mail)
        {
            await DataContext.Mail
                .Where(x => x.Id == Mail.Id)
                .UpdateFromQueryAsync(x => new MailDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Mail> Mails)
        {
            IdFilter IdFilter = new IdFilter { In = Mails.Select(x => x.Id).ToList() };
            List<MailDAO> MailDAOs = new List<MailDAO>();
            List<MailDAO> DbMailDAOs = await DataContext.Mail
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Mail Mail in Mails)
            {
                MailDAO MailDAO = DbMailDAOs
                        .Where(x => x.Id == Mail.Id)
                        .FirstOrDefault();
                if (MailDAO == null)
                {
                    MailDAO = new MailDAO();
                    MailDAO.CreatedAt = StaticParams.DateTimeNow;
                    MailDAO.RowId = Guid.NewGuid();
                    Mail.RowId = MailDAO.RowId;
                }
                MailDAO.Username = Mail.Username;
                MailDAO.Password = Mail.Password;
                MailDAO.Recipients = Mail.Recipients;
                MailDAO.BccRecipients = Mail.BccRecipients;
                MailDAO.CcRecipients = Mail.CcRecipients;
                MailDAO.Subject = Mail.Subject;
                MailDAO.Body = Mail.Body;
                MailDAO.RetryCount = Mail.RetryCount;
                MailDAO.Error = Mail.Error;
                MailDAO.UpdatedAt = StaticParams.DateTimeNow;
                MailDAOs.Add(MailDAO);
            }
            await DataContext.BulkMergeAsync(MailDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Mail> Mails)
        {
            List<long> Ids = Mails.Select(x => x.Id).ToList();
            await DataContext.Mail
                .WhereBulkContains(Ids, x => x.Id)
                .UpdateFromQueryAsync(x => new MailDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(Mail Mail)
        {
        }
        
    }
}
