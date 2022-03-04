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
    public interface IQuestionRepository
    {
        Task<int> CountAll(QuestionFilter QuestionFilter);
        Task<int> Count(QuestionFilter QuestionFilter);
        Task<List<Question>> List(QuestionFilter QuestionFilter);
        Task<List<Question>> List(List<long> Ids);
        Task<Question> Get(long Id);
        Task<bool> Create(Question Question);
        Task<bool> Update(Question Question);
        Task<bool> Delete(Question Question);
        Task<bool> BulkMerge(List<Question> Questions);
        Task<bool> BulkDelete(List<Question> Questions);
    }
    public class QuestionRepository : IQuestionRepository
    {
        private DataContext DataContext;
        public QuestionRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<QuestionDAO>> DynamicFilter(IQueryable<QuestionDAO> query, QuestionFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.QuestionContent, filter.QuestionContent);
            query = query.Where(q => q.Description, filter.Description);
            return query;
        }

        private async Task<IQueryable<QuestionDAO>> OrFilter(IQueryable<QuestionDAO> query, QuestionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<QuestionDAO> initQuery = query.Where(q => false);
            foreach (QuestionFilter QuestionFilter in filter.OrFilter)
            {
                IQueryable<QuestionDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, QuestionFilter.Id);
                queryable = queryable.Where(q => q.QuestionContent, QuestionFilter.QuestionContent);
                queryable = queryable.Where(q => q.Description, QuestionFilter.Description);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<QuestionDAO> DynamicOrder(IQueryable<QuestionDAO> query, QuestionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case QuestionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case QuestionOrder.QuestionContent:
                            query = query.OrderBy(q => q.QuestionContent);
                            break;
                        case QuestionOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case QuestionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case QuestionOrder.QuestionContent:
                            query = query.OrderByDescending(q => q.QuestionContent);
                            break;
                        case QuestionOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Question>> DynamicSelect(IQueryable<QuestionDAO> query, QuestionFilter filter)
        {
            List<Question> Questions = await query.Select(q => new Question()
            {
                Id = filter.Selects.Contains(QuestionSelect.Id) ? q.Id : default(long),
                QuestionContent = filter.Selects.Contains(QuestionSelect.QuestionContent) ? q.QuestionContent : default(string),
                Description = filter.Selects.Contains(QuestionSelect.Description) ? q.Description : default(string),
            }).ToListAsync();
            return Questions;
        }

        public async Task<int> CountAll(QuestionFilter filter)
        {
            IQueryable<QuestionDAO> QuestionDAOs = DataContext.Question.AsNoTracking();
            QuestionDAOs = await DynamicFilter(QuestionDAOs, filter);
            return await QuestionDAOs.CountAsync();
        }

        public async Task<int> Count(QuestionFilter filter)
        {
            IQueryable<QuestionDAO> QuestionDAOs = DataContext.Question.AsNoTracking();
            QuestionDAOs = await DynamicFilter(QuestionDAOs, filter);
            QuestionDAOs = await OrFilter(QuestionDAOs, filter);
            return await QuestionDAOs.CountAsync();
        }

        public async Task<List<Question>> List(QuestionFilter filter)
        {
            if (filter == null) return new List<Question>();
            IQueryable<QuestionDAO> QuestionDAOs = DataContext.Question.AsNoTracking();
            QuestionDAOs = await DynamicFilter(QuestionDAOs, filter);
            QuestionDAOs = await OrFilter(QuestionDAOs, filter);
            QuestionDAOs = DynamicOrder(QuestionDAOs, filter);
            List<Question> Questions = await DynamicSelect(QuestionDAOs, filter);
            return Questions;
        }

        public async Task<List<Question>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<QuestionDAO> query = DataContext.Question.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Question> Questions = await query.AsNoTracking()
            .Select(x => new Question()
            {
                Id = x.Id,
                QuestionContent = x.QuestionContent,
                Description = x.Description,
            }).ToListAsync();
            
            var ChoiceQuery = DataContext.Choice.AsNoTracking()
                .Where(x => x.QuestionId, IdFilter);
            List<Choice> Choices = await ChoiceQuery
                .Select(x => new Choice
                {
                    Id = x.Id,
                    ChoiceContent = x.ChoiceContent,
                    Description = x.Description,
                    QuestionId = x.QuestionId,
                    MbtiSingleTypeId = x.MbtiSingleTypeId,
                    MbtiSingleType = new MbtiSingleType
                    {
                        Id = x.MbtiSingleType.Id,
                        Code = x.MbtiSingleType.Code,
                        Name = x.MbtiSingleType.Name,
                    },
                }).ToListAsync();

            foreach(Question Question in Questions)
            {
                Question.Choices = Choices
                    .Where(x => x.QuestionId == Question.Id)
                    .ToList();
            }


            return Questions;
        }

        public async Task<Question> Get(long Id)
        {
            Question Question = await DataContext.Question.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new Question()
            {
                Id = x.Id,
                QuestionContent = x.QuestionContent,
                Description = x.Description,
            }).FirstOrDefaultAsync();

            if (Question == null)
                return null;
            Question.Choices = await DataContext.Choice.AsNoTracking()
                .Where(x => x.QuestionId == Question.Id)
                .Select(x => new Choice
                {
                    Id = x.Id,
                    ChoiceContent = x.ChoiceContent,
                    Description = x.Description,
                    QuestionId = x.QuestionId,
                    MbtiSingleTypeId = x.MbtiSingleTypeId,
                    MbtiSingleType = new MbtiSingleType
                    {
                        Id = x.MbtiSingleType.Id,
                        Code = x.MbtiSingleType.Code,
                        Name = x.MbtiSingleType.Name,
                    },
                }).ToListAsync();

            return Question;
        }
        
        public async Task<bool> Create(Question Question)
        {
            QuestionDAO QuestionDAO = new QuestionDAO();
            QuestionDAO.Id = Question.Id;
            QuestionDAO.QuestionContent = Question.QuestionContent;
            QuestionDAO.Description = Question.Description;
            DataContext.Question.Add(QuestionDAO);
            await DataContext.SaveChangesAsync();
            Question.Id = QuestionDAO.Id;
            await SaveReference(Question);
            return true;
        }

        public async Task<bool> Update(Question Question)
        {
            QuestionDAO QuestionDAO = DataContext.Question
                .Where(x => x.Id == Question.Id)
                .FirstOrDefault();
            if (QuestionDAO == null)
                return false;
            QuestionDAO.Id = Question.Id;
            QuestionDAO.QuestionContent = Question.QuestionContent;
            QuestionDAO.Description = Question.Description;
            await DataContext.SaveChangesAsync();
            await SaveReference(Question);
            return true;
        }

        public async Task<bool> Delete(Question Question)
        {
            await DataContext.Question
                .Where(x => x.Id == Question.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Question> Questions)
        {
            IdFilter IdFilter = new IdFilter { In = Questions.Select(x => x.Id).ToList() };
            List<QuestionDAO> QuestionDAOs = new List<QuestionDAO>();
            List<QuestionDAO> DbQuestionDAOs = await DataContext.Question
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Question Question in Questions)
            {
                QuestionDAO QuestionDAO = DbQuestionDAOs
                        .Where(x => x.Id == Question.Id)
                        .FirstOrDefault();
                if (QuestionDAO == null)
                {
                    QuestionDAO = new QuestionDAO();
                }
                QuestionDAO.QuestionContent = Question.QuestionContent;
                QuestionDAO.Description = Question.Description;
                QuestionDAOs.Add(QuestionDAO);
            }
            await DataContext.BulkMergeAsync(QuestionDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Question> Questions)
        {
            List<long> Ids = Questions.Select(x => x.Id).ToList();
            await DataContext.Question
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Question Question)
        {
            await DataContext.Choice
                .Where(x => x.QuestionId == Question.Id)
                .DeleteFromQueryAsync();
            List<ChoiceDAO> ChoiceDAOs = new List<ChoiceDAO>();
            if (Question.Choices != null)
            {
                foreach (Choice Choice in Question.Choices)
                {
                    ChoiceDAO ChoiceDAO = new ChoiceDAO();
                    ChoiceDAO.Id = Choice.Id;
                    ChoiceDAO.ChoiceContent = Choice.ChoiceContent;
                    ChoiceDAO.Description = Choice.Description;
                    ChoiceDAO.QuestionId = Question.Id;
                    ChoiceDAO.MbtiSingleTypeId = Choice.MbtiSingleTypeId;
                    ChoiceDAOs.Add(ChoiceDAO);
                }
                await DataContext.Choice.BulkMergeAsync(ChoiceDAOs);
            }
        }
        
    }
}
