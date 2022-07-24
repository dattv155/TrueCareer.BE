using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer;
using TrueCareer.Common;
using TrueCareer.Enums;
using TrueCareer.Entities;
using TrueCareer.Repositories;

namespace TrueCareer.Services.MMbtiResult
{
    public interface IMbtiResultValidator : IServiceScoped
    {
        Task Get(MbtiResult MbtiResult);
        Task<bool> Create(MbtiResult MbtiResult);
        Task<bool> CalcResult(List<long> SingleTypeIds);
        Task<bool> Update(MbtiResult MbtiResult);
        Task<bool> Delete(MbtiResult MbtiResult);
        Task<bool> BulkDelete(List<MbtiResult> MbtiResults);
        Task<bool> Import(List<MbtiResult> MbtiResults);
    }

    public class MbtiResultValidator : IMbtiResultValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private MbtiResultMessage MbtiResultMessage;

        public MbtiResultValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.MbtiResultMessage = new MbtiResultMessage();
        }

        public async Task Get(MbtiResult MbtiResult)
        {
        }

        public async Task<bool> Create(MbtiResult MbtiResult)
        {
            await ValidateMbtiPersonalType(MbtiResult);
            await ValidateUser(MbtiResult);
            return MbtiResult.IsValidated;
        }
        
        public async Task<bool> CalcResult(List<long> SingleTypeIds)
        {
            return true;
        }

        public async Task<bool> Update(MbtiResult MbtiResult)
        {
            if (await ValidateId(MbtiResult))
            {
                await ValidateMbtiPersonalType(MbtiResult);
                await ValidateUser(MbtiResult);
            }
            return MbtiResult.IsValidated;
        }

        public async Task<bool> Delete(MbtiResult MbtiResult)
        {
            if (await ValidateId(MbtiResult))
            {
            }
            return MbtiResult.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<MbtiResult> MbtiResults)
        {
            foreach (MbtiResult MbtiResult in MbtiResults)
            {
                await Delete(MbtiResult);
            }
            return MbtiResults.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<MbtiResult> MbtiResults)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(MbtiResult MbtiResult)
        {
            MbtiResultFilter MbtiResultFilter = new MbtiResultFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = MbtiResult.Id },
                Selects = MbtiResultSelect.Id
            };

            int count = await UOW.MbtiResultRepository.CountAll(MbtiResultFilter);
            if (count == 0)
                MbtiResult.AddError(nameof(MbtiResultValidator), nameof(MbtiResult.Id), MbtiResultMessage.Error.IdNotExisted, MbtiResultMessage);
            return MbtiResult.IsValidated;
        }

        private async Task<bool> ValidateMbtiPersonalType(MbtiResult MbtiResult)
        {       
            if(MbtiResult.MbtiPersonalTypeId == 0)
            {
                MbtiResult.AddError(nameof(MbtiResultValidator), nameof(MbtiResult.MbtiPersonalType), MbtiResultMessage.Error.MbtiPersonalTypeEmpty, MbtiResultMessage);
            }
            else
            {
                if(!MbtiPersonalTypeEnum.MbtiPersonalTypeEnumList.Any(x => MbtiResult.MbtiPersonalTypeId == x.Id))
                {
                    MbtiResult.AddError(nameof(MbtiResultValidator), nameof(MbtiResult.MbtiPersonalType), MbtiResultMessage.Error.MbtiPersonalTypeNotExisted, MbtiResultMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateUser(MbtiResult MbtiResult)
        {       
            if(MbtiResult.UserId == 0)
            {
                MbtiResult.AddError(nameof(MbtiResultValidator), nameof(MbtiResult.User), MbtiResultMessage.Error.UserEmpty, MbtiResultMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  MbtiResult.UserId },
                });
                if(count == 0)
                {
                    MbtiResult.AddError(nameof(MbtiResultValidator), nameof(MbtiResult.User), MbtiResultMessage.Error.UserNotExisted, MbtiResultMessage);
                }
            }
            return true;
        }
    }
}
