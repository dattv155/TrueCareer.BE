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

namespace TrueCareer.Services.MMbtiSingleType
{
    public interface IMbtiSingleTypeService :  IServiceScoped
    {
        Task<int> Count(MbtiSingleTypeFilter MbtiSingleTypeFilter);
        Task<List<MbtiSingleType>> List(MbtiSingleTypeFilter MbtiSingleTypeFilter);
    }

    public class MbtiSingleTypeService : BaseService, IMbtiSingleTypeService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public MbtiSingleTypeService(
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
        public async Task<int> Count(MbtiSingleTypeFilter MbtiSingleTypeFilter)
        {
            try
            {
                int result = await UOW.MbtiSingleTypeRepository.Count(MbtiSingleTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiSingleTypeService));
            }
            return 0;
        }

        public async Task<List<MbtiSingleType>> List(MbtiSingleTypeFilter MbtiSingleTypeFilter)
        {
            try
            {
                List<MbtiSingleType> MbtiSingleTypes = await UOW.MbtiSingleTypeRepository.List(MbtiSingleTypeFilter);
                return MbtiSingleTypes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiSingleTypeService));
            }
            return null;
        }

    }
}
