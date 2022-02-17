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

namespace TrueCareer.Services.MMentorConnection
{
    public interface IMentorConnectionService :  IServiceScoped
    {
        Task<int> Count(MentorConnectionFilter MentorConnectionFilter);
        Task<List<MentorConnection>> List(MentorConnectionFilter MentorConnectionFilter);
        Task<MentorConnection> Get(long Id);
        Task<MentorConnection> Create(MentorConnection MentorConnection);
        Task<MentorConnection> Update(MentorConnection MentorConnection);
        Task<MentorConnection> Delete(MentorConnection MentorConnection);
        Task<List<MentorConnection>> BulkDelete(List<MentorConnection> MentorConnections);
        Task<List<MentorConnection>> Import(List<MentorConnection> MentorConnections);
        Task<MentorConnectionFilter> ToFilter(MentorConnectionFilter MentorConnectionFilter);
    }

    public class MentorConnectionService : BaseService, IMentorConnectionService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IMentorConnectionValidator MentorConnectionValidator;

        public MentorConnectionService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IMentorConnectionValidator MentorConnectionValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.MentorConnectionValidator = MentorConnectionValidator;
        }
        public async Task<int> Count(MentorConnectionFilter MentorConnectionFilter)
        {
            try
            {
                int result = await UOW.MentorConnectionRepository.Count(MentorConnectionFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorConnectionService));
            }
            return 0;
        }

        public async Task<List<MentorConnection>> List(MentorConnectionFilter MentorConnectionFilter)
        {
            try
            {
                List<MentorConnection> MentorConnections = await UOW.MentorConnectionRepository.List(MentorConnectionFilter);
                return MentorConnections;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorConnectionService));
            }
            return null;
        }

        public async Task<MentorConnection> Get(long Id)
        {
            MentorConnection MentorConnection = await UOW.MentorConnectionRepository.Get(Id);
            await MentorConnectionValidator.Get(MentorConnection);
            if (MentorConnection == null)
                return null;
            return MentorConnection;
        }
        
        public async Task<MentorConnection> Create(MentorConnection MentorConnection)
        {
            if (!await MentorConnectionValidator.Create(MentorConnection))
                return MentorConnection;

            try
            {
                await UOW.MentorConnectionRepository.Create(MentorConnection);
                MentorConnection = await UOW.MentorConnectionRepository.Get(MentorConnection.Id);
                Logging.CreateAuditLog(MentorConnection, new { }, nameof(MentorConnectionService));
                return MentorConnection;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorConnectionService));
            }
            return null;
        }

        public async Task<MentorConnection> Update(MentorConnection MentorConnection)
        {
            if (!await MentorConnectionValidator.Update(MentorConnection))
                return MentorConnection;
            try
            {
                var oldData = await UOW.MentorConnectionRepository.Get(MentorConnection.Id);

                await UOW.MentorConnectionRepository.Update(MentorConnection);

                MentorConnection = await UOW.MentorConnectionRepository.Get(MentorConnection.Id);
                Logging.CreateAuditLog(MentorConnection, oldData, nameof(MentorConnectionService));
                return MentorConnection;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorConnectionService));
            }
            return null;
        }

        public async Task<MentorConnection> Delete(MentorConnection MentorConnection)
        {
            if (!await MentorConnectionValidator.Delete(MentorConnection))
                return MentorConnection;

            try
            {
                await UOW.MentorConnectionRepository.Delete(MentorConnection);
                Logging.CreateAuditLog(new { }, MentorConnection, nameof(MentorConnectionService));
                return MentorConnection;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorConnectionService));
            }
            return null;
        }

        public async Task<List<MentorConnection>> BulkDelete(List<MentorConnection> MentorConnections)
        {
            if (!await MentorConnectionValidator.BulkDelete(MentorConnections))
                return MentorConnections;

            try
            {
                await UOW.MentorConnectionRepository.BulkDelete(MentorConnections);
                Logging.CreateAuditLog(new { }, MentorConnections, nameof(MentorConnectionService));
                return MentorConnections;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorConnectionService));
            }
            return null;

        }
        
        public async Task<List<MentorConnection>> Import(List<MentorConnection> MentorConnections)
        {
            if (!await MentorConnectionValidator.Import(MentorConnections))
                return MentorConnections;
            try
            {
                await UOW.MentorConnectionRepository.BulkMerge(MentorConnections);

                Logging.CreateAuditLog(MentorConnections, new { }, nameof(MentorConnectionService));
                return MentorConnections;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorConnectionService));
            }
            return null;
        }     
        
        public async Task<MentorConnectionFilter> ToFilter(MentorConnectionFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<MentorConnectionFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                MentorConnectionFilter subFilter = new MentorConnectionFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.MentorId))
                        subFilter.MentorId = FilterBuilder.Merge(subFilter.MentorId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Url))
                        subFilter.Url = FilterBuilder.Merge(subFilter.Url, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ConnectionTypeId))
                        subFilter.ConnectionTypeId = FilterBuilder.Merge(subFilter.ConnectionTypeId, FilterPermissionDefinition.IdFilter);
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

        private void Sync(List<MentorConnection> MentorConnections)
        {
            List<ConnectionType> ConnectionTypes = new List<ConnectionType>();
            List<AppUser> AppUsers = new List<AppUser>();
            ConnectionTypes.AddRange(MentorConnections.Select(x => new ConnectionType { Id = x.ConnectionTypeId }));
            AppUsers.AddRange(MentorConnections.Select(x => new AppUser { Id = x.MentorId }));
            
            ConnectionTypes = ConnectionTypes.Distinct().ToList();
            AppUsers = AppUsers.Distinct().ToList();
            RabbitManager.PublishList(ConnectionTypes, RoutingKeyEnum.ConnectionTypeUsed.Code);
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
        }

    }
}
