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

namespace TrueCareer.Services.MNewsStatus
{
    public interface INewsStatusService :  IServiceScoped
    {
        Task<int> Count(NewsStatusFilter NewsStatusFilter);
        Task<List<NewsStatus>> List(NewsStatusFilter NewsStatusFilter);
    }

    public class NewsStatusService : BaseService, INewsStatusService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public NewsStatusService(
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
        public async Task<int> Count(NewsStatusFilter NewsStatusFilter)
        {
            try
            {
                int result = await UOW.NewsStatusRepository.Count(NewsStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NewsStatusService));
            }
            return 0;
        }

        public async Task<List<NewsStatus>> List(NewsStatusFilter NewsStatusFilter)
        {
            try
            {
                List<NewsStatus> NewsStatuses = await UOW.NewsStatusRepository.List(NewsStatusFilter);
                return NewsStatuses;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(NewsStatusService));
            }
            return null;
        }

    }
}
