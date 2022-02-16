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
    public interface IMajorRepository
    {
        Task<int> CountAll(MajorFilter MajorFilter);
        Task<int> Count(MajorFilter MajorFilter);
        Task<List<Major>> List(MajorFilter MajorFilter);
        Task<List<Major>> List(List<long> Ids);
        Task<Major> Get(long Id);
        Task<bool> Create(Major Major);
        Task<bool> Update(Major Major);
        Task<bool> Delete(Major Major);
        Task<bool> BulkMerge(List<Major> Majors);
        Task<bool> BulkDelete(List<Major> Majors);
    }
    public class MajorRepository : IMajorRepository
    {
        private DataContext DataContext;
        public MajorRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<MajorDAO>> DynamicFilter(IQueryable<MajorDAO> query, MajorFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Description, filter.Description);
            return query;
        }

        private async Task<IQueryable<MajorDAO>> OrFilter(IQueryable<MajorDAO> query, MajorFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MajorDAO> initQuery = query.Where(q => false);
            foreach (MajorFilter MajorFilter in filter.OrFilter)
            {
                IQueryable<MajorDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, MajorFilter.Id);
                queryable = queryable.Where(q => q.Name, MajorFilter.Name);
                queryable = queryable.Where(q => q.Description, MajorFilter.Description);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<MajorDAO> DynamicOrder(IQueryable<MajorDAO> query, MajorFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MajorOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case MajorOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case MajorOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MajorOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case MajorOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case MajorOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Major>> DynamicSelect(IQueryable<MajorDAO> query, MajorFilter filter)
        {
            List<Major> Majors = await query.Select(q => new Major()
            {
                Id = filter.Selects.Contains(MajorSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(MajorSelect.Name) ? q.Name : default(string),
                Description = filter.Selects.Contains(MajorSelect.Description) ? q.Description : default(string),
            }).ToListAsync();
            return Majors;
        }

        public async Task<int> CountAll(MajorFilter filter)
        {
            IQueryable<MajorDAO> MajorDAOs = DataContext.Major.AsNoTracking();
            MajorDAOs = await DynamicFilter(MajorDAOs, filter);
            return await MajorDAOs.CountAsync();
        }

        public async Task<int> Count(MajorFilter filter)
        {
            IQueryable<MajorDAO> MajorDAOs = DataContext.Major.AsNoTracking();
            MajorDAOs = await DynamicFilter(MajorDAOs, filter);
            MajorDAOs = await OrFilter(MajorDAOs, filter);
            return await MajorDAOs.CountAsync();
        }

        public async Task<List<Major>> List(MajorFilter filter)
        {
            if (filter == null) return new List<Major>();
            IQueryable<MajorDAO> MajorDAOs = DataContext.Major.AsNoTracking();
            MajorDAOs = await DynamicFilter(MajorDAOs, filter);
            MajorDAOs = await OrFilter(MajorDAOs, filter);
            MajorDAOs = DynamicOrder(MajorDAOs, filter);
            List<Major> Majors = await DynamicSelect(MajorDAOs, filter);
            return Majors;
        }

        public async Task<List<Major>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<MajorDAO> query = DataContext.Major.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Major> Majors = await query.AsNoTracking()
            .Select(x => new Major()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            }).ToListAsync();
            

            return Majors;
        }

        public async Task<Major> Get(long Id)
        {
            Major Major = await DataContext.Major.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new Major()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            }).FirstOrDefaultAsync();

            if (Major == null)
                return null;

            return Major;
        }
        
        public async Task<bool> Create(Major Major)
        {
            MajorDAO MajorDAO = new MajorDAO();
            MajorDAO.Id = Major.Id;
            MajorDAO.Name = Major.Name;
            MajorDAO.Description = Major.Description;
            DataContext.Major.Add(MajorDAO);
            await DataContext.SaveChangesAsync();
            Major.Id = MajorDAO.Id;
            await SaveReference(Major);
            return true;
        }

        public async Task<bool> Update(Major Major)
        {
            MajorDAO MajorDAO = DataContext.Major
                .Where(x => x.Id == Major.Id)
                .FirstOrDefault();
            if (MajorDAO == null)
                return false;
            MajorDAO.Id = Major.Id;
            MajorDAO.Name = Major.Name;
            MajorDAO.Description = Major.Description;
            await DataContext.SaveChangesAsync();
            await SaveReference(Major);
            return true;
        }

        public async Task<bool> Delete(Major Major)
        {
            await DataContext.Major
                .Where(x => x.Id == Major.Id)
                .DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Major> Majors)
        {
            IdFilter IdFilter = new IdFilter { In = Majors.Select(x => x.Id).ToList() };
            List<MajorDAO> MajorDAOs = new List<MajorDAO>();
            List<MajorDAO> DbMajorDAOs = await DataContext.Major
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Major Major in Majors)
            {
                MajorDAO MajorDAO = DbMajorDAOs
                        .Where(x => x.Id == Major.Id)
                        .FirstOrDefault();
                if (MajorDAO == null)
                {
                    MajorDAO = new MajorDAO();
                }
                MajorDAO.Name = Major.Name;
                MajorDAO.Description = Major.Description;
                MajorDAOs.Add(MajorDAO);
            }
            await DataContext.BulkMergeAsync(MajorDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Major> Majors)
        {
            List<long> Ids = Majors.Select(x => x.Id).ToList();
            await DataContext.Major
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Major Major)
        {
        }
        
    }
}
