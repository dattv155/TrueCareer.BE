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

namespace TrueCareer.Services.MConnectionStatus
{
    public interface IConnectionStatusService :  IServiceScoped
    {
        Task<int> Count(ConnectionStatusFilter ConnectionStatusFilter);
        Task<List<ConnectionStatus>> List(ConnectionStatusFilter ConnectionStatusFilter);
    }

    public class ConnectionStatusService : BaseService, IConnectionStatusService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public ConnectionStatusService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
        }
        public async Task<int> Count(ConnectionStatusFilter ConnectionStatusFilter)
        {
            try
            {
                int result = await UOW.ConnectionStatusRepository.Count(ConnectionStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConnectionStatusService));
            }
            return 0;
        }

        public async Task<List<ConnectionStatus>> List(ConnectionStatusFilter ConnectionStatusFilter)
        {
            try
            {
                List<ConnectionStatus> ConnectionStatuses = await UOW.ConnectionStatusRepository.List(ConnectionStatusFilter);
                return ConnectionStatuses;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConnectionStatusService));
            }
            return null;
        }

    }
}
