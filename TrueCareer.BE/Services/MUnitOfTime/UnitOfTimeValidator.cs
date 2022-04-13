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

namespace TrueCareer.Services.MUnitOfTime
{
    public interface IUnitOfTimeValidator : IServiceScoped
    {
        Task Get(UnitOfTime UnitOfTime);
        Task<bool> Create(UnitOfTime UnitOfTime);
        Task<bool> Update(UnitOfTime UnitOfTime);
        Task<bool> Delete(UnitOfTime UnitOfTime);
        Task<bool> BulkDelete(List<UnitOfTime> UnitOfTimes);
        Task<bool> Import(List<UnitOfTime> UnitOfTimes);
    }

    public class UnitOfTimeValidator : IUnitOfTimeValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private UnitOfTimeMessage UnitOfTimeMessage;

        public UnitOfTimeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.UnitOfTimeMessage = new UnitOfTimeMessage();
        }

        public async Task Get(UnitOfTime UnitOfTime)
        {
        }

        public async Task<bool> Create(UnitOfTime UnitOfTime)
        {
            await ValidateCode(UnitOfTime);
            await ValidateName(UnitOfTime);
            await ValidateStartAt(UnitOfTime);
            await ValidateEndAt(UnitOfTime);
            return UnitOfTime.IsValidated;
        }

        public async Task<bool> Update(UnitOfTime UnitOfTime)
        {
            if (await ValidateId(UnitOfTime))
            {
                await ValidateCode(UnitOfTime);
                await ValidateName(UnitOfTime);
                await ValidateStartAt(UnitOfTime);
                await ValidateEndAt(UnitOfTime);
            }
            return UnitOfTime.IsValidated;
        }

        public async Task<bool> Delete(UnitOfTime UnitOfTime)
        {
            if (await ValidateId(UnitOfTime))
            {
            }
            return UnitOfTime.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<UnitOfTime> UnitOfTimes)
        {
            foreach (UnitOfTime UnitOfTime in UnitOfTimes)
            {
                await Delete(UnitOfTime);
            }
            return UnitOfTimes.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<UnitOfTime> UnitOfTimes)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(UnitOfTime UnitOfTime)
        {
            UnitOfTimeFilter UnitOfTimeFilter = new UnitOfTimeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = UnitOfTime.Id },
                Selects = UnitOfTimeSelect.Id
            };

            int count = await UOW.UnitOfTimeRepository.CountAll(UnitOfTimeFilter);
            if (count == 0)
                UnitOfTime.AddError(nameof(UnitOfTimeValidator), nameof(UnitOfTime.Id), UnitOfTimeMessage.Error.IdNotExisted, UnitOfTimeMessage);
            return UnitOfTime.IsValidated;
        }

        private async Task<bool> ValidateCode(UnitOfTime UnitOfTime)
        {
            if(string.IsNullOrEmpty(UnitOfTime.Code))
            {
                UnitOfTime.AddError(nameof(UnitOfTimeValidator), nameof(UnitOfTime.Code), UnitOfTimeMessage.Error.CodeEmpty, UnitOfTimeMessage);
            }
            else if(UnitOfTime.Code.Count() > 500)
            {
                UnitOfTime.AddError(nameof(UnitOfTimeValidator), nameof(UnitOfTime.Code), UnitOfTimeMessage.Error.CodeOverLength, UnitOfTimeMessage);
            }
            return UnitOfTime.IsValidated;
        }
        private async Task<bool> ValidateName(UnitOfTime UnitOfTime)
        {
            if(string.IsNullOrEmpty(UnitOfTime.Name))
            {
                UnitOfTime.AddError(nameof(UnitOfTimeValidator), nameof(UnitOfTime.Name), UnitOfTimeMessage.Error.NameEmpty, UnitOfTimeMessage);
            }
            else if(UnitOfTime.Name.Count() > 500)
            {
                UnitOfTime.AddError(nameof(UnitOfTimeValidator), nameof(UnitOfTime.Name), UnitOfTimeMessage.Error.NameOverLength, UnitOfTimeMessage);
            }
            return UnitOfTime.IsValidated;
        }
        private async Task<bool> ValidateStartAt(UnitOfTime UnitOfTime)
        {   
            return true;
        }
        private async Task<bool> ValidateEndAt(UnitOfTime UnitOfTime)
        {   
            return true;
        }
    }
}
