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

namespace TrueCareer.Services.MActiveTime
{
    public interface IActiveTimeService :  IServiceScoped
    {
        Task<int> Count(ActiveTimeFilter ActiveTimeFilter);
        Task<List<ActiveTime>> List(ActiveTimeFilter ActiveTimeFilter);
        Task<ActiveTime> Get(long Id);
        Task<ActiveTime> Create(ActiveTime ActiveTime);
        Task<ActiveTime> Update(ActiveTime ActiveTime);
        Task<ActiveTime> Delete(ActiveTime ActiveTime);
        Task<List<ActiveTime>> BulkDelete(List<ActiveTime> ActiveTimes);
        Task<List<ActiveTime>> Import(List<ActiveTime> ActiveTimes);
        Task<ActiveTimeFilter> ToFilter(ActiveTimeFilter ActiveTimeFilter);
    }

    public class ActiveTimeService : BaseService, IActiveTimeService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IActiveTimeValidator ActiveTimeValidator;

        public ActiveTimeService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IActiveTimeValidator ActiveTimeValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.ActiveTimeValidator = ActiveTimeValidator;
        }
        public async Task<int> Count(ActiveTimeFilter ActiveTimeFilter)
        {
            try
            {
                int result = await UOW.ActiveTimeRepository.Count(ActiveTimeFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ActiveTimeService));
            }
            return 0;
        }

        public async Task<List<ActiveTime>> List(ActiveTimeFilter ActiveTimeFilter)
        {
            try
            {
                List<ActiveTime> ActiveTimes = await UOW.ActiveTimeRepository.List(ActiveTimeFilter);
                return ActiveTimes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ActiveTimeService));
            }
            return null;
        }

        public async Task<ActiveTime> Get(long Id)
        {
            ActiveTime ActiveTime = await UOW.ActiveTimeRepository.Get(Id);
            await ActiveTimeValidator.Get(ActiveTime);
            if (ActiveTime == null)
                return null;
            return ActiveTime;
        }
        
        public async Task<ActiveTime> Create(ActiveTime ActiveTime)
        {
            if (!await ActiveTimeValidator.Create(ActiveTime))
                return ActiveTime;

            try
            {
                await UOW.ActiveTimeRepository.Create(ActiveTime);
                ActiveTime = await UOW.ActiveTimeRepository.Get(ActiveTime.Id);
                Logging.CreateAuditLog(ActiveTime, new { }, nameof(ActiveTimeService));
                return ActiveTime;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ActiveTimeService));
            }
            return null;
        }

        public async Task<ActiveTime> Update(ActiveTime ActiveTime)
        {
            if (!await ActiveTimeValidator.Update(ActiveTime))
                return ActiveTime;
            try
            {
                var oldData = await UOW.ActiveTimeRepository.Get(ActiveTime.Id);

                await UOW.ActiveTimeRepository.Update(ActiveTime);

                ActiveTime = await UOW.ActiveTimeRepository.Get(ActiveTime.Id);
                Logging.CreateAuditLog(ActiveTime, oldData, nameof(ActiveTimeService));
                return ActiveTime;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ActiveTimeService));
            }
            return null;
        }

        public async Task<ActiveTime> Delete(ActiveTime ActiveTime)
        {
            if (!await ActiveTimeValidator.Delete(ActiveTime))
                return ActiveTime;

            try
            {
                await UOW.ActiveTimeRepository.Delete(ActiveTime);
                Logging.CreateAuditLog(new { }, ActiveTime, nameof(ActiveTimeService));
                return ActiveTime;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ActiveTimeService));
            }
            return null;
        }

        public async Task<List<ActiveTime>> BulkDelete(List<ActiveTime> ActiveTimes)
        {
            if (!await ActiveTimeValidator.BulkDelete(ActiveTimes))
                return ActiveTimes;

            try
            {
                await UOW.ActiveTimeRepository.BulkDelete(ActiveTimes);
                Logging.CreateAuditLog(new { }, ActiveTimes, nameof(ActiveTimeService));
                return ActiveTimes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ActiveTimeService));
            }
            return null;

        }
        
        public async Task<List<ActiveTime>> Import(List<ActiveTime> ActiveTimes)
        {
            if (!await ActiveTimeValidator.Import(ActiveTimes))
                return ActiveTimes;
            try
            {
                await UOW.ActiveTimeRepository.BulkMerge(ActiveTimes);

                Logging.CreateAuditLog(ActiveTimes, new { }, nameof(ActiveTimeService));
                return ActiveTimes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ActiveTimeService));
            }
            return null;
        }     
        
        public async Task<ActiveTimeFilter> ToFilter(ActiveTimeFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ActiveTimeFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ActiveTimeFilter subFilter = new ActiveTimeFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StartAt))
                        subFilter.StartAt = FilterBuilder.Merge(subFilter.StartAt, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EndAt))
                        subFilter.EndAt = FilterBuilder.Merge(subFilter.EndAt, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.MentorId))
                        subFilter.MentorId = FilterBuilder.Merge(subFilter.MentorId, FilterPermissionDefinition.IdFilter);
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

        private void Sync(List<ActiveTime> ActiveTimes)
        {
            List<AppUser> AppUsers = new List<AppUser>();
            AppUsers.AddRange(ActiveTimes.Select(x => new AppUser { Id = x.MentorId }));
            
            AppUsers = AppUsers.Distinct().ToList();
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
        }

    }
}
