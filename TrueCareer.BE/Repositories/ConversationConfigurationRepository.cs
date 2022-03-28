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
    public interface IConversationConfigurationRepository
    {
        Task<int> CountAll(ConversationConfigurationFilter ConversationConfigurationFilter);
        Task<int> Count(ConversationConfigurationFilter ConversationConfigurationFilter);
        Task<List<ConversationConfiguration>> List(ConversationConfigurationFilter ConversationConfigurationFilter);
        Task<List<ConversationConfiguration>> List(List<long> Ids);
        Task<ConversationConfiguration> Get(long Id);
        Task<bool> Create(ConversationConfiguration ConversationConfiguration);
        Task<bool> Update(ConversationConfiguration ConversationConfiguration);
        Task<bool> Delete(ConversationConfiguration ConversationConfiguration);
        Task<bool> BulkMerge(List<ConversationConfiguration> ConversationConfigurations);
        Task<bool> BulkDelete(List<ConversationConfiguration> ConversationConfigurations);
    }
    public class ConversationConfigurationRepository : IConversationConfigurationRepository
    {
        private DataContext DataContext;
        public ConversationConfigurationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ConversationConfigurationDAO>> DynamicFilter(IQueryable<ConversationConfigurationDAO> query, ConversationConfigurationFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.AppId, filter.AppId);
            query = query.Where(q => q.AppSecret, filter.AppSecret);
            query = query.Where(q => q.AppName, filter.AppName);
            query = query.Where(q => q.OaId, filter.OaId);
            query = query.Where(q => q.OaToken, filter.OaToken);
            query = query.Where(q => q.OaSecretKey, filter.OaSecretKey);
            query = query.Where(q => q.ExpiredAt, filter.ExpiredAt);
            query = query.Where(q => q.ConversationTypeId, filter.ConversationTypeId);
            query = query.Where(q => q.StatusId, filter.StatusId);
            return query;
        }

        private async Task<IQueryable<ConversationConfigurationDAO>> OrFilter(IQueryable<ConversationConfigurationDAO> query, ConversationConfigurationFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ConversationConfigurationDAO> initQuery = query.Where(q => false);
            foreach (ConversationConfigurationFilter ConversationConfigurationFilter in filter.OrFilter)
            {
                IQueryable<ConversationConfigurationDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ConversationConfigurationFilter.Id);
                queryable = queryable.Where(q => q.AppId, ConversationConfigurationFilter.AppId);
                queryable = queryable.Where(q => q.AppSecret, ConversationConfigurationFilter.AppSecret);
                queryable = queryable.Where(q => q.AppName, ConversationConfigurationFilter.AppName);
                queryable = queryable.Where(q => q.OaId, ConversationConfigurationFilter.OaId);
                queryable = queryable.Where(q => q.OaToken, ConversationConfigurationFilter.OaToken);
                queryable = queryable.Where(q => q.OaSecretKey, ConversationConfigurationFilter.OaSecretKey);
                queryable = queryable.Where(q => q.ExpiredAt, ConversationConfigurationFilter.ExpiredAt);
                queryable = queryable.Where(q => q.ConversationTypeId, ConversationConfigurationFilter.ConversationTypeId);
                queryable = queryable.Where(q => q.StatusId, ConversationConfigurationFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ConversationConfigurationDAO> DynamicOrder(IQueryable<ConversationConfigurationDAO> query, ConversationConfigurationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ConversationConfigurationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ConversationConfigurationOrder.AppSecret:
                            query = query.OrderBy(q => q.AppSecret);
                            break;
                        case ConversationConfigurationOrder.AppName:
                            query = query.OrderBy(q => q.AppName);
                            break;
                        case ConversationConfigurationOrder.OaToken:
                            query = query.OrderBy(q => q.OaToken);
                            break;
                        case ConversationConfigurationOrder.OaSecretKey:
                            query = query.OrderBy(q => q.OaSecretKey);
                            break;
                        case ConversationConfigurationOrder.ConversationType:
                            query = query.OrderBy(q => q.ConversationTypeId);
                            break;
                        case ConversationConfigurationOrder.ExpiredAt:
                            query = query.OrderBy(q => q.ExpiredAt);
                            break;
                        case ConversationConfigurationOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ConversationConfigurationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ConversationConfigurationOrder.AppSecret:
                            query = query.OrderByDescending(q => q.AppSecret);
                            break;
                        case ConversationConfigurationOrder.AppName:
                            query = query.OrderByDescending(q => q.AppName);
                            break;
                        case ConversationConfigurationOrder.OaToken:
                            query = query.OrderByDescending(q => q.OaToken);
                            break;
                        case ConversationConfigurationOrder.OaSecretKey:
                            query = query.OrderByDescending(q => q.OaSecretKey);
                            break;
                        case ConversationConfigurationOrder.ConversationType:
                            query = query.OrderByDescending(q => q.ConversationTypeId);
                            break;
                        case ConversationConfigurationOrder.ExpiredAt:
                            query = query.OrderByDescending(q => q.ExpiredAt);
                            break;
                        case ConversationConfigurationOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ConversationConfiguration>> DynamicSelect(IQueryable<ConversationConfigurationDAO> query, ConversationConfigurationFilter filter)
        {
            List<ConversationConfiguration> ConversationConfigurations = await query.Select(x => new ConversationConfiguration()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                AppId = x.AppId,
                AppSecret = x.AppSecret,
                AppName = x.AppName,
                OaId = x.OaId,
                OaToken = x.OaToken,
                OaSecretKey = x.OaSecretKey,
                ConversationTypeId = x.ConversationTypeId,
                ExpiredAt = x.ExpiredAt,
                StatusId = x.StatusId,
                ConversationType = x.ConversationType == null ? null : new ConversationType
                {
                    Id = x.ConversationType.Id,
                    Code = x.ConversationType.Code,
                    Name = x.ConversationType.Name,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).ToListAsync();
            return ConversationConfigurations;
        }

        public async Task<int> CountAll(ConversationConfigurationFilter filter)
        {
            IQueryable<ConversationConfigurationDAO> ConversationConfigurationDAOs = DataContext.ConversationConfiguration.AsNoTracking();
            ConversationConfigurationDAOs = await DynamicFilter(ConversationConfigurationDAOs, filter);
            return await ConversationConfigurationDAOs.CountAsync();
        }

        public async Task<int> Count(ConversationConfigurationFilter filter)
        {
            IQueryable<ConversationConfigurationDAO> ConversationConfigurationDAOs = DataContext.ConversationConfiguration.AsNoTracking();
            ConversationConfigurationDAOs = await DynamicFilter(ConversationConfigurationDAOs, filter);
            ConversationConfigurationDAOs = await OrFilter(ConversationConfigurationDAOs, filter);
            return await ConversationConfigurationDAOs.CountAsync();
        }

        public async Task<List<ConversationConfiguration>> List(ConversationConfigurationFilter filter)
        {
            if (filter == null) return new List<ConversationConfiguration>();
            IQueryable<ConversationConfigurationDAO> ConversationConfigurationDAOs = DataContext.ConversationConfiguration.AsNoTracking();
            ConversationConfigurationDAOs = await DynamicFilter(ConversationConfigurationDAOs, filter);
            ConversationConfigurationDAOs = await OrFilter(ConversationConfigurationDAOs, filter);
            ConversationConfigurationDAOs = DynamicOrder(ConversationConfigurationDAOs, filter);
            List<ConversationConfiguration> ConversationConfigurations = await DynamicSelect(ConversationConfigurationDAOs, filter);
            return ConversationConfigurations;
        }

        public async Task<List<ConversationConfiguration>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ConversationConfigurationDAO> query = DataContext.ConversationConfiguration.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<ConversationConfiguration> ConversationConfigurations = await query.AsNoTracking()
            .Select(x => new ConversationConfiguration()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                AppId = x.AppId ,
                AppSecret = x.AppSecret,
                AppName = x.AppName,
                OaId = x.OaId ,
                OaToken = x.OaToken,
                OaSecretKey = x.OaSecretKey,
                ConversationTypeId = x.ConversationTypeId,
                ExpiredAt = x.ExpiredAt,
                StatusId = x.StatusId,
                ConversationType = x.ConversationType == null ? null : new ConversationType
                {
                    Id = x.ConversationType.Id,
                    Code = x.ConversationType.Code,
                    Name = x.ConversationType.Name,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).ToListAsync();
            

            return ConversationConfigurations;
        }

        public async Task<ConversationConfiguration> Get(long Id)
        {
            ConversationConfiguration ConversationConfiguration = await DataContext.ConversationConfiguration.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new ConversationConfiguration()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                AppId = x.AppId,
                AppSecret = x.AppSecret,
                AppName = x.AppName,
                OaId = x.OaId ,
                OaToken = x.OaToken,
                OaSecretKey = x.OaSecretKey,
                ConversationTypeId = x.ConversationTypeId,
                ExpiredAt = x.ExpiredAt,
                StatusId = x.StatusId,
                ConversationType = x.ConversationType == null ? null : new ConversationType
                {
                    Id = x.ConversationType.Id,
                    Code = x.ConversationType.Code,
                    Name = x.ConversationType.Name,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (ConversationConfiguration == null)
                return null;

            return ConversationConfiguration;
        }
        
        public async Task<bool> Create(ConversationConfiguration ConversationConfiguration)
        {
            ConversationConfigurationDAO ConversationConfigurationDAO = new ConversationConfigurationDAO();
            ConversationConfigurationDAO.Id = ConversationConfiguration.Id;
            ConversationConfigurationDAO.AppId = ConversationConfiguration.AppId;
            ConversationConfigurationDAO.AppSecret = ConversationConfiguration.AppSecret;
            ConversationConfigurationDAO.AppName = ConversationConfiguration.AppName;
            ConversationConfigurationDAO.OaId = ConversationConfiguration.OaId;
            ConversationConfigurationDAO.OaToken = ConversationConfiguration.OaToken;
            ConversationConfigurationDAO.OaSecretKey = ConversationConfiguration.OaSecretKey;
            ConversationConfigurationDAO.ConversationTypeId = ConversationConfiguration.ConversationTypeId;
            ConversationConfigurationDAO.ExpiredAt = ConversationConfiguration.ExpiredAt;
            ConversationConfigurationDAO.StatusId = ConversationConfiguration.StatusId;
            ConversationConfigurationDAO.CreatedAt = StaticParams.DateTimeNow;
            ConversationConfigurationDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.ConversationConfiguration.Add(ConversationConfigurationDAO);
            await DataContext.SaveChangesAsync();
            ConversationConfiguration.Id = ConversationConfigurationDAO.Id;
            await SaveReference(ConversationConfiguration);
            return true;
        }

        public async Task<bool> Update(ConversationConfiguration ConversationConfiguration)
        {
            ConversationConfigurationDAO ConversationConfigurationDAO = DataContext.ConversationConfiguration
                .Where(x => x.Id == ConversationConfiguration.Id)
                .FirstOrDefault();
            if (ConversationConfigurationDAO == null)
                return false;
            ConversationConfigurationDAO.AppId = ConversationConfiguration.AppId;
            ConversationConfigurationDAO.AppSecret = ConversationConfiguration.AppSecret;
            ConversationConfigurationDAO.AppName = ConversationConfiguration.AppName;
            ConversationConfigurationDAO.OaId = ConversationConfiguration.OaId;
            ConversationConfigurationDAO.OaToken = ConversationConfiguration.OaToken;
            ConversationConfigurationDAO.OaSecretKey = ConversationConfiguration.OaSecretKey;
            ConversationConfigurationDAO.ConversationTypeId = ConversationConfiguration.ConversationTypeId;
            ConversationConfigurationDAO.ExpiredAt = ConversationConfiguration.ExpiredAt;
            ConversationConfigurationDAO.StatusId = ConversationConfiguration.StatusId;
            ConversationConfigurationDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ConversationConfiguration);
            return true;
        }

        public async Task<bool> Delete(ConversationConfiguration ConversationConfiguration)
        {
            await DataContext.ConversationConfiguration
                .Where(x => x.Id == ConversationConfiguration.Id)
                .UpdateFromQueryAsync(x => new ConversationConfigurationDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ConversationConfiguration> ConversationConfigurations)
        {
            IdFilter IdFilter = new IdFilter { In = ConversationConfigurations.Select(x => x.Id).ToList() };
            List<ConversationConfigurationDAO> ConversationConfigurationDAOs = new List<ConversationConfigurationDAO>();
            List<ConversationConfigurationDAO> DbConversationConfigurationDAOs = await DataContext.ConversationConfiguration
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (ConversationConfiguration ConversationConfiguration in ConversationConfigurations)
            {
                ConversationConfigurationDAO ConversationConfigurationDAO = DbConversationConfigurationDAOs
                        .Where(x => x.Id == ConversationConfiguration.Id)
                        .FirstOrDefault();
                if (ConversationConfigurationDAO == null)
                {
                    ConversationConfigurationDAO = new ConversationConfigurationDAO();
                    ConversationConfigurationDAO.CreatedAt = StaticParams.DateTimeNow;
                }
                ConversationConfigurationDAO.AppId = ConversationConfiguration.AppId;
                ConversationConfigurationDAO.AppSecret = ConversationConfiguration.AppSecret;
                ConversationConfigurationDAO.AppName = ConversationConfiguration.AppName;
                ConversationConfigurationDAO.OaId = ConversationConfiguration.OaId;
                ConversationConfigurationDAO.OaToken = ConversationConfiguration.OaToken;
                ConversationConfigurationDAO.OaSecretKey = ConversationConfiguration.OaSecretKey;
                ConversationConfigurationDAO.ConversationTypeId = ConversationConfiguration.ConversationTypeId;
                ConversationConfigurationDAO.ExpiredAt = ConversationConfiguration.ExpiredAt;
                ConversationConfigurationDAO.StatusId = ConversationConfiguration.StatusId;
                ConversationConfigurationDAO.UpdatedAt = StaticParams.DateTimeNow;
                ConversationConfigurationDAOs.Add(ConversationConfigurationDAO);
            }
            await DataContext.BulkMergeAsync(ConversationConfigurationDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ConversationConfiguration> ConversationConfigurations)
        {
            List<long> Ids = ConversationConfigurations.Select(x => x.Id).ToList();
            IdFilter IdFilter = new IdFilter { In = Ids };
            await DataContext.ConversationConfiguration
                .Where(x => x.Id, IdFilter)
                .UpdateFromQueryAsync(x => new ConversationConfigurationDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(ConversationConfiguration ConversationConfiguration)
        {
        }
        
    }
}
