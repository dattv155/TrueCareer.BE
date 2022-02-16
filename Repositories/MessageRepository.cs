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
    public interface IMessageRepository
    {
        Task<int> CountAll(MessageFilter MessageFilter);
        Task<int> Count(MessageFilter MessageFilter);
        Task<List<Message>> List(MessageFilter MessageFilter);
        Task<List<Message>> List(List<long> Ids);
        Task<Message> Get(long Id);
        Task<bool> Create(Message Message);
        Task<bool> Update(Message Message);
        Task<bool> Delete(Message Message);
        Task<bool> BulkMerge(List<Message> Messages);
        Task<bool> BulkDelete(List<Message> Messages);
    }
    public class MessageRepository : IMessageRepository
    {
        private DataContext DataContext;
        public MessageRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<MessageDAO>> DynamicFilter(IQueryable<MessageDAO> query, MessageFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Content, filter.Content);
            query = query.Where(q => q.ConversationId, filter.ConversationId);
            return query;
        }

        private async Task<IQueryable<MessageDAO>> OrFilter(IQueryable<MessageDAO> query, MessageFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MessageDAO> initQuery = query.Where(q => false);
            foreach (MessageFilter MessageFilter in filter.OrFilter)
            {
                IQueryable<MessageDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, MessageFilter.Id);
                queryable = queryable.Where(q => q.Content, MessageFilter.Content);
                queryable = queryable.Where(q => q.ConversationId, MessageFilter.ConversationId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<MessageDAO> DynamicOrder(IQueryable<MessageDAO> query, MessageFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MessageOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case MessageOrder.User:
                            query = query.OrderBy(q => q.UserId);
                            break;
                        case MessageOrder.Content:
                            query = query.OrderBy(q => q.Content);
                            break;
                        case MessageOrder.Conversation:
                            query = query.OrderBy(q => q.ConversationId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MessageOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case MessageOrder.User:
                            query = query.OrderByDescending(q => q.UserId);
                            break;
                        case MessageOrder.Content:
                            query = query.OrderByDescending(q => q.Content);
                            break;
                        case MessageOrder.Conversation:
                            query = query.OrderByDescending(q => q.ConversationId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Message>> DynamicSelect(IQueryable<MessageDAO> query, MessageFilter filter)
        {
            List<Message> Messages = await query.Select(q => new Message()
            {
                Id = filter.Selects.Contains(MessageSelect.Id) ? q.Id : default(long),
                UserId = filter.Selects.Contains(MessageSelect.User) ? q.UserId : default(long),
                Content = filter.Selects.Contains(MessageSelect.Content) ? q.Content : default(string),
                ConversationId = filter.Selects.Contains(MessageSelect.Conversation) ? q.ConversationId : default(long),
                Conversation = filter.Selects.Contains(MessageSelect.Conversation) && q.Conversation != null ? new Conversation
                {
                    Id = q.Conversation.Id,
                    LatestContent = q.Conversation.LatestContent,
                    LatestUserId = q.Conversation.LatestUserId,
                    Hash = q.Conversation.Hash,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return Messages;
        }

        public async Task<int> CountAll(MessageFilter filter)
        {
            IQueryable<MessageDAO> MessageDAOs = DataContext.Message.AsNoTracking();
            MessageDAOs = await DynamicFilter(MessageDAOs, filter);
            return await MessageDAOs.CountAsync();
        }

        public async Task<int> Count(MessageFilter filter)
        {
            IQueryable<MessageDAO> MessageDAOs = DataContext.Message.AsNoTracking();
            MessageDAOs = await DynamicFilter(MessageDAOs, filter);
            MessageDAOs = await OrFilter(MessageDAOs, filter);
            return await MessageDAOs.CountAsync();
        }

        public async Task<List<Message>> List(MessageFilter filter)
        {
            if (filter == null) return new List<Message>();
            IQueryable<MessageDAO> MessageDAOs = DataContext.Message.AsNoTracking();
            MessageDAOs = await DynamicFilter(MessageDAOs, filter);
            MessageDAOs = await OrFilter(MessageDAOs, filter);
            MessageDAOs = DynamicOrder(MessageDAOs, filter);
            List<Message> Messages = await DynamicSelect(MessageDAOs, filter);
            return Messages;
        }

        public async Task<List<Message>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MessageDAO> query = DataContext.Message.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Message> Messages = await query.AsNoTracking()
            .Select(x => new Message()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                UserId = x.UserId,
                Content = x.Content,
                ConversationId = x.ConversationId,
                Conversation = x.Conversation == null ? null : new Conversation
                {
                    Id = x.Conversation.Id,
                    LatestContent = x.Conversation.LatestContent,
                    LatestUserId = x.Conversation.LatestUserId,
                    Hash = x.Conversation.Hash,
                },
            }).ToListAsync();
            

            return Messages;
        }

        public async Task<Message> Get(long Id)
        {
            Message Message = await DataContext.Message.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Message()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                UserId = x.UserId,
                Content = x.Content,
                ConversationId = x.ConversationId,
                Conversation = x.Conversation == null ? null : new Conversation
                {
                    Id = x.Conversation.Id,
                    LatestContent = x.Conversation.LatestContent,
                    LatestUserId = x.Conversation.LatestUserId,
                    Hash = x.Conversation.Hash,
                },
            }).FirstOrDefaultAsync();

            if (Message == null)
                return null;

            return Message;
        }
        
        public async Task<bool> Create(Message Message)
        {
            MessageDAO MessageDAO = new MessageDAO();
            MessageDAO.Id = Message.Id;
            MessageDAO.UserId = Message.UserId;
            MessageDAO.Content = Message.Content;
            MessageDAO.ConversationId = Message.ConversationId;
            MessageDAO.CreatedAt = StaticParams.DateTimeNow;
            MessageDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Message.Add(MessageDAO);
            await DataContext.SaveChangesAsync();
            Message.Id = MessageDAO.Id;
            await SaveReference(Message);
            return true;
        }

        public async Task<bool> Update(Message Message)
        {
            MessageDAO MessageDAO = DataContext.Message
                .Where(x => x.Id == Message.Id)
                .FirstOrDefault();
            if (MessageDAO == null)
                return false;
            MessageDAO.Id = Message.Id;
            MessageDAO.UserId = Message.UserId;
            MessageDAO.Content = Message.Content;
            MessageDAO.ConversationId = Message.ConversationId;
            MessageDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Message);
            return true;
        }

        public async Task<bool> Delete(Message Message)
        {
            await DataContext.Message
                .Where(x => x.Id == Message.Id)
                .UpdateFromQueryAsync(x => new MessageDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Message> Messages)
        {
            IdFilter IdFilter = new IdFilter { In = Messages.Select(x => x.Id).ToList() };
            List<MessageDAO> MessageDAOs = new List<MessageDAO>();
            List<MessageDAO> DbMessageDAOs = await DataContext.Message
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Message Message in Messages)
            {
                MessageDAO MessageDAO = DbMessageDAOs
                        .Where(x => x.Id == Message.Id)
                        .FirstOrDefault();
                if (MessageDAO == null)
                {
                    MessageDAO = new MessageDAO();
                    MessageDAO.CreatedAt = StaticParams.DateTimeNow;
                }
                MessageDAO.UserId = Message.UserId;
                MessageDAO.Content = Message.Content;
                MessageDAO.ConversationId = Message.ConversationId;
                MessageDAO.UpdatedAt = StaticParams.DateTimeNow;
                MessageDAOs.Add(MessageDAO);
            }
            await DataContext.BulkMergeAsync(MessageDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Message> Messages)
        {
            List<long> Ids = Messages.Select(x => x.Id).ToList();
            await DataContext.Message
                .WhereBulkContains(Ids, x => x.Id)
                .UpdateFromQueryAsync(x => new MessageDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(Message Message)
        {
        }
        
    }
}
