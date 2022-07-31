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

namespace TrueCareer.Services.MMentorMenteeConnection
{
    public interface IMentorMenteeConnectionService : IServiceScoped
    {
        Task<int> Count(MentorMenteeConnectionFilter MentorMenteeConnectionFilter);
        Task<List<MentorMenteeConnection>> List(MentorMenteeConnectionFilter MentorMenteeConnectionFilter);
        Task<MentorMenteeConnection> Get(long Id);
        Task<MentorMenteeConnection> Create(MentorMenteeConnection MentorMenteeConnection);
        Task<MentorMenteeConnection> Update(MentorMenteeConnection MentorMenteeConnection);
        Task<MentorMenteeConnection> Delete(MentorMenteeConnection MentorMenteeConnection);
        Task<List<MentorMenteeConnection>> BulkDelete(List<MentorMenteeConnection> MentorMenteeConnections);
        Task<List<MentorMenteeConnection>> Import(List<MentorMenteeConnection> MentorMenteeConnections);
        Task<MentorMenteeConnectionFilter> ToFilter(MentorMenteeConnectionFilter MentorMenteeConnectionFilter);
    }

    public class MentorMenteeConnectionService : BaseService, IMentorMenteeConnectionService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        private IMentorMenteeConnectionValidator MentorMenteeConnectionValidator;

        public MentorMenteeConnectionService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IMentorMenteeConnectionValidator MentorMenteeConnectionValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;

            this.MentorMenteeConnectionValidator = MentorMenteeConnectionValidator;
        }
        public async Task<int> Count(MentorMenteeConnectionFilter MentorMenteeConnectionFilter)
        {
            try
            {
                int result = await UOW.MentorMenteeConnectionRepository.Count(MentorMenteeConnectionFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorMenteeConnectionService));
            }
            return 0;
        }

        public async Task<List<MentorMenteeConnection>> List(MentorMenteeConnectionFilter MentorMenteeConnectionFilter)
        {
            try
            {
                List<MentorMenteeConnection> MentorMenteeConnections = await UOW.MentorMenteeConnectionRepository.List(MentorMenteeConnectionFilter);
                return MentorMenteeConnections;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorMenteeConnectionService));
            }
            return null;
        }

        public async Task<MentorMenteeConnection> Get(long Id)
        {
            MentorMenteeConnection MentorMenteeConnection = await UOW.MentorMenteeConnectionRepository.Get(Id);
            await MentorMenteeConnectionValidator.Get(MentorMenteeConnection);
            if (MentorMenteeConnection == null)
                return null;
            return MentorMenteeConnection;
        }

        public async Task<MentorMenteeConnection> Create(MentorMenteeConnection MentorMenteeConnection)
        {
            if (!await MentorMenteeConnectionValidator.Create(MentorMenteeConnection))
                return MentorMenteeConnection;

            try
            {
                await UOW.MentorMenteeConnectionRepository.Create(MentorMenteeConnection);
                MentorMenteeConnection = await UOW.MentorMenteeConnectionRepository.Get(MentorMenteeConnection.Id);
                Logging.CreateAuditLog(MentorMenteeConnection, new { }, nameof(MentorMenteeConnectionService));
                return MentorMenteeConnection;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorMenteeConnectionService));
            }
            return null;
        }

        public async Task<MentorMenteeConnection> Update(MentorMenteeConnection MentorMenteeConnection)
        {
            if (!await MentorMenteeConnectionValidator.Update(MentorMenteeConnection))
                return MentorMenteeConnection;
            try
            {
                var oldData = await UOW.MentorMenteeConnectionRepository.Get(MentorMenteeConnection.Id);

                await UOW.MentorMenteeConnectionRepository.Update(MentorMenteeConnection);

                MentorMenteeConnection = await UOW.MentorMenteeConnectionRepository.Get(MentorMenteeConnection.Id);
                Logging.CreateAuditLog(MentorMenteeConnection, oldData, nameof(MentorMenteeConnectionService));
                return MentorMenteeConnection;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorMenteeConnectionService));
            }
            return null;
        }

        public async Task<MentorMenteeConnection> Delete(MentorMenteeConnection MentorMenteeConnection)
        {
            if (!await MentorMenteeConnectionValidator.Delete(MentorMenteeConnection))
                return MentorMenteeConnection;

            try
            {
                await UOW.MentorMenteeConnectionRepository.Delete(MentorMenteeConnection);
                Logging.CreateAuditLog(new { }, MentorMenteeConnection, nameof(MentorMenteeConnectionService));
                return MentorMenteeConnection;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorMenteeConnectionService));
            }
            return null;
        }

        public async Task<List<MentorMenteeConnection>> BulkDelete(List<MentorMenteeConnection> MentorMenteeConnections)
        {
            if (!await MentorMenteeConnectionValidator.BulkDelete(MentorMenteeConnections))
                return MentorMenteeConnections;

            try
            {
                await UOW.MentorMenteeConnectionRepository.BulkDelete(MentorMenteeConnections);
                Logging.CreateAuditLog(new { }, MentorMenteeConnections, nameof(MentorMenteeConnectionService));
                return MentorMenteeConnections;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorMenteeConnectionService));
            }
            return null;

        }

        public async Task<List<MentorMenteeConnection>> Import(List<MentorMenteeConnection> MentorMenteeConnections)
        {
            if (!await MentorMenteeConnectionValidator.Import(MentorMenteeConnections))
                return MentorMenteeConnections;
            try
            {
                await UOW.MentorMenteeConnectionRepository.BulkMerge(MentorMenteeConnections);

                Logging.CreateAuditLog(MentorMenteeConnections, new { }, nameof(MentorMenteeConnectionService));
                return MentorMenteeConnections;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MentorMenteeConnectionService));
            }
            return null;
        }

        public async Task<MentorMenteeConnectionFilter> ToFilter(MentorMenteeConnectionFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<MentorMenteeConnectionFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                MentorMenteeConnectionFilter subFilter = new MentorMenteeConnectionFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.MentorId))
                        subFilter.MentorId = FilterBuilder.Merge(subFilter.MentorId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.MenteeId))
                        subFilter.MenteeId = FilterBuilder.Merge(subFilter.MenteeId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ConnectionId))
                        subFilter.ConnectionId = FilterBuilder.Merge(subFilter.ConnectionId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.FirstMessage))
                        subFilter.FirstMessage = FilterBuilder.Merge(subFilter.FirstMessage, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ConnectionStatusId))
                        subFilter.ConnectionStatusId = FilterBuilder.Merge(subFilter.ConnectionStatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ActiveTimeId))
                        subFilter.ActiveTimeId = FilterBuilder.Merge(subFilter.ActiveTimeId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
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

        private void Sync(List<MentorMenteeConnection> MentorMenteeConnections)
        {

        }

    }
}
