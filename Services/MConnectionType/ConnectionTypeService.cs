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

namespace TrueCareer.Services.MConnectionType
{
    public interface IConnectionTypeService :  IServiceScoped
    {
        Task<int> Count(ConnectionTypeFilter ConnectionTypeFilter);
        Task<List<ConnectionType>> List(ConnectionTypeFilter ConnectionTypeFilter);
    }

    public class ConnectionTypeService : BaseService, IConnectionTypeService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public ConnectionTypeService(
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
        public async Task<int> Count(ConnectionTypeFilter ConnectionTypeFilter)
        {
            try
            {
                int result = await UOW.ConnectionTypeRepository.Count(ConnectionTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConnectionTypeService));
            }
            return 0;
        }

        public async Task<List<ConnectionType>> List(ConnectionTypeFilter ConnectionTypeFilter)
        {
            try
            {
                List<ConnectionType> ConnectionTypes = await UOW.ConnectionTypeRepository.List(ConnectionTypeFilter);
                return ConnectionTypes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConnectionTypeService));
            }
            return null;
        }

    }
}
