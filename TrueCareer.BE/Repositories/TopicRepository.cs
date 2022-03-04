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
    public interface ITopicRepository
    {
        Task<int> CountAll(TopicFilter TopicFilter);
        Task<int> Count(TopicFilter TopicFilter);
        Task<List<Topic>> List(TopicFilter TopicFilter);
        Task<List<Topic>> List(List<long> Ids);
        Task<Topic> Get(long Id);
        Task<bool> Create(Topic Topic);
        Task<bool> Update(Topic Topic);
        Task<bool> Delete(Topic Topic);
        Task<bool> BulkMerge(List<Topic> Topics);
        Task<bool> BulkDelete(List<Topic> Topics);
    }
    public class TopicRepository : ITopicRepository
    {
        private DataContext DataContext;
        public TopicRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<TopicDAO>> DynamicFilter(IQueryable<TopicDAO> query, TopicFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Title, filter.Title);
            query = query.Where(q => q.Description, filter.Description);
            query = query.Where(q => q.Cost, filter.Cost);
            return query;
        }

        private async Task<IQueryable<TopicDAO>> OrFilter(IQueryable<TopicDAO> query, TopicFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<TopicDAO> initQuery = query.Where(q => false);
            foreach (TopicFilter TopicFilter in filter.OrFilter)
            {
                IQueryable<TopicDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, TopicFilter.Id);
                queryable = queryable.Where(q => q.Title, TopicFilter.Title);
                queryable = queryable.Where(q => q.Description, TopicFilter.Description);
                queryable = queryable.Where(q => q.Cost, TopicFilter.Cost);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<TopicDAO> DynamicOrder(IQueryable<TopicDAO> query, TopicFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case TopicOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case TopicOrder.Title:
                            query = query.OrderBy(q => q.Title);
                            break;
                        case TopicOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case TopicOrder.Cost:
                            query = query.OrderBy(q => q.Cost);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case TopicOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case TopicOrder.Title:
                            query = query.OrderByDescending(q => q.Title);
                            break;
                        case TopicOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case TopicOrder.Cost:
                            query = query.OrderByDescending(q => q.Cost);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Topic>> DynamicSelect(IQueryable<TopicDAO> query, TopicFilter filter)
        {
            List<Topic> Topics = await query.Select(q => new Topic()
            {
                Id = filter.Selects.Contains(TopicSelect.Id) ? q.Id : default(long),
                Title = filter.Selects.Contains(TopicSelect.Title) ? q.Title : default(string),
                Description = filter.Selects.Contains(TopicSelect.Description) ? q.Description : default(string),
                Cost = filter.Selects.Contains(TopicSelect.Cost) ? q.Cost : default(decimal),
            }).ToListAsync();
            return Topics;
        }

        public async Task<int> CountAll(TopicFilter filter)
        {
            IQueryable<TopicDAO> TopicDAOs = DataContext.Topic.AsNoTracking();
            TopicDAOs = await DynamicFilter(TopicDAOs, filter);
            return await TopicDAOs.CountAsync();
        }

        public async Task<int> Count(TopicFilter filter)
        {
            IQueryable<TopicDAO> TopicDAOs = DataContext.Topic.AsNoTracking();
            TopicDAOs = await DynamicFilter(TopicDAOs, filter);
            TopicDAOs = await OrFilter(TopicDAOs, filter);
            return await TopicDAOs.CountAsync();
        }

        public async Task<List<Topic>> List(TopicFilter filter)
        {
            if (filter == null) return new List<Topic>();
            IQueryable<TopicDAO> TopicDAOs = DataContext.Topic.AsNoTracking();
            TopicDAOs = await DynamicFilter(TopicDAOs, filter);
            TopicDAOs = await OrFilter(TopicDAOs, filter);
            TopicDAOs = DynamicOrder(TopicDAOs, filter);
            List<Topic> Topics = await DynamicSelect(TopicDAOs, filter);
            return Topics;
        }

        public async Task<List<Topic>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<TopicDAO> query = DataContext.Topic.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Topic> Topics = await query.AsNoTracking()
            .Select(x => new Topic()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Cost = x.Cost,
            }).ToListAsync();
            

            return Topics;
        }

        public async Task<Topic> Get(long Id)
        {
            Topic Topic = await DataContext.Topic.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new Topic()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Cost = x.Cost,
            }).FirstOrDefaultAsync();

            if (Topic == null)
                return null;

            return Topic;
        }
        
        public async Task<bool> Create(Topic Topic)
        {
            TopicDAO TopicDAO = new TopicDAO();
            TopicDAO.Id = Topic.Id;
            TopicDAO.Title = Topic.Title;
            TopicDAO.Description = Topic.Description;
            TopicDAO.Cost = Topic.Cost;
            DataContext.Topic.Add(TopicDAO);
            await DataContext.SaveChangesAsync();
            Topic.Id = TopicDAO.Id;
            await SaveReference(Topic);
            return true;
        }

        public async Task<bool> Update(Topic Topic)
        {
            TopicDAO TopicDAO = DataContext.Topic
                .Where(x => x.Id == Topic.Id)
                .FirstOrDefault();
            if (TopicDAO == null)
                return false;
            TopicDAO.Id = Topic.Id;
            TopicDAO.Title = Topic.Title;
            TopicDAO.Description = Topic.Description;
            TopicDAO.Cost = Topic.Cost;
            await DataContext.SaveChangesAsync();
            await SaveReference(Topic);
            return true;
        }

        public async Task<bool> Delete(Topic Topic)
        {
            await DataContext.Topic
                .Where(x => x.Id == Topic.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Topic> Topics)
        {
            IdFilter IdFilter = new IdFilter { In = Topics.Select(x => x.Id).ToList() };
            List<TopicDAO> TopicDAOs = new List<TopicDAO>();
            List<TopicDAO> DbTopicDAOs = await DataContext.Topic
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Topic Topic in Topics)
            {
                TopicDAO TopicDAO = DbTopicDAOs
                        .Where(x => x.Id == Topic.Id)
                        .FirstOrDefault();
                if (TopicDAO == null)
                {
                    TopicDAO = new TopicDAO();
                }
                TopicDAO.Title = Topic.Title;
                TopicDAO.Description = Topic.Description;
                TopicDAO.Cost = Topic.Cost;
                TopicDAOs.Add(TopicDAO);
            }
            await DataContext.BulkMergeAsync(TopicDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Topic> Topics)
        {
            List<long> Ids = Topics.Select(x => x.Id).ToList();
            await DataContext.Topic
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Topic Topic)
        {
        }
        
    }
}
