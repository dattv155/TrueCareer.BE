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

namespace TrueCareer.Services.MInformationType
{
    public interface IInformationTypeService :  IServiceScoped
    {
        Task<int> Count(InformationTypeFilter InformationTypeFilter);
        Task<List<InformationType>> List(InformationTypeFilter InformationTypeFilter);
    }

    public class InformationTypeService : BaseService, IInformationTypeService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public InformationTypeService(
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
        public async Task<int> Count(InformationTypeFilter InformationTypeFilter)
        {
            try
            {
                int result = await UOW.InformationTypeRepository.Count(InformationTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(InformationTypeService));
            }
            return 0;
        }

        public async Task<List<InformationType>> List(InformationTypeFilter InformationTypeFilter)
        {
            try
            {
                List<InformationType> InformationTypes = await UOW.InformationTypeRepository.List(InformationTypeFilter);
                return InformationTypes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(InformationTypeService));
            }
            return null;
        }

    }
}
