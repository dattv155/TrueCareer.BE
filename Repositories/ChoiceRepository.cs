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
    public interface IChoiceRepository
    {
        Task<int> CountAll(ChoiceFilter ChoiceFilter);
        Task<int> Count(ChoiceFilter ChoiceFilter);
        Task<List<Choice>> List(ChoiceFilter ChoiceFilter);
        Task<List<Choice>> List(List<long> Ids);
        Task<Choice> Get(long Id);
        Task<bool> Create(Choice Choice);
        Task<bool> Update(Choice Choice);
        Task<bool> Delete(Choice Choice);
        Task<bool> BulkMerge(List<Choice> Choices);
        Task<bool> BulkDelete(List<Choice> Choices);
    }
    public class ChoiceRepository : IChoiceRepository
    {
        private DataContext DataContext;
        public ChoiceRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ChoiceDAO>> DynamicFilter(IQueryable<ChoiceDAO> query, ChoiceFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.ChoiceContent, filter.ChoiceContent);
            query = query.Where(q => q.Description, filter.Description);
            query = query.Where(q => q.MbtiSingleTypeId, filter.MbtiSingleTypeId);
            query = query.Where(q => q.QuestionId, filter.QuestionId);
            return query;
        }

        private async Task<IQueryable<ChoiceDAO>> OrFilter(IQueryable<ChoiceDAO> query, ChoiceFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ChoiceDAO> initQuery = query.Where(q => false);
            foreach (ChoiceFilter ChoiceFilter in filter.OrFilter)
            {
                IQueryable<ChoiceDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ChoiceFilter.Id);
                queryable = queryable.Where(q => q.ChoiceContent, ChoiceFilter.ChoiceContent);
                queryable = queryable.Where(q => q.Description, ChoiceFilter.Description);
                queryable = queryable.Where(q => q.MbtiSingleTypeId, ChoiceFilter.MbtiSingleTypeId);
                queryable = queryable.Where(q => q.QuestionId, ChoiceFilter.QuestionId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ChoiceDAO> DynamicOrder(IQueryable<ChoiceDAO> query, ChoiceFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ChoiceOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ChoiceOrder.ChoiceContent:
                            query = query.OrderBy(q => q.ChoiceContent);
                            break;
                        case ChoiceOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case ChoiceOrder.Question:
                            query = query.OrderBy(q => q.QuestionId);
                            break;
                        case ChoiceOrder.MbtiSingleType:
                            query = query.OrderBy(q => q.MbtiSingleTypeId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ChoiceOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ChoiceOrder.ChoiceContent:
                            query = query.OrderByDescending(q => q.ChoiceContent);
                            break;
                        case ChoiceOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case ChoiceOrder.Question:
                            query = query.OrderByDescending(q => q.QuestionId);
                            break;
                        case ChoiceOrder.MbtiSingleType:
                            query = query.OrderByDescending(q => q.MbtiSingleTypeId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Choice>> DynamicSelect(IQueryable<ChoiceDAO> query, ChoiceFilter filter)
        {
            List<Choice> Choices = await query.Select(q => new Choice()
            {
                Id = filter.Selects.Contains(ChoiceSelect.Id) ? q.Id : default(long),
                ChoiceContent = filter.Selects.Contains(ChoiceSelect.ChoiceContent) ? q.ChoiceContent : default(string),
                Description = filter.Selects.Contains(ChoiceSelect.Description) ? q.Description : default(string),
                QuestionId = filter.Selects.Contains(ChoiceSelect.Question) ? q.QuestionId : default(long),
                MbtiSingleTypeId = filter.Selects.Contains(ChoiceSelect.MbtiSingleType) ? q.MbtiSingleTypeId : default(long),
                MbtiSingleType = filter.Selects.Contains(ChoiceSelect.MbtiSingleType) && q.MbtiSingleType != null ? new MbtiSingleType
                {
                    Id = q.MbtiSingleType.Id,
                    Code = q.MbtiSingleType.Code,
                    Name = q.MbtiSingleType.Name,
                } : null,
                Question = filter.Selects.Contains(ChoiceSelect.Question) && q.Question != null ? new Question
                {
                    Id = q.Question.Id,
                    QuestionContent = q.Question.QuestionContent,
                    Description = q.Question.Description,
                } : null,
            }).ToListAsync();
            return Choices;
        }

        public async Task<int> CountAll(ChoiceFilter filter)
        {
            IQueryable<ChoiceDAO> ChoiceDAOs = DataContext.Choice.AsNoTracking();
            ChoiceDAOs = await DynamicFilter(ChoiceDAOs, filter);
            return await ChoiceDAOs.CountAsync();
        }

        public async Task<int> Count(ChoiceFilter filter)
        {
            IQueryable<ChoiceDAO> ChoiceDAOs = DataContext.Choice.AsNoTracking();
            ChoiceDAOs = await DynamicFilter(ChoiceDAOs, filter);
            ChoiceDAOs = await OrFilter(ChoiceDAOs, filter);
            return await ChoiceDAOs.CountAsync();
        }

        public async Task<List<Choice>> List(ChoiceFilter filter)
        {
            if (filter == null) return new List<Choice>();
            IQueryable<ChoiceDAO> ChoiceDAOs = DataContext.Choice.AsNoTracking();
            ChoiceDAOs = await DynamicFilter(ChoiceDAOs, filter);
            ChoiceDAOs = await OrFilter(ChoiceDAOs, filter);
            ChoiceDAOs = DynamicOrder(ChoiceDAOs, filter);
            List<Choice> Choices = await DynamicSelect(ChoiceDAOs, filter);
            return Choices;
        }

        public async Task<List<Choice>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ChoiceDAO> query = DataContext.Choice.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Choice> Choices = await query.AsNoTracking()
            .Select(x => new Choice()
            {
                Id = x.Id,
                ChoiceContent = x.ChoiceContent,
                Description = x.Description,
                QuestionId = x.QuestionId,
                MbtiSingleTypeId = x.MbtiSingleTypeId,
                MbtiSingleType = x.MbtiSingleType == null ? null : new MbtiSingleType
                {
                    Id = x.MbtiSingleType.Id,
                    Code = x.MbtiSingleType.Code,
                    Name = x.MbtiSingleType.Name,
                },
                Question = x.Question == null ? null : new Question
                {
                    Id = x.Question.Id,
                    QuestionContent = x.Question.QuestionContent,
                    Description = x.Question.Description,
                },
            }).ToListAsync();
            

            return Choices;
        }

        public async Task<Choice> Get(long Id)
        {
            Choice Choice = await DataContext.Choice.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new Choice()
            {
                Id = x.Id,
                ChoiceContent = x.ChoiceContent,
                Description = x.Description,
                QuestionId = x.QuestionId,
                MbtiSingleTypeId = x.MbtiSingleTypeId,
                MbtiSingleType = x.MbtiSingleType == null ? null : new MbtiSingleType
                {
                    Id = x.MbtiSingleType.Id,
                    Code = x.MbtiSingleType.Code,
                    Name = x.MbtiSingleType.Name,
                },
                Question = x.Question == null ? null : new Question
                {
                    Id = x.Question.Id,
                    QuestionContent = x.Question.QuestionContent,
                    Description = x.Question.Description,
                },
            }).FirstOrDefaultAsync();

            if (Choice == null)
                return null;

            return Choice;
        }
        
        public async Task<bool> Create(Choice Choice)
        {
            ChoiceDAO ChoiceDAO = new ChoiceDAO();
            ChoiceDAO.Id = Choice.Id;
            ChoiceDAO.ChoiceContent = Choice.ChoiceContent;
            ChoiceDAO.Description = Choice.Description;
            ChoiceDAO.QuestionId = Choice.QuestionId;
            ChoiceDAO.MbtiSingleTypeId = Choice.MbtiSingleTypeId;
            DataContext.Choice.Add(ChoiceDAO);
            await DataContext.SaveChangesAsync();
            Choice.Id = ChoiceDAO.Id;
            await SaveReference(Choice);
            return true;
        }

        public async Task<bool> Update(Choice Choice)
        {
            ChoiceDAO ChoiceDAO = DataContext.Choice
                .Where(x => x.Id == Choice.Id)
                .FirstOrDefault();
            if (ChoiceDAO == null)
                return false;
            ChoiceDAO.Id = Choice.Id;
            ChoiceDAO.ChoiceContent = Choice.ChoiceContent;
            ChoiceDAO.Description = Choice.Description;
            ChoiceDAO.QuestionId = Choice.QuestionId;
            ChoiceDAO.MbtiSingleTypeId = Choice.MbtiSingleTypeId;
            await DataContext.SaveChangesAsync();
            await SaveReference(Choice);
            return true;
        }

        public async Task<bool> Delete(Choice Choice)
        {
            await DataContext.Choice
                .Where(x => x.Id == Choice.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Choice> Choices)
        {
            IdFilter IdFilter = new IdFilter { In = Choices.Select(x => x.Id).ToList() };
            List<ChoiceDAO> ChoiceDAOs = new List<ChoiceDAO>();
            List<ChoiceDAO> DbChoiceDAOs = await DataContext.Choice
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Choice Choice in Choices)
            {
                ChoiceDAO ChoiceDAO = DbChoiceDAOs
                        .Where(x => x.Id == Choice.Id)
                        .FirstOrDefault();
                if (ChoiceDAO == null)
                {
                    ChoiceDAO = new ChoiceDAO();
                }
                ChoiceDAO.ChoiceContent = Choice.ChoiceContent;
                ChoiceDAO.Description = Choice.Description;
                ChoiceDAO.QuestionId = Choice.QuestionId;
                ChoiceDAO.MbtiSingleTypeId = Choice.MbtiSingleTypeId;
                ChoiceDAOs.Add(ChoiceDAO);
            }
            await DataContext.BulkMergeAsync(ChoiceDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Choice> Choices)
        {
            List<long> Ids = Choices.Select(x => x.Id).ToList();
            await DataContext.Choice
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Choice Choice)
        {
        }
        
    }
}
