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
    public interface IInformationRepository
    {
        Task<int> CountAll(InformationFilter InformationFilter);
        Task<int> Count(InformationFilter InformationFilter);
        Task<List<Information>> List(InformationFilter InformationFilter);
        Task<List<Information>> List(List<long> Ids);
        Task<Information> Get(long Id);
        Task<bool> Create(Information Information);
        Task<bool> Update(Information Information);
        Task<bool> Delete(Information Information);
        Task<bool> BulkMerge(List<Information> Information);
        Task<bool> BulkDelete(List<Information> Information);
    }
    public class InformationRepository : IInformationRepository
    {
        private DataContext DataContext;
        public InformationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<InformationDAO>> DynamicFilter(IQueryable<InformationDAO> query, InformationFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Description, filter.Description);
            query = query.Where(q => q.StartAt, filter.StartAt);
            query = query.Where(q => q.Role, filter.Role);
            query = query.Where(q => q.Image, filter.Image);
            query = query.Where(q => q.EndAt, filter.EndAt);
            query = query.Where(q => q.InformationTypeId, filter.InformationTypeId);
            query = query.Where(q => q.TopicId, filter.TopicId);
            query = query.Where(q => q.UserId, filter.UserId);
            return query;
        }

        private async Task<IQueryable<InformationDAO>> OrFilter(IQueryable<InformationDAO> query, InformationFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<InformationDAO> initQuery = query.Where(q => false);
            foreach (InformationFilter InformationFilter in filter.OrFilter)
            {
                IQueryable<InformationDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, InformationFilter.Id);
                queryable = queryable.Where(q => q.Name, InformationFilter.Name);
                queryable = queryable.Where(q => q.Description, InformationFilter.Description);
                queryable = queryable.Where(q => q.StartAt, InformationFilter.StartAt);
                queryable = queryable.Where(q => q.Role, InformationFilter.Role);
                queryable = queryable.Where(q => q.Image, InformationFilter.Image);
                queryable = queryable.Where(q => q.EndAt, InformationFilter.EndAt);
                queryable = queryable.Where(q => q.InformationTypeId, InformationFilter.InformationTypeId);
                queryable = queryable.Where(q => q.TopicId, InformationFilter.TopicId);
                queryable = queryable.Where(q => q.UserId, InformationFilter.UserId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<InformationDAO> DynamicOrder(IQueryable<InformationDAO> query, InformationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case InformationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case InformationOrder.InformationType:
                            query = query.OrderBy(q => q.InformationTypeId);
                            break;
                        case InformationOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case InformationOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case InformationOrder.StartAt:
                            query = query.OrderBy(q => q.StartAt);
                            break;
                        case InformationOrder.Role:
                            query = query.OrderBy(q => q.Role);
                            break;
                        case InformationOrder.Image:
                            query = query.OrderBy(q => q.Image);
                            break;
                        case InformationOrder.Topic:
                            query = query.OrderBy(q => q.TopicId);
                            break;
                        case InformationOrder.User:
                            query = query.OrderBy(q => q.UserId);
                            break;
                        case InformationOrder.EndAt:
                            query = query.OrderBy(q => q.EndAt);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case InformationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case InformationOrder.InformationType:
                            query = query.OrderByDescending(q => q.InformationTypeId);
                            break;
                        case InformationOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case InformationOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case InformationOrder.StartAt:
                            query = query.OrderByDescending(q => q.StartAt);
                            break;
                        case InformationOrder.Role:
                            query = query.OrderByDescending(q => q.Role);
                            break;
                        case InformationOrder.Image:
                            query = query.OrderByDescending(q => q.Image);
                            break;
                        case InformationOrder.Topic:
                            query = query.OrderByDescending(q => q.TopicId);
                            break;
                        case InformationOrder.User:
                            query = query.OrderByDescending(q => q.UserId);
                            break;
                        case InformationOrder.EndAt:
                            query = query.OrderByDescending(q => q.EndAt);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Information>> DynamicSelect(IQueryable<InformationDAO> query, InformationFilter filter)
        {
            List<Information> Information = await query.Select(q => new Information()
            {
                Id = filter.Selects.Contains(InformationSelect.Id) ? q.Id : default(long),
                InformationTypeId = filter.Selects.Contains(InformationSelect.InformationType) ? q.InformationTypeId : default(long),
                Name = filter.Selects.Contains(InformationSelect.Name) ? q.Name : default(string),
                Description = filter.Selects.Contains(InformationSelect.Description) ? q.Description : default(string),
                StartAt = filter.Selects.Contains(InformationSelect.StartAt) ? q.StartAt : default(DateTime),
                Role = filter.Selects.Contains(InformationSelect.Role) ? q.Role : default(string),
                Image = filter.Selects.Contains(InformationSelect.Image) ? q.Image : default(string),
                TopicId = filter.Selects.Contains(InformationSelect.Topic) ? q.TopicId : default(long),
                UserId = filter.Selects.Contains(InformationSelect.User) ? q.UserId : default(long),
                EndAt = filter.Selects.Contains(InformationSelect.EndAt) ? q.EndAt : default(DateTime),
                InformationType = filter.Selects.Contains(InformationSelect.InformationType) && q.InformationType != null ? new InformationType
                {
                    Id = q.InformationType.Id,
                    Name = q.InformationType.Name,
                    Code = q.InformationType.Code,
                } : null,
                Topic = filter.Selects.Contains(InformationSelect.Topic) && q.Topic != null ? new Topic
                {
                    Id = q.Topic.Id,
                    Title = q.Topic.Title,
                    Description = q.Topic.Description,
                    Cost = q.Topic.Cost,
                } : null,
                User = filter.Selects.Contains(InformationSelect.User) && q.User != null ? new AppUser
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
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return Information;
        }

        public async Task<int> CountAll(InformationFilter filter)
        {
            IQueryable<InformationDAO> InformationDAOs = DataContext.Information.AsNoTracking();
            InformationDAOs = await DynamicFilter(InformationDAOs, filter);
            return await InformationDAOs.CountAsync();
        }

        public async Task<int> Count(InformationFilter filter)
        {
            IQueryable<InformationDAO> InformationDAOs = DataContext.Information.AsNoTracking();
            InformationDAOs = await DynamicFilter(InformationDAOs, filter);
            InformationDAOs = await OrFilter(InformationDAOs, filter);
            return await InformationDAOs.CountAsync();
        }

        public async Task<List<Information>> List(InformationFilter filter)
        {
            if (filter == null) return new List<Information>();
            IQueryable<InformationDAO> InformationDAOs = DataContext.Information.AsNoTracking();
            InformationDAOs = await DynamicFilter(InformationDAOs, filter);
            InformationDAOs = await OrFilter(InformationDAOs, filter);
            InformationDAOs = DynamicOrder(InformationDAOs, filter);
            List<Information> Information = await DynamicSelect(InformationDAOs, filter);
            return Information;
        }

        public async Task<List<Information>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<InformationDAO> query = DataContext.Information.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Information> Information = await query.AsNoTracking()
            .Select(x => new Information()
            {
                RowId = x.RowId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                InformationTypeId = x.InformationTypeId,
                Name = x.Name,
                Description = x.Description,
                StartAt = x.StartAt,
                Role = x.Role,
                Image = x.Image,
                TopicId = x.TopicId,
                UserId = x.UserId,
                EndAt = x.EndAt,
                InformationType = x.InformationType == null ? null : new InformationType
                {
                    Id = x.InformationType.Id,
                    Name = x.InformationType.Name,
                    Code = x.InformationType.Code,
                },
                Topic = x.Topic == null ? null : new Topic
                {
                    Id = x.Topic.Id,
                    Title = x.Topic.Title,
                    Description = x.Topic.Description,
                    Cost = x.Topic.Cost,
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
            

            return Information;
        }

        public async Task<Information> Get(long Id)
        {
            Information Information = await DataContext.Information.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Information()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                InformationTypeId = x.InformationTypeId,
                Name = x.Name,
                Description = x.Description,
                StartAt = x.StartAt,
                Role = x.Role,
                Image = x.Image,
                TopicId = x.TopicId,
                UserId = x.UserId,
                EndAt = x.EndAt,
                InformationType = x.InformationType == null ? null : new InformationType
                {
                    Id = x.InformationType.Id,
                    Name = x.InformationType.Name,
                    Code = x.InformationType.Code,
                },
                Topic = x.Topic == null ? null : new Topic
                {
                    Id = x.Topic.Id,
                    Title = x.Topic.Title,
                    Description = x.Topic.Description,
                    Cost = x.Topic.Cost,
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

            if (Information == null)
                return null;

            return Information;
        }
        
        public async Task<bool> Create(Information Information)
        {
            InformationDAO InformationDAO = new InformationDAO();
            InformationDAO.Id = Information.Id;
            InformationDAO.InformationTypeId = Information.InformationTypeId;
            InformationDAO.Name = Information.Name;
            InformationDAO.Description = Information.Description;
            InformationDAO.StartAt = Information.StartAt;
            InformationDAO.Role = Information.Role;
            InformationDAO.Image = Information.Image;
            InformationDAO.TopicId = Information.TopicId;
            InformationDAO.UserId = Information.UserId;
            InformationDAO.EndAt = Information.EndAt;
            InformationDAO.RowId = Guid.NewGuid();
            InformationDAO.CreatedAt = StaticParams.DateTimeNow;
            InformationDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Information.Add(InformationDAO);
            await DataContext.SaveChangesAsync();
            Information.Id = InformationDAO.Id;
            await SaveReference(Information);
            return true;
        }

        public async Task<bool> Update(Information Information)
        {
            InformationDAO InformationDAO = DataContext.Information
                .Where(x => x.Id == Information.Id)
                .FirstOrDefault();
            if (InformationDAO == null)
                return false;
            InformationDAO.Id = Information.Id;
            InformationDAO.InformationTypeId = Information.InformationTypeId;
            InformationDAO.Name = Information.Name;
            InformationDAO.Description = Information.Description;
            InformationDAO.StartAt = Information.StartAt;
            InformationDAO.Role = Information.Role;
            InformationDAO.Image = Information.Image;
            InformationDAO.TopicId = Information.TopicId;
            InformationDAO.UserId = Information.UserId;
            InformationDAO.EndAt = Information.EndAt;
            InformationDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Information);
            return true;
        }

        public async Task<bool> Delete(Information Information)
        {
            await DataContext.Information
                .Where(x => x.Id == Information.Id)
                .UpdateFromQueryAsync(x => new InformationDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Information> Informations)
        {
            IdFilter IdFilter = new IdFilter { In = Informations.Select(x => x.Id).ToList() };
            List<InformationDAO> InformationDAOs = new List<InformationDAO>();
            List<InformationDAO> DbInformationDAOs = await DataContext.Information
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Information Information in Informations)
            {
                InformationDAO InformationDAO = DbInformationDAOs
                        .Where(x => x.Id == Information.Id)
                        .FirstOrDefault();
                if (InformationDAO == null)
                {
                    InformationDAO = new InformationDAO();
                    InformationDAO.CreatedAt = StaticParams.DateTimeNow;
                    InformationDAO.RowId = Guid.NewGuid();
                    Information.RowId = InformationDAO.RowId;
                }
                InformationDAO.InformationTypeId = Information.InformationTypeId;
                InformationDAO.Name = Information.Name;
                InformationDAO.Description = Information.Description;
                InformationDAO.StartAt = Information.StartAt;
                InformationDAO.Role = Information.Role;
                InformationDAO.Image = Information.Image;
                InformationDAO.TopicId = Information.TopicId;
                InformationDAO.UserId = Information.UserId;
                InformationDAO.EndAt = Information.EndAt;
                InformationDAO.UpdatedAt = StaticParams.DateTimeNow;
                InformationDAOs.Add(InformationDAO);
            }
            await DataContext.BulkMergeAsync(InformationDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Information> Information)
        {
            List<long> Ids = Information.Select(x => x.Id).ToList();
            await DataContext.Information
                .WhereBulkContains(Ids, x => x.Id)
                .UpdateFromQueryAsync(x => new InformationDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(Information Information)
        {
        }
        
    }
}
