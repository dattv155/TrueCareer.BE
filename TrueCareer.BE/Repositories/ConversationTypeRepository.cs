using TrueSight.Common;
using TrueCareer.Helpers;
using TrueCareer.Entities;
using TrueCareer.BE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Repositories
{
    public interface IConversationTypeRepository
    {
        Task<int> Count(ConversationTypeFilter ConversationTypeFilter);
        Task<List<ConversationType>> List(ConversationTypeFilter ConversationTypeFilter);
        Task<List<ConversationType>> List(List<long> Ids);
        Task<ConversationType> Get(long Id);
    }
    public class ConversationTypeRepository : IConversationTypeRepository
    {
        private DataContext DataContext;
        public ConversationTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ConversationTypeDAO> DynamicFilter(IQueryable<ConversationTypeDAO> query, ConversationTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ConversationTypeDAO> OrFilter(IQueryable<ConversationTypeDAO> query, ConversationTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ConversationTypeDAO> initQuery = query.Where(q => false);
            foreach (ConversationTypeFilter ConversationTypeFilter in filter.OrFilter)
            {
                IQueryable<ConversationTypeDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.Code, filter.Code);
                queryable = queryable.Where(q => q.Name, filter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ConversationTypeDAO> DynamicOrder(IQueryable<ConversationTypeDAO> query, ConversationTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ConversationTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ConversationTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ConversationTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ConversationTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ConversationTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ConversationTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ConversationType>> DynamicSelect(IQueryable<ConversationTypeDAO> query, ConversationTypeFilter filter)
        {
            List<ConversationType> ConversationTypes = await query.Select(q => new ConversationType()
            {
                Id = filter.Selects.Contains(ConversationTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ConversationTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ConversationTypeSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return ConversationTypes;
        }

        public async Task<int> Count(ConversationTypeFilter filter)
        {
            IQueryable<ConversationTypeDAO> ConversationTypes = DataContext.ConversationType.AsNoTracking();
            ConversationTypes = DynamicFilter(ConversationTypes, filter);
            return await ConversationTypes.CountAsync();
        }

        public async Task<List<ConversationType>> List(ConversationTypeFilter filter)
        {
            if (filter == null) return new List<ConversationType>();
            IQueryable<ConversationTypeDAO> ConversationTypeDAOs = DataContext.ConversationType.AsNoTracking();
            ConversationTypeDAOs = DynamicFilter(ConversationTypeDAOs, filter);
            ConversationTypeDAOs = DynamicOrder(ConversationTypeDAOs, filter);
            List<ConversationType> ConversationTypes = await DynamicSelect(ConversationTypeDAOs, filter);
            return ConversationTypes;
        }

        public async Task<List<ConversationType>> List(List<long> Ids)
        {
            List<ConversationType> ConversationTypes = await DataContext.ConversationType.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new ConversationType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return ConversationTypes;
        }

        public async Task<ConversationType> Get(long Id)
        {
            ConversationType ConversationType = await DataContext.ConversationType.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new ConversationType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (ConversationType == null)
                return null;

            return ConversationType;
        }
    }
}
