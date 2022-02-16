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

namespace TrueCareer.Services.MMbtiPersonalType
{
    public interface IMbtiPersonalTypeService :  IServiceScoped
    {
        Task<int> Count(MbtiPersonalTypeFilter MbtiPersonalTypeFilter);
        Task<List<MbtiPersonalType>> List(MbtiPersonalTypeFilter MbtiPersonalTypeFilter);
    }

    public class MbtiPersonalTypeService : BaseService, IMbtiPersonalTypeService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public MbtiPersonalTypeService(
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
        public async Task<int> Count(MbtiPersonalTypeFilter MbtiPersonalTypeFilter)
        {
            try
            {
                int result = await UOW.MbtiPersonalTypeRepository.Count(MbtiPersonalTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiPersonalTypeService));
            }
            return 0;
        }

        public async Task<List<MbtiPersonalType>> List(MbtiPersonalTypeFilter MbtiPersonalTypeFilter)
        {
            try
            {
                List<MbtiPersonalType> MbtiPersonalTypes = await UOW.MbtiPersonalTypeRepository.List(MbtiPersonalTypeFilter);
                return MbtiPersonalTypes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiPersonalTypeService));
            }
            return null;
        }

    }
}
