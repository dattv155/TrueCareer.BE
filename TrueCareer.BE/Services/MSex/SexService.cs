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

namespace TrueCareer.Services.MSex
{
    public interface ISexService :  IServiceScoped
    {
        Task<int> Count(SexFilter SexFilter);
        Task<List<Sex>> List(SexFilter SexFilter);
    }

    public class SexService : BaseService, ISexService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public SexService(
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
        public async Task<int> Count(SexFilter SexFilter)
        {
            try
            {
                int result = await UOW.SexRepository.Count(SexFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SexService));
            }
            return 0;
        }

        public async Task<List<Sex>> List(SexFilter SexFilter)
        {
            try
            {
                List<Sex> Sexes = await UOW.SexRepository.List(SexFilter);
                return Sexes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SexService));
            }
            return null;
        }

    }
}
