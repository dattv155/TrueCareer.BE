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
    public interface IConversationParticipantRepository
    {
        Task<int> CountAll(ConversationParticipantFilter ConversationParticipantFilter);
        Task<int> Count(ConversationParticipantFilter ConversationParticipantFilter);
        Task<List<ConversationParticipant>> List(ConversationParticipantFilter ConversationParticipantFilter);
        Task<List<ConversationParticipant>> List(List<long> Ids);
        Task<ConversationParticipant> Get(long Id);
        Task<bool> Create(ConversationParticipant ConversationParticipant);
        Task<bool> Update(ConversationParticipant ConversationParticipant);
        Task<bool> Delete(ConversationParticipant ConversationParticipant);
        Task<bool> BulkMerge(List<ConversationParticipant> ConversationParticipants);
        Task<bool> BulkDelete(List<ConversationParticipant> ConversationParticipants);
    }
    public class ConversationParticipantRepository : IConversationParticipantRepository
    {
        private DataContext DataContext;
        public ConversationParticipantRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ConversationParticipantDAO>> DynamicFilter(IQueryable<ConversationParticipantDAO> query, ConversationParticipantFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.ConversationId, filter.ConversationId);
            query = query.Where(q => q.UserId, filter.UserId);
            return query;
        }

        private async Task<IQueryable<ConversationParticipantDAO>> OrFilter(IQueryable<ConversationParticipantDAO> query, ConversationParticipantFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ConversationParticipantDAO> initQuery = query.Where(q => false);
            foreach (ConversationParticipantFilter ConversationParticipantFilter in filter.OrFilter)
            {
                IQueryable<ConversationParticipantDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ConversationParticipantFilter.Id);
                queryable = queryable.Where(q => q.ConversationId, ConversationParticipantFilter.ConversationId);
                queryable = queryable.Where(q => q.UserId, ConversationParticipantFilter.UserId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ConversationParticipantDAO> DynamicOrder(IQueryable<ConversationParticipantDAO> query, ConversationParticipantFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ConversationParticipantOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ConversationParticipantOrder.Conversation:
                            query = query.OrderBy(q => q.ConversationId);
                            break;
                        case ConversationParticipantOrder.User:
                            query = query.OrderBy(q => q.UserId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ConversationParticipantOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ConversationParticipantOrder.Conversation:
                            query = query.OrderByDescending(q => q.ConversationId);
                            break;
                        case ConversationParticipantOrder.User:
                            query = query.OrderByDescending(q => q.UserId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ConversationParticipant>> DynamicSelect(IQueryable<ConversationParticipantDAO> query, ConversationParticipantFilter filter)
        {
            List<ConversationParticipant> ConversationParticipants = await query.Select(q => new ConversationParticipant()
            {
                Id = filter.Selects.Contains(ConversationParticipantSelect.Id) ? q.Id : default(long),
                ConversationId = filter.Selects.Contains(ConversationParticipantSelect.Conversation) ? q.ConversationId : default(long),
                UserId = filter.Selects.Contains(ConversationParticipantSelect.User) ? q.UserId : default(long),
                Conversation = filter.Selects.Contains(ConversationParticipantSelect.Conversation) && q.Conversation != null ? new Conversation
                {
                    Id = q.Conversation.Id,
                    LatestContent = q.Conversation.LatestContent,
                    LatestUserId = q.Conversation.LatestUserId,
                    Hash = q.Conversation.Hash,
                } : null,
                User = filter.Selects.Contains(ConversationParticipantSelect.User) && q.User != null ? new AppUser
                {
                    Id = q.User.Id,
                    Username = q.User.Username,
                    Email = q.User.Email,
                    Phone = q.User.Phone,
                    Password = q.User.Password,
                    DisplayName = q.User.DisplayName,
                    SexId = q.User.SexId,
                    Birthday = q.User.Birthday,
                    Avatar = q.User.Avatar,
                    CoverImage = q.User.CoverImage,
                } : null,
            }).ToListAsync();
            return ConversationParticipants;
        }

        public async Task<int> CountAll(ConversationParticipantFilter filter)
        {
            IQueryable<ConversationParticipantDAO> ConversationParticipantDAOs = DataContext.ConversationParticipant.AsNoTracking();
            ConversationParticipantDAOs = await DynamicFilter(ConversationParticipantDAOs, filter);
            return await ConversationParticipantDAOs.CountAsync();
        }

        public async Task<int> Count(ConversationParticipantFilter filter)
        {
            IQueryable<ConversationParticipantDAO> ConversationParticipantDAOs = DataContext.ConversationParticipant.AsNoTracking();
            ConversationParticipantDAOs = await DynamicFilter(ConversationParticipantDAOs, filter);
            ConversationParticipantDAOs = await OrFilter(ConversationParticipantDAOs, filter);
            return await ConversationParticipantDAOs.CountAsync();
        }

        public async Task<List<ConversationParticipant>> List(ConversationParticipantFilter filter)
        {
            if (filter == null) return new List<ConversationParticipant>();
            IQueryable<ConversationParticipantDAO> ConversationParticipantDAOs = DataContext.ConversationParticipant.AsNoTracking();
            ConversationParticipantDAOs = await DynamicFilter(ConversationParticipantDAOs, filter);
            ConversationParticipantDAOs = await OrFilter(ConversationParticipantDAOs, filter);
            ConversationParticipantDAOs = DynamicOrder(ConversationParticipantDAOs, filter);
            List<ConversationParticipant> ConversationParticipants = await DynamicSelect(ConversationParticipantDAOs, filter);
            return ConversationParticipants;
        }

        public async Task<List<ConversationParticipant>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ConversationParticipantDAO> query = DataContext.ConversationParticipant.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<ConversationParticipant> ConversationParticipants = await query.AsNoTracking()
            .Select(x => new ConversationParticipant()
            {
                Id = x.Id,
                ConversationId = x.ConversationId,
                UserId = x.UserId,
                Conversation = x.Conversation == null ? null : new Conversation
                {
                    Id = x.Conversation.Id,
                    LatestContent = x.Conversation.LatestContent,
                    LatestUserId = x.Conversation.LatestUserId,
                    Hash = x.Conversation.Hash,
                },
                User = x.User == null ? null : new AppUser
                {
                    Id = x.User.Id,
                    Username = x.User.Username,
                    Email = x.User.Email,
                    Phone = x.User.Phone,
                    Password = x.User.Password,
                    DisplayName = x.User.DisplayName,
                    SexId = x.User.SexId,
                    Birthday = x.User.Birthday,
                    Avatar = x.User.Avatar,
                    CoverImage = x.User.CoverImage,
                },
            }).ToListAsync();
            

            return ConversationParticipants;
        }

        public async Task<ConversationParticipant> Get(long Id)
        {
            ConversationParticipant ConversationParticipant = await DataContext.ConversationParticipant.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new ConversationParticipant()
            {
                Id = x.Id,
                ConversationId = x.ConversationId,
                UserId = x.UserId,
                Conversation = x.Conversation == null ? null : new Conversation
                {
                    Id = x.Conversation.Id,
                    LatestContent = x.Conversation.LatestContent,
                    LatestUserId = x.Conversation.LatestUserId,
                    Hash = x.Conversation.Hash,
                },
                User = x.User == null ? null : new AppUser
                {
                    Id = x.User.Id,
                    Username = x.User.Username,
                    Email = x.User.Email,
                    Phone = x.User.Phone,
                    Password = x.User.Password,
                    DisplayName = x.User.DisplayName,
                    SexId = x.User.SexId,
                    Birthday = x.User.Birthday,
                    Avatar = x.User.Avatar,
                    CoverImage = x.User.CoverImage,
                },
            }).FirstOrDefaultAsync();

            if (ConversationParticipant == null)
                return null;

            return ConversationParticipant;
        }
        
        public async Task<bool> Create(ConversationParticipant ConversationParticipant)
        {
            ConversationParticipantDAO ConversationParticipantDAO = new ConversationParticipantDAO();
            ConversationParticipantDAO.Id = ConversationParticipant.Id;
            ConversationParticipantDAO.ConversationId = ConversationParticipant.ConversationId;
            ConversationParticipantDAO.UserId = ConversationParticipant.UserId;
            DataContext.ConversationParticipant.Add(ConversationParticipantDAO);
            await DataContext.SaveChangesAsync();
            ConversationParticipant.Id = ConversationParticipantDAO.Id;
            await SaveReference(ConversationParticipant);
            return true;
        }

        public async Task<bool> Update(ConversationParticipant ConversationParticipant)
        {
            ConversationParticipantDAO ConversationParticipantDAO = DataContext.ConversationParticipant
                .Where(x => x.Id == ConversationParticipant.Id)
                .FirstOrDefault();
            if (ConversationParticipantDAO == null)
                return false;
            ConversationParticipantDAO.Id = ConversationParticipant.Id;
            ConversationParticipantDAO.ConversationId = ConversationParticipant.ConversationId;
            ConversationParticipantDAO.UserId = ConversationParticipant.UserId;
            await DataContext.SaveChangesAsync();
            await SaveReference(ConversationParticipant);
            return true;
        }

        public async Task<bool> Delete(ConversationParticipant ConversationParticipant)
        {
            await DataContext.ConversationParticipant
                .Where(x => x.Id == ConversationParticipant.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ConversationParticipant> ConversationParticipants)
        {
            IdFilter IdFilter = new IdFilter { In = ConversationParticipants.Select(x => x.Id).ToList() };
            List<ConversationParticipantDAO> ConversationParticipantDAOs = new List<ConversationParticipantDAO>();
            List<ConversationParticipantDAO> DbConversationParticipantDAOs = await DataContext.ConversationParticipant
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (ConversationParticipant ConversationParticipant in ConversationParticipants)
            {
                ConversationParticipantDAO ConversationParticipantDAO = DbConversationParticipantDAOs
                        .Where(x => x.Id == ConversationParticipant.Id)
                        .FirstOrDefault();
                if (ConversationParticipantDAO == null)
                {
                    ConversationParticipantDAO = new ConversationParticipantDAO();
                }
                ConversationParticipantDAO.ConversationId = ConversationParticipant.ConversationId;
                ConversationParticipantDAO.UserId = ConversationParticipant.UserId;
                ConversationParticipantDAOs.Add(ConversationParticipantDAO);
            }
            await DataContext.BulkMergeAsync(ConversationParticipantDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ConversationParticipant> ConversationParticipants)
        {
            List<long> Ids = ConversationParticipants.Select(x => x.Id).ToList();
            await DataContext.ConversationParticipant
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(ConversationParticipant ConversationParticipant)
        {
        }
        
    }
}
