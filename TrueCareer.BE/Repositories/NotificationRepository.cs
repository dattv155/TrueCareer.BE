using TrueSight.Common;
using TrueCareer.Common;
using TrueCareer.Helpers;
using TrueCareer.Entities;
using TrueCareer.BE.Models;
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
    public interface INotificationRepository
    {
        Task<int> CountAll(NotificationFilter NotificationFilter);
        Task<int> Count(NotificationFilter NotificationFilter);
        Task<List<Notification>> List(NotificationFilter NotificationFilter);
        Task<List<Notification>> List(List<long> Ids);
        Task<Notification> Get(long Id);
        Task<bool> Create(Notification Notification);
        Task<bool> Update(Notification Notification);
        Task<bool> Delete(Notification Notification);
        Task<bool> BulkMerge(List<Notification> Notifications);
        Task<bool> BulkDelete(List<Notification> Notifications);
    }
    public class NotificationRepository : INotificationRepository
    {
        private DataContext DataContext;
        public NotificationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<NotificationDAO>> DynamicFilter(IQueryable<NotificationDAO> query, NotificationFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.TitleWeb, filter.TitleWeb);
            query = query.Where(q => q.ContentWeb, filter.ContentWeb);
            query = query.Where(q => q.Unread, filter.Unread);
            query = query.Where(q => q.Time, filter.Time);
            query = query.Where(q => q.LinkWebsite, filter.LinkWebsite);
            query = query.Where(q => q.RecipientId, filter.RecipientId);
            query = query.Where(q => q.SenderId, filter.SenderId);
            return query;
        }

        private async Task<IQueryable<NotificationDAO>> OrFilter(IQueryable<NotificationDAO> query, NotificationFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<NotificationDAO> initQuery = query.Where(q => false);
            foreach (NotificationFilter NotificationFilter in filter.OrFilter)
            {
                IQueryable<NotificationDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, NotificationFilter.Id);
                queryable = queryable.Where(q => q.TitleWeb, NotificationFilter.TitleWeb);
                queryable = queryable.Where(q => q.ContentWeb, NotificationFilter.ContentWeb);
                queryable = queryable.Where(q => q.Unread, NotificationFilter.Unread);
                queryable = queryable.Where(q => q.Time, NotificationFilter.Time);
                queryable = queryable.Where(q => q.LinkWebsite, NotificationFilter.LinkWebsite);
                queryable = queryable.Where(q => q.RecipientId, NotificationFilter.RecipientId);
                queryable = queryable.Where(q => q.SenderId, NotificationFilter.SenderId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<NotificationDAO> DynamicOrder(IQueryable<NotificationDAO> query, NotificationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case NotificationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case NotificationOrder.TitleWeb:
                            query = query.OrderBy(q => q.TitleWeb);
                            break;
                        case NotificationOrder.ContentWeb:
                            query = query.OrderBy(q => q.ContentWeb);
                            break;
                        case NotificationOrder.Sender:
                            query = query.OrderBy(q => q.SenderId);
                            break;
                        case NotificationOrder.Recipient:
                            query = query.OrderBy(q => q.RecipientId);
                            break;
                        case NotificationOrder.Unread:
                            query = query.OrderBy(q => q.Unread);
                            break;
                        case NotificationOrder.Time:
                            query = query.OrderBy(q => q.Time);
                            break;
                        case NotificationOrder.LinkWebsite:
                            query = query.OrderBy(q => q.LinkWebsite);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case NotificationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case NotificationOrder.TitleWeb:
                            query = query.OrderByDescending(q => q.TitleWeb);
                            break;
                        case NotificationOrder.ContentWeb:
                            query = query.OrderByDescending(q => q.ContentWeb);
                            break;
                        case NotificationOrder.Sender:
                            query = query.OrderByDescending(q => q.SenderId);
                            break;
                        case NotificationOrder.Recipient:
                            query = query.OrderByDescending(q => q.RecipientId);
                            break;
                        case NotificationOrder.Unread:
                            query = query.OrderByDescending(q => q.Unread);
                            break;
                        case NotificationOrder.Time:
                            query = query.OrderByDescending(q => q.Time);
                            break;
                        case NotificationOrder.LinkWebsite:
                            query = query.OrderByDescending(q => q.LinkWebsite);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Notification>> DynamicSelect(IQueryable<NotificationDAO> query, NotificationFilter filter)
        {
            List<Notification> Notifications = await query.Select(q => new Notification()
            {
                Id = filter.Selects.Contains(NotificationSelect.Id) ? q.Id : default(long),
                TitleWeb = filter.Selects.Contains(NotificationSelect.TitleWeb) ? q.TitleWeb : default(string),
                ContentWeb = filter.Selects.Contains(NotificationSelect.ContentWeb) ? q.ContentWeb : default(string),
                SenderId = filter.Selects.Contains(NotificationSelect.Sender) ? q.SenderId : default(long),
                RecipientId = filter.Selects.Contains(NotificationSelect.Recipient) ? q.RecipientId : default(long),
                Unread = filter.Selects.Contains(NotificationSelect.Unread) ? q.Unread : default(bool),
                Time = filter.Selects.Contains(NotificationSelect.Time) ? q.Time : default(DateTime),
                LinkWebsite = filter.Selects.Contains(NotificationSelect.LinkWebsite) ? q.LinkWebsite : default(string),
                Recipient = filter.Selects.Contains(NotificationSelect.Recipient) && q.Recipient != null ? new AppUser
                {
                    Id = q.Recipient.Id,
                    Username = q.Recipient.Username,
                    Email = q.Recipient.Email,
                    Phone = q.Recipient.Phone,
                    Password = q.Recipient.Password,
                    DisplayName = q.Recipient.DisplayName,
                    SexId = q.Recipient.SexId,
                    Birthday = q.Recipient.Birthday,
                    Avatar = q.Recipient.Avatar,
                    CoverImage = q.Recipient.CoverImage,
                } : null,
                Sender = filter.Selects.Contains(NotificationSelect.Sender) && q.Sender != null ? new AppUser
                {
                    Id = q.Sender.Id,
                    Username = q.Sender.Username,
                    Email = q.Sender.Email,
                    Phone = q.Sender.Phone,
                    Password = q.Sender.Password,
                    DisplayName = q.Sender.DisplayName,
                    SexId = q.Sender.SexId,
                    Birthday = q.Sender.Birthday,
                    Avatar = q.Sender.Avatar,
                    CoverImage = q.Sender.CoverImage,
                } : null,
                RowId = q.RowId,
            }).ToListAsync();
            return Notifications;
        }

        public async Task<int> CountAll(NotificationFilter filter)
        {
            IQueryable<NotificationDAO> NotificationDAOs = DataContext.Notification.AsNoTracking();
            NotificationDAOs = await DynamicFilter(NotificationDAOs, filter);
            return await NotificationDAOs.CountAsync();
        }

        public async Task<int> Count(NotificationFilter filter)
        {
            IQueryable<NotificationDAO> NotificationDAOs = DataContext.Notification.AsNoTracking();
            NotificationDAOs = await DynamicFilter(NotificationDAOs, filter);
            NotificationDAOs = await OrFilter(NotificationDAOs, filter);
            return await NotificationDAOs.CountAsync();
        }

        public async Task<List<Notification>> List(NotificationFilter filter)
        {
            if (filter == null) return new List<Notification>();
            IQueryable<NotificationDAO> NotificationDAOs = DataContext.Notification.AsNoTracking();
            NotificationDAOs = await DynamicFilter(NotificationDAOs, filter);
            NotificationDAOs = await OrFilter(NotificationDAOs, filter);
            NotificationDAOs = DynamicOrder(NotificationDAOs, filter);
            List<Notification> Notifications = await DynamicSelect(NotificationDAOs, filter);
            return Notifications;
        }

        public async Task<List<Notification>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<NotificationDAO> query = DataContext.Notification.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Notification> Notifications = await query.AsNoTracking()
            .Select(x => new Notification()
            {
                RowId = x.RowId,
                Id = x.Id,
                TitleWeb = x.TitleWeb,
                ContentWeb = x.ContentWeb,
                SenderId = x.SenderId,
                RecipientId = x.RecipientId,
                Unread = x.Unread,
                Time = x.Time,
                LinkWebsite = x.LinkWebsite,
                Recipient = x.Recipient == null ? null : new AppUser
                {
                    Id = x.Recipient.Id,
                    Username = x.Recipient.Username,
                    Email = x.Recipient.Email,
                    Phone = x.Recipient.Phone,
                    Password = x.Recipient.Password,
                    DisplayName = x.Recipient.DisplayName,
                    SexId = x.Recipient.SexId,
                    Birthday = x.Recipient.Birthday,
                    Avatar = x.Recipient.Avatar,
                    CoverImage = x.Recipient.CoverImage,
                },
                Sender = x.Sender == null ? null : new AppUser
                {
                    Id = x.Sender.Id,
                    Username = x.Sender.Username,
                    Email = x.Sender.Email,
                    Phone = x.Sender.Phone,
                    Password = x.Sender.Password,
                    DisplayName = x.Sender.DisplayName,
                    SexId = x.Sender.SexId,
                    Birthday = x.Sender.Birthday,
                    Avatar = x.Sender.Avatar,
                    CoverImage = x.Sender.CoverImage,
                },
            }).ToListAsync();
            

            return Notifications;
        }

        public async Task<Notification> Get(long Id)
        {
            Notification Notification = await DataContext.Notification.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new Notification()
            {
                Id = x.Id,
                TitleWeb = x.TitleWeb,
                ContentWeb = x.ContentWeb,
                SenderId = x.SenderId,
                RecipientId = x.RecipientId,
                Unread = x.Unread,
                Time = x.Time,
                LinkWebsite = x.LinkWebsite,
                Recipient = x.Recipient == null ? null : new AppUser
                {
                    Id = x.Recipient.Id,
                    Username = x.Recipient.Username,
                    Email = x.Recipient.Email,
                    Phone = x.Recipient.Phone,
                    Password = x.Recipient.Password,
                    DisplayName = x.Recipient.DisplayName,
                    SexId = x.Recipient.SexId,
                    Birthday = x.Recipient.Birthday,
                    Avatar = x.Recipient.Avatar,
                    CoverImage = x.Recipient.CoverImage,
                },
                Sender = x.Sender == null ? null : new AppUser
                {
                    Id = x.Sender.Id,
                    Username = x.Sender.Username,
                    Email = x.Sender.Email,
                    Phone = x.Sender.Phone,
                    Password = x.Sender.Password,
                    DisplayName = x.Sender.DisplayName,
                    SexId = x.Sender.SexId,
                    Birthday = x.Sender.Birthday,
                    Avatar = x.Sender.Avatar,
                    CoverImage = x.Sender.CoverImage,
                },
            }).FirstOrDefaultAsync();

            if (Notification == null)
                return null;

            return Notification;
        }
        
        public async Task<bool> Create(Notification Notification)
        {
            NotificationDAO NotificationDAO = new NotificationDAO();
            NotificationDAO.Id = Notification.Id;
            NotificationDAO.TitleWeb = Notification.TitleWeb;
            NotificationDAO.ContentWeb = Notification.ContentWeb;
            NotificationDAO.SenderId = Notification.SenderId;
            NotificationDAO.RecipientId = Notification.RecipientId;
            NotificationDAO.Unread = Notification.Unread;
            NotificationDAO.Time = Notification.Time;
            NotificationDAO.LinkWebsite = Notification.LinkWebsite;
            NotificationDAO.RowId = Guid.NewGuid();
            DataContext.Notification.Add(NotificationDAO);
            await DataContext.SaveChangesAsync();
            Notification.Id = NotificationDAO.Id;
            await SaveReference(Notification);
            return true;
        }

        public async Task<bool> Update(Notification Notification)
        {
            NotificationDAO NotificationDAO = DataContext.Notification
                .Where(x => x.Id == Notification.Id)
                .FirstOrDefault();
            if (NotificationDAO == null)
                return false;
            NotificationDAO.Id = Notification.Id;
            NotificationDAO.TitleWeb = Notification.TitleWeb;
            NotificationDAO.ContentWeb = Notification.ContentWeb;
            NotificationDAO.SenderId = Notification.SenderId;
            NotificationDAO.RecipientId = Notification.RecipientId;
            NotificationDAO.Unread = Notification.Unread;
            NotificationDAO.Time = Notification.Time;
            NotificationDAO.LinkWebsite = Notification.LinkWebsite;
            await DataContext.SaveChangesAsync();
            await SaveReference(Notification);
            return true;
        }

        public async Task<bool> Delete(Notification Notification)
        {
            await DataContext.Notification
                .Where(x => x.Id == Notification.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Notification> Notifications)
        {
            IdFilter IdFilter = new IdFilter { In = Notifications.Select(x => x.Id).ToList() };
            List<NotificationDAO> NotificationDAOs = new List<NotificationDAO>();
            List<NotificationDAO> DbNotificationDAOs = await DataContext.Notification
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Notification Notification in Notifications)
            {
                NotificationDAO NotificationDAO = DbNotificationDAOs
                        .Where(x => x.Id == Notification.Id)
                        .FirstOrDefault();
                if (NotificationDAO == null)
                {
                    NotificationDAO = new NotificationDAO();
                    NotificationDAO.RowId = Guid.NewGuid();
                    Notification.RowId = NotificationDAO.RowId;
                }
                NotificationDAO.TitleWeb = Notification.TitleWeb;
                NotificationDAO.ContentWeb = Notification.ContentWeb;
                NotificationDAO.SenderId = Notification.SenderId;
                NotificationDAO.RecipientId = Notification.RecipientId;
                NotificationDAO.Unread = Notification.Unread;
                NotificationDAO.Time = Notification.Time;
                NotificationDAO.LinkWebsite = Notification.LinkWebsite;
                NotificationDAOs.Add(NotificationDAO);
            }
            await DataContext.BulkMergeAsync(NotificationDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Notification> Notifications)
        {
            List<long> Ids = Notifications.Select(x => x.Id).ToList();
            await DataContext.Notification
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Notification Notification)
        {
        }
        
    }
}
