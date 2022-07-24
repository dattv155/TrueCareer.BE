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
using TrueCareer.Services.MMbtiPersonalType;

namespace TrueCareer.Services.MMbtiResult
{
    public interface IMbtiResultService :  IServiceScoped
    {
        Task<int> Count(MbtiResultFilter MbtiResultFilter);
        Task<List<MbtiResult>> List(MbtiResultFilter MbtiResultFilter);
        Task<MbtiResult> Get(long Id);
        Task<MbtiResult> Create(MbtiResult MbtiResult);
        Task<MbtiResult> Update(MbtiResult MbtiResult);
        Task<MbtiResult> Delete(MbtiResult MbtiResult);
        Task<MbtiResult> CalcResult(List<long> SingleTypeIds);
        Task<List<MbtiResult>> BulkDelete(List<MbtiResult> MbtiResults);
        Task<List<MbtiResult>> Import(List<MbtiResult> MbtiResults);
        Task<MbtiResultFilter> ToFilter(MbtiResultFilter MbtiResultFilter);
    }

    public class MbtiResultService : BaseService, IMbtiResultService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IMbtiResultValidator MbtiResultValidator;

        public MbtiResultService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IMbtiResultValidator MbtiResultValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.MbtiResultValidator = MbtiResultValidator;
        }
        public async Task<int> Count(MbtiResultFilter MbtiResultFilter)
        {
            try
            {
                int result = await UOW.MbtiResultRepository.Count(MbtiResultFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiResultService));
            }
            return 0;
        }

        public async Task<List<MbtiResult>> List(MbtiResultFilter MbtiResultFilter)
        {
            try
            {
                List<MbtiResult> MbtiResults = await UOW.MbtiResultRepository.List(MbtiResultFilter);
                return MbtiResults;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiResultService));
            }
            return null;
        }

        public async Task<MbtiResult> Get(long Id)
        {
            MbtiResult MbtiResult = await UOW.MbtiResultRepository.Get(Id);
            await MbtiResultValidator.Get(MbtiResult);
            if (MbtiResult == null)
                return null;
            return MbtiResult;
        }
        
        public async Task<MbtiResult> Create(MbtiResult MbtiResult)
        {
            if (!await MbtiResultValidator.Create(MbtiResult))
                return MbtiResult;

            try
            {
                await UOW.MbtiResultRepository.Create(MbtiResult);
                MbtiResult = await UOW.MbtiResultRepository.Get(MbtiResult.Id);
                Logging.CreateAuditLog(MbtiResult, new { }, nameof(MbtiResultService));
                return MbtiResult;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiResultService));
            }
            return null;
        }
        
        public async Task<MbtiResult> CalcResult(List<long> SingleTypeIds)
        {
            if (!await MbtiResultValidator.CalcResult(SingleTypeIds))
                return new MbtiResult();

            try
            {
                double E = 0, I = 0, S = 0, N = 0, T = 0, F = 0, J = 0, P = 0;
                string result = "";

                foreach (var Id in SingleTypeIds)
                {
                    if (Id == MbtiSingleTypeEnum.Extroverts.Id)
                    {
                        E++;
                    } else if (Id == MbtiSingleTypeEnum.Introverts.Id)
                    {
                        I++;
                    } else if (Id == MbtiSingleTypeEnum.Sensors.Id)
                    {
                        S++;
                    } else if (Id == MbtiSingleTypeEnum.Intuitives.Id)
                    {
                        N++;
                    } else if (Id == MbtiSingleTypeEnum.Thinkers.Id)
                    {
                        T++;
                    } else if (Id == MbtiSingleTypeEnum.Feelers.Id)
                    {
                        F++;
                    } else if (Id == MbtiSingleTypeEnum.Judgers.Id)
                    {
                        J++;
                    } else if (Id == MbtiSingleTypeEnum.Perceivers.Id)
                    {
                        P++;
                    }
                }

                double EPercent, IPercent, SPercent, NPercent, TPercent, FPercent, JPercent, PPercent;
                EPercent = Math.Floor(E / 10 * 100);
                IPercent = Math.Floor(I / 10 * 100);
                SPercent = Math.Floor(S / 20 * 100);
                NPercent = Math.Floor(N / 20 * 100);
                TPercent = Math.Floor(T / 20 * 100);
                FPercent = Math.Floor(F / 20 * 100);
                JPercent = Math.Floor(J / 20 * 100);
                PPercent = Math.Floor(P / 20 * 100);
                
                result += (EPercent >= IPercent) ? "E" : "I";
                result += (SPercent >= NPercent) ? "S" : "N";
                result += (TPercent >= FPercent) ? "T" : "F";
                result += (JPercent >= PPercent) ? "J" : "P";

                MbtiPersonalTypeFilter MbtiPersonalTypeFilter = new MbtiPersonalTypeFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = MbtiPersonalTypeSelect.ALL
                };
                List<MbtiPersonalType> MbtiPersonalTypes = await UOW.MbtiPersonalTypeRepository.List(MbtiPersonalTypeFilter);
                
                MbtiPersonalType MbtiPersonalType = MbtiPersonalTypes.Where(x => x.Code.ToString() == result).FirstOrDefault();

                MbtiResult MbtiResult = new MbtiResult();

                MbtiResult.UserId = CurrentContext.UserId;
                MbtiResult.MbtiPersonalTypeId = MbtiPersonalType == null ? 0 : MbtiPersonalType.Id;
                MbtiResult.MbtiPersonalType = MbtiPersonalType;
                await UOW.MbtiResultRepository.Create(MbtiResult);
                MbtiResult = await UOW.MbtiResultRepository.Get(MbtiResult.Id);
                Logging.CreateAuditLog(MbtiResult, new { }, nameof(MbtiResultService));
                return MbtiResult;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiResultService));
            }
            return null;
        }

        public async Task<MbtiResult> Update(MbtiResult MbtiResult)
        {
            if (!await MbtiResultValidator.Update(MbtiResult))
                return MbtiResult;
            try
            {
                var oldData = await UOW.MbtiResultRepository.Get(MbtiResult.Id);

                await UOW.MbtiResultRepository.Update(MbtiResult);

                MbtiResult = await UOW.MbtiResultRepository.Get(MbtiResult.Id);
                Logging.CreateAuditLog(MbtiResult, oldData, nameof(MbtiResultService));
                return MbtiResult;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiResultService));
            }
            return null;
        }

        public async Task<MbtiResult> Delete(MbtiResult MbtiResult)
        {
            if (!await MbtiResultValidator.Delete(MbtiResult))
                return MbtiResult;

            try
            {
                await UOW.MbtiResultRepository.Delete(MbtiResult);
                Logging.CreateAuditLog(new { }, MbtiResult, nameof(MbtiResultService));
                return MbtiResult;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiResultService));
            }
            return null;
        }

        public async Task<List<MbtiResult>> BulkDelete(List<MbtiResult> MbtiResults)
        {
            if (!await MbtiResultValidator.BulkDelete(MbtiResults))
                return MbtiResults;

            try
            {
                await UOW.MbtiResultRepository.BulkDelete(MbtiResults);
                Logging.CreateAuditLog(new { }, MbtiResults, nameof(MbtiResultService));
                return MbtiResults;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiResultService));
            }
            return null;

        }
        
        public async Task<List<MbtiResult>> Import(List<MbtiResult> MbtiResults)
        {
            if (!await MbtiResultValidator.Import(MbtiResults))
                return MbtiResults;
            try
            {
                await UOW.MbtiResultRepository.BulkMerge(MbtiResults);

                Logging.CreateAuditLog(MbtiResults, new { }, nameof(MbtiResultService));
                return MbtiResults;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MbtiResultService));
            }
            return null;
        }     
        
        public async Task<MbtiResultFilter> ToFilter(MbtiResultFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<MbtiResultFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                MbtiResultFilter subFilter = new MbtiResultFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UserId))
                        subFilter.UserId = FilterBuilder.Merge(subFilter.UserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.MbtiPersonalTypeId))
                        subFilter.MbtiPersonalTypeId = FilterBuilder.Merge(subFilter.MbtiPersonalTypeId, FilterPermissionDefinition.IdFilter);
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

        private void Sync(List<MbtiResult> MbtiResults)
        {
            List<MbtiPersonalType> MbtiPersonalTypes = new List<MbtiPersonalType>();
            List<AppUser> AppUsers = new List<AppUser>();
            MbtiPersonalTypes.AddRange(MbtiResults.Select(x => new MbtiPersonalType { Id = x.MbtiPersonalTypeId }));
            AppUsers.AddRange(MbtiResults.Select(x => new AppUser { Id = x.UserId }));
            
            MbtiPersonalTypes = MbtiPersonalTypes.Distinct().ToList();
            AppUsers = AppUsers.Distinct().ToList();
            RabbitManager.PublishList(MbtiPersonalTypes, RoutingKeyEnum.MbtiPersonalTypeUsed.Code);
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
        }

    }
}
