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
    public interface ISchoolRepository
    {
        Task<int> CountAll(SchoolFilter SchoolFilter);
        Task<int> Count(SchoolFilter SchoolFilter);
        Task<List<School>> List(SchoolFilter SchoolFilter);
        Task<List<School>> List(List<long> Ids);
        Task<School> Get(long Id);
        Task<bool> Create(School School);
        Task<bool> Update(School School);
        Task<bool> Delete(School School);
        Task<bool> BulkMerge(List<School> Schools);
        Task<bool> BulkDelete(List<School> Schools);
    }
    public class SchoolRepository : ISchoolRepository
    {
        private DataContext DataContext;
        public SchoolRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<SchoolDAO>> DynamicFilter(IQueryable<SchoolDAO> query, SchoolFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Description, filter.Description);
            return query;
        }

        private async Task<IQueryable<SchoolDAO>> OrFilter(IQueryable<SchoolDAO> query, SchoolFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<SchoolDAO> initQuery = query.Where(q => false);
            foreach (SchoolFilter SchoolFilter in filter.OrFilter)
            {
                IQueryable<SchoolDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, SchoolFilter.Id);
                queryable = queryable.Where(q => q.Name, SchoolFilter.Name);
                queryable = queryable.Where(q => q.Description, SchoolFilter.Description);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<SchoolDAO> DynamicOrder(IQueryable<SchoolDAO> query, SchoolFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SchoolOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SchoolOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case SchoolOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SchoolOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SchoolOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case SchoolOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<School>> DynamicSelect(IQueryable<SchoolDAO> query, SchoolFilter filter)
        {
            List<School> Schools = await query.Select(q => new School()
            {
                Id = filter.Selects.Contains(SchoolSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(SchoolSelect.Name) ? q.Name : default(string),
                Description = filter.Selects.Contains(SchoolSelect.Description) ? q.Description : default(string),
                RowId = q.RowId,
            }).ToListAsync();
            return Schools;
        }

        public async Task<int> CountAll(SchoolFilter filter)
        {
            IQueryable<SchoolDAO> SchoolDAOs = DataContext.School.AsNoTracking();
            SchoolDAOs = await DynamicFilter(SchoolDAOs, filter);
            return await SchoolDAOs.CountAsync();
        }

        public async Task<int> Count(SchoolFilter filter)
        {
            IQueryable<SchoolDAO> SchoolDAOs = DataContext.School.AsNoTracking();
            SchoolDAOs = await DynamicFilter(SchoolDAOs, filter);
            SchoolDAOs = await OrFilter(SchoolDAOs, filter);
            return await SchoolDAOs.CountAsync();
        }

        public async Task<List<School>> List(SchoolFilter filter)
        {
            if (filter == null) return new List<School>();
            IQueryable<SchoolDAO> SchoolDAOs = DataContext.School.AsNoTracking();
            SchoolDAOs = await DynamicFilter(SchoolDAOs, filter);
            SchoolDAOs = await OrFilter(SchoolDAOs, filter);
            SchoolDAOs = DynamicOrder(SchoolDAOs, filter);
            List<School> Schools = await DynamicSelect(SchoolDAOs, filter);
            return Schools;
        }

        public async Task<List<School>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<SchoolDAO> query = DataContext.School.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<School> Schools = await query.AsNoTracking()
            .Select(x => new School()
            {
                RowId = x.RowId,
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            }).ToListAsync();
            

            return Schools;
        }

        public async Task<School> Get(long Id)
        {
            School School = await DataContext.School.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new School()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            }).FirstOrDefaultAsync();

            if (School == null)
                return null;

            return School;
        }
        
        public async Task<bool> Create(School School)
        {
            SchoolDAO SchoolDAO = new SchoolDAO();
            SchoolDAO.Id = School.Id;
            SchoolDAO.Name = School.Name;
            SchoolDAO.Description = School.Description;
            SchoolDAO.RowId = Guid.NewGuid();
            DataContext.School.Add(SchoolDAO);
            await DataContext.SaveChangesAsync();
            School.Id = SchoolDAO.Id;
            await SaveReference(School);
            return true;
        }

        public async Task<bool> Update(School School)
        {
            SchoolDAO SchoolDAO = DataContext.School
                .Where(x => x.Id == School.Id)
                .FirstOrDefault();
            if (SchoolDAO == null)
                return false;
            SchoolDAO.Id = School.Id;
            SchoolDAO.Name = School.Name;
            SchoolDAO.Description = School.Description;
            await DataContext.SaveChangesAsync();
            await SaveReference(School);
            return true;
        }

        public async Task<bool> Delete(School School)
        {
            await DataContext.School
                .Where(x => x.Id == School.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<School> Schools)
        {
            IdFilter IdFilter = new IdFilter { In = Schools.Select(x => x.Id).ToList() };
            List<SchoolDAO> SchoolDAOs = new List<SchoolDAO>();
            List<SchoolDAO> DbSchoolDAOs = await DataContext.School
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (School School in Schools)
            {
                SchoolDAO SchoolDAO = DbSchoolDAOs
                        .Where(x => x.Id == School.Id)
                        .FirstOrDefault();
                if (SchoolDAO == null)
                {
                    SchoolDAO = new SchoolDAO();
                    SchoolDAO.RowId = Guid.NewGuid();
                    School.RowId = SchoolDAO.RowId;
                }
                SchoolDAO.Name = School.Name;
                SchoolDAO.Description = School.Description;
                SchoolDAOs.Add(SchoolDAO);
            }
            await DataContext.BulkMergeAsync(SchoolDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<School> Schools)
        {
            List<long> Ids = Schools.Select(x => x.Id).ToList();
            await DataContext.School
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(School School)
        {
        }
        
    }
}
