using TrueSight.Common;
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
using TrueCareer.Common;

namespace TrueCareer.Services.MGlobalUser
{
    public interface IGlobalUserService : IServiceScoped
    {
        Task<int> Count(GlobalUserFilter GlobalUserFilter);
        Task<List<GlobalUser>> List(GlobalUserFilter GlobalUserFilter);
        Task<GlobalUser> Get(long Id);
        Task<GlobalUser> Get(Guid RowId);
        Task<List<GlobalUser>> BulkMerge(List<GlobalUser> GlobalUsers);
        Task<bool> CreateToken(FirebaseToken FirebaseToken);
        Task<bool> DeleteToken(FirebaseToken FirebaseToken);
    }

    public class GlobalUserService : IGlobalUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public GlobalUserService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(GlobalUserFilter GlobalUserFilter)
        {
            try
            {
                int result = await UOW.GlobalUserRepository.Count(GlobalUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(GlobalUserService));
            }
            return 0;
        }

        public async Task<List<GlobalUser>> List(GlobalUserFilter GlobalUserFilter)
        {
            try
            {
                List<GlobalUser> GlobalUsers = await UOW.GlobalUserRepository.List(GlobalUserFilter);
                return GlobalUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(GlobalUserService));
            }
            return null;
        }

        public async Task<GlobalUser> Get(long Id)
        {
            GlobalUser GlobalUser = await UOW.GlobalUserRepository.Get(Id);
            if (GlobalUser == null)
                return null;
            return GlobalUser;
        }

        public async Task<GlobalUser> Get(Guid RowId)
        {
            GlobalUser GlobalUser = await UOW.GlobalUserRepository.Get(RowId);
            if (GlobalUser == null)
                return null;
            return GlobalUser;
        }

        public async Task<List<GlobalUser>> BulkMerge(List<GlobalUser> GlobalUsers)
        {
            try
            {
                await UOW.GlobalUserRepository.BulkMerge(GlobalUsers);
                return GlobalUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(GlobalUserService));
            }
            return null;
        }
        public async Task<bool> CreateToken(FirebaseToken FirebaseToken)
        {
            return await UOW.GlobalUserRepository.CreateToken(FirebaseToken);
        }

        public async Task<bool> DeleteToken(FirebaseToken FirebaseToken)
        {
            return await UOW.GlobalUserRepository.DeleteToken(FirebaseToken);
        }
    }
}
