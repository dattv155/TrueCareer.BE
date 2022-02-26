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
    public interface IConversationRepository
    {
        Task<int> CountAll(ConversationFilter ConversationFilter);
        Task<int> Count(ConversationFilter ConversationFilter);
        Task<List<Conversation>> List(ConversationFilter ConversationFilter);
        Task<List<Conversation>> List(List<long> Ids);
        Task<Conversation> Get(long Id);
        Task<bool> Create(Conversation Conversation);
        Task<bool> Update(Conversation Conversation);
        Task<bool> Delete(Conversation Conversation);
        Task<bool> BulkMerge(List<Conversation> Conversations);
        Task<bool> BulkDelete(List<Conversation> Conversations);
    }
    public class ConversationRepository : IConversationRepository
    {
        private DataContext DataContext;
        public ConversationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ConversationDAO>> DynamicFilter(IQueryable<ConversationDAO> query, ConversationFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.LatestContent, filter.LatestContent);
            query = query.Where(q => q.Hash, filter.Hash);
            return query;
        }

        private async Task<IQueryable<ConversationDAO>> OrFilter(IQueryable<ConversationDAO> query, ConversationFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ConversationDAO> initQuery = query.Where(q => false);
            foreach (ConversationFilter ConversationFilter in filter.OrFilter)
            {
                IQueryable<ConversationDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ConversationFilter.Id);
                queryable = queryable.Where(q => q.LatestContent, ConversationFilter.LatestContent);
                queryable = queryable.Where(q => q.Hash, ConversationFilter.Hash);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ConversationDAO> DynamicOrder(IQueryable<ConversationDAO> query, ConversationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ConversationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ConversationOrder.LatestContent:
                            query = query.OrderBy(q => q.LatestContent);
                            break;
                        case ConversationOrder.LatestUser:
                            query = query.OrderBy(q => q.LatestUserId);
                            break;
                        case ConversationOrder.Hash:
                            query = query.OrderBy(q => q.Hash);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ConversationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ConversationOrder.LatestContent:
                            query = query.OrderByDescending(q => q.LatestContent);
                            break;
                        case ConversationOrder.LatestUser:
                            query = query.OrderByDescending(q => q.LatestUserId);
                            break;
                        case ConversationOrder.Hash:
                            query = query.OrderByDescending(q => q.Hash);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Conversation>> DynamicSelect(IQueryable<ConversationDAO> query, ConversationFilter filter)
        {
            List<Conversation> Conversations = await query.Select(q => new Conversation()
            {
                Id = filter.Selects.Contains(ConversationSelect.Id) ? q.Id : default(long),
                LatestContent = filter.Selects.Contains(ConversationSelect.LatestContent) ? q.LatestContent : default(string),
                LatestUserId = filter.Selects.Contains(ConversationSelect.LatestUser) ? q.LatestUserId : default(long?),
                Hash = filter.Selects.Contains(ConversationSelect.Hash) ? q.Hash : default(string),
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return Conversations;
        }

        public async Task<int> CountAll(ConversationFilter filter)
        {
            IQueryable<ConversationDAO> ConversationDAOs = DataContext.Conversation.AsNoTracking();
            ConversationDAOs = await DynamicFilter(ConversationDAOs, filter);
            return await ConversationDAOs.CountAsync();
        }

        public async Task<int> Count(ConversationFilter filter)
        {
            IQueryable<ConversationDAO> ConversationDAOs = DataContext.Conversation.AsNoTracking();
            ConversationDAOs = await DynamicFilter(ConversationDAOs, filter);
            ConversationDAOs = await OrFilter(ConversationDAOs, filter);
            return await ConversationDAOs.CountAsync();
        }

        public async Task<List<Conversation>> List(ConversationFilter filter)
        {
            if (filter == null) return new List<Conversation>();
            IQueryable<ConversationDAO> ConversationDAOs = DataContext.Conversation.AsNoTracking();
            ConversationDAOs = await DynamicFilter(ConversationDAOs, filter);
            ConversationDAOs = await OrFilter(ConversationDAOs, filter);
            ConversationDAOs = DynamicOrder(ConversationDAOs, filter);
            List<Conversation> Conversations = await DynamicSelect(ConversationDAOs, filter);
            return Conversations;
        }

        public async Task<List<Conversation>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ConversationDAO> query = DataContext.Conversation.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Conversation> Conversations = await query.AsNoTracking()
            .Select(x => new Conversation()
            {
                RowId = x.RowId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                LatestContent = x.LatestContent,
                LatestUserId = x.LatestUserId,
                Hash = x.Hash,
            }).ToListAsync();
            
            var MessageQuery = DataContext.Message.AsNoTracking()
                .Where(x => x.ConversationId, IdFilter);
            List<Message> Messages = await MessageQuery
                .Where(x => x.DeletedAt == null)
                .Select(x => new Message
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Content = x.Content,
                    ConversationId = x.ConversationId,
                }).ToListAsync();

            foreach(Conversation Conversation in Conversations)
            {
                Conversation.Messages = Messages
                    .Where(x => x.ConversationId == Conversation.Id)
                    .ToList();
            }


            return Conversations;
        }

        public async Task<Conversation> Get(long Id)
        {
            Conversation Conversation = await DataContext.Conversation.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Conversation()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                LatestContent = x.LatestContent,
                LatestUserId = x.LatestUserId,
                Hash = x.Hash,
            }).FirstOrDefaultAsync();

            if (Conversation == null)
                return null;
            Conversation.Messages = await DataContext.Message.AsNoTracking()
                .Where(x => x.ConversationId == Conversation.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new Message
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Content = x.Content,
                    ConversationId = x.ConversationId,
                }).ToListAsync();

            return Conversation;
        }
        
        public async Task<bool> Create(Conversation Conversation)
        {
            ConversationDAO ConversationDAO = new ConversationDAO();
            ConversationDAO.Id = Conversation.Id;
            ConversationDAO.LatestContent = Conversation.LatestContent;
            ConversationDAO.LatestUserId = Conversation.LatestUserId;
            ConversationDAO.Hash = Conversation.Hash;
            ConversationDAO.RowId = Guid.NewGuid();
            ConversationDAO.CreatedAt = StaticParams.DateTimeNow;
            ConversationDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Conversation.Add(ConversationDAO);
            await DataContext.SaveChangesAsync();
            Conversation.Id = ConversationDAO.Id;
            await SaveReference(Conversation);
            return true;
        }

        public async Task<bool> Update(Conversation Conversation)
        {
            ConversationDAO ConversationDAO = DataContext.Conversation
                .Where(x => x.Id == Conversation.Id)
                .FirstOrDefault();
            if (ConversationDAO == null)
                return false;
            ConversationDAO.Id = Conversation.Id;
            ConversationDAO.LatestContent = Conversation.LatestContent;
            ConversationDAO.LatestUserId = Conversation.LatestUserId;
            ConversationDAO.Hash = Conversation.Hash;
            ConversationDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Conversation);
            return true;
        }

        public async Task<bool> Delete(Conversation Conversation)
        {
            await DataContext.Conversation
                .Where(x => x.Id == Conversation.Id)
                .UpdateFromQueryAsync(x => new ConversationDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Conversation> Conversations)
        {
            IdFilter IdFilter = new IdFilter { In = Conversations.Select(x => x.Id).ToList() };
            List<ConversationDAO> ConversationDAOs = new List<ConversationDAO>();
            List<ConversationDAO> DbConversationDAOs = await DataContext.Conversation
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Conversation Conversation in Conversations)
            {
                ConversationDAO ConversationDAO = DbConversationDAOs
                        .Where(x => x.Id == Conversation.Id)
                        .FirstOrDefault();
                if (ConversationDAO == null)
                {
                    ConversationDAO = new ConversationDAO();
                    ConversationDAO.CreatedAt = StaticParams.DateTimeNow;
                    ConversationDAO.RowId = Guid.NewGuid();
                    Conversation.RowId = ConversationDAO.RowId;
                }
                ConversationDAO.LatestContent = Conversation.LatestContent;
                ConversationDAO.LatestUserId = Conversation.LatestUserId;
                ConversationDAO.Hash = Conversation.Hash;
                ConversationDAO.UpdatedAt = StaticParams.DateTimeNow;
                ConversationDAOs.Add(ConversationDAO);
            }
            await DataContext.BulkMergeAsync(ConversationDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Conversation> Conversations)
        {
            List<long> Ids = Conversations.Select(x => x.Id).ToList();
            await DataContext.Conversation
                .WhereBulkContains(Ids, x => x.Id)
                .UpdateFromQueryAsync(x => new ConversationDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(Conversation Conversation)
        {
            List<MessageDAO> MessageDAOs = new List<MessageDAO>();
            List<MessageDAO> DbMessageDAOs = await DataContext.Message
                .Where(x => x.ConversationId == Conversation.Id)
                .ToListAsync();
            DbMessageDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (Conversation.Messages != null)
            {
                foreach (Message Message in Conversation.Messages)
                {
                    MessageDAO MessageDAO = DbMessageDAOs
                        .Where(x => x.Id == Message.Id).FirstOrDefault();
                    if (MessageDAO == null)
                    {
                        MessageDAO = new MessageDAO();
                        MessageDAO.CreatedAt = StaticParams.DateTimeNow;
                        MessageDAO.RowId = Guid.NewGuid();
                    }
                    MessageDAO.UserId = Message.UserId;
                    MessageDAO.Content = Message.Content;
                    MessageDAO.ConversationId = Conversation.Id;
                    MessageDAO.UpdatedAt = StaticParams.DateTimeNow;
                    MessageDAO.DeletedAt = null;
                    MessageDAOs.Add(MessageDAO);
                }
                await DataContext.Message.BulkMergeAsync(MessageDAOs);
            }
        }
        
    }
}
