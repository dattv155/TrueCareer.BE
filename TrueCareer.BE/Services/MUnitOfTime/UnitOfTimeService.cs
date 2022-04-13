using TrueSight.Common;
using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using TrueCareer.Repositories;
using TrueCareer.Entities;
using TrueCareer.Enums;

namespace TrueCareer.Services.MUnitOfTime
{
    public interface IUnitOfTimeService :  IServiceScoped
    {
        Task<int> Count(UnitOfTimeFilter UnitOfTimeFilter);
        Task<List<UnitOfTime>> List(UnitOfTimeFilter UnitOfTimeFilter);
        Task<UnitOfTime> Get(long Id);
        Task<UnitOfTime> Create(UnitOfTime UnitOfTime);
        Task<UnitOfTime> Update(UnitOfTime UnitOfTime);
        Task<UnitOfTime> Delete(UnitOfTime UnitOfTime);
        Task<List<UnitOfTime>> BulkDelete(List<UnitOfTime> UnitOfTimes);
        Task<List<UnitOfTime>> Import(List<UnitOfTime> UnitOfTimes);
        Task<UnitOfTimeFilter> ToFilter(UnitOfTimeFilter UnitOfTimeFilter);
    }

    public class UnitOfTimeService : BaseService, IUnitOfTimeService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IUnitOfTimeValidator UnitOfTimeValidator;

        public UnitOfTimeService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IUnitOfTimeValidator UnitOfTimeValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.UnitOfTimeValidator = UnitOfTimeValidator;
        }
        public async Task<int> Count(UnitOfTimeFilter UnitOfTimeFilter)
        {
            try
            {
                int result = await UOW.UnitOfTimeRepository.Count(UnitOfTimeFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(UnitOfTimeService));
            }
            return 0;
        }

        public async Task<List<UnitOfTime>> List(UnitOfTimeFilter UnitOfTimeFilter)
        {
            try
            {
                List<UnitOfTime> UnitOfTimes = await UOW.UnitOfTimeRepository.List(UnitOfTimeFilter);
                return UnitOfTimes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(UnitOfTimeService));
            }
            return null;
        }

        public async Task<UnitOfTime> Get(long Id)
        {
            UnitOfTime UnitOfTime = await UOW.UnitOfTimeRepository.Get(Id);
            await UnitOfTimeValidator.Get(UnitOfTime);
            if (UnitOfTime == null)
                return null;
            return UnitOfTime;
        }
        
        public async Task<UnitOfTime> Create(UnitOfTime UnitOfTime)
        {
            if (!await UnitOfTimeValidator.Create(UnitOfTime))
                return UnitOfTime;

            try
            {
                await UOW.UnitOfTimeRepository.Create(UnitOfTime);
                UnitOfTime = await UOW.UnitOfTimeRepository.Get(UnitOfTime.Id);
                Logging.CreateAuditLog(UnitOfTime, new { }, nameof(UnitOfTimeService));
                return UnitOfTime;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(UnitOfTimeService));
            }
            return null;
        }

        public async Task<UnitOfTime> Update(UnitOfTime UnitOfTime)
        {
            if (!await UnitOfTimeValidator.Update(UnitOfTime))
                return UnitOfTime;
            try
            {
                var oldData = await UOW.UnitOfTimeRepository.Get(UnitOfTime.Id);

                await UOW.UnitOfTimeRepository.Update(UnitOfTime);

                UnitOfTime = await UOW.UnitOfTimeRepository.Get(UnitOfTime.Id);
                Logging.CreateAuditLog(UnitOfTime, oldData, nameof(UnitOfTimeService));
                return UnitOfTime;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(UnitOfTimeService));
            }
            return null;
        }

        public async Task<UnitOfTime> Delete(UnitOfTime UnitOfTime)
        {
            if (!await UnitOfTimeValidator.Delete(UnitOfTime))
                return UnitOfTime;

            try
            {
                await UOW.UnitOfTimeRepository.Delete(UnitOfTime);
                Logging.CreateAuditLog(new { }, UnitOfTime, nameof(UnitOfTimeService));
                return UnitOfTime;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(UnitOfTimeService));
            }
            return null;
        }

        public async Task<List<UnitOfTime>> BulkDelete(List<UnitOfTime> UnitOfTimes)
        {
            if (!await UnitOfTimeValidator.BulkDelete(UnitOfTimes))
                return UnitOfTimes;

            try
            {
                await UOW.UnitOfTimeRepository.BulkDelete(UnitOfTimes);
                Logging.CreateAuditLog(new { }, UnitOfTimes, nameof(UnitOfTimeService));
                return UnitOfTimes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(UnitOfTimeService));
            }
            return null;

        }
        
        public async Task<List<UnitOfTime>> Import(List<UnitOfTime> UnitOfTimes)
        {
            if (!await UnitOfTimeValidator.Import(UnitOfTimes))
                return UnitOfTimes;
            try
            {
                await UOW.UnitOfTimeRepository.BulkMerge(UnitOfTimes);

                Logging.CreateAuditLog(UnitOfTimes, new { }, nameof(UnitOfTimeService));
                return UnitOfTimes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(UnitOfTimeService));
            }
            return null;
        }     
        
        public async Task<UnitOfTimeFilter> ToFilter(UnitOfTimeFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<UnitOfTimeFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                UnitOfTimeFilter subFilter = new UnitOfTimeFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StartAt))
                        subFilter.StartAt = FilterBuilder.Merge(subFilter.StartAt, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EndAt))
                        subFilter.EndAt = FilterBuilder.Merge(subFilter.EndAt, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private void Sync(List<UnitOfTime> UnitOfTimes)
        {
            
        }

    }
}
