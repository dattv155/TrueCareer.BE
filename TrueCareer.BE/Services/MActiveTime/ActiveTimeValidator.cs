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

namespace TrueCareer.Services.MActiveTime
{
    public interface IActiveTimeValidator : IServiceScoped
    {
        Task Get(ActiveTime ActiveTime);
        Task<bool> Create(ActiveTime ActiveTime);
        Task<bool> Update(ActiveTime ActiveTime);
        Task<bool> Delete(ActiveTime ActiveTime);
        Task<bool> BulkDelete(List<ActiveTime> ActiveTimes);
        Task<bool> Import(List<ActiveTime> ActiveTimes);
    }

    public class ActiveTimeValidator : IActiveTimeValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private ActiveTimeMessage ActiveTimeMessage;

        public ActiveTimeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ActiveTimeMessage = new ActiveTimeMessage();
        }

        public async Task Get(ActiveTime ActiveTime)
        {
        }

        public async Task<bool> Create(ActiveTime ActiveTime)
        {
            await ValidateActiveDate(ActiveTime);

            await ValidateMentor(ActiveTime);
            return ActiveTime.IsValidated;
        }

        public async Task<bool> Update(ActiveTime ActiveTime)
        {
            if (await ValidateId(ActiveTime))
            {
                await ValidateActiveDate(ActiveTime);

                await ValidateMentor(ActiveTime);
            }
            return ActiveTime.IsValidated;
        }

        public async Task<bool> Delete(ActiveTime ActiveTime)
        {
            if (await ValidateId(ActiveTime))
            {
            }
            return ActiveTime.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ActiveTime> ActiveTimes)
        {
            foreach (ActiveTime ActiveTime in ActiveTimes)
            {
                await Delete(ActiveTime);
            }
            return ActiveTimes.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<ActiveTime> ActiveTimes)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(ActiveTime ActiveTime)
        {
            ActiveTimeFilter ActiveTimeFilter = new ActiveTimeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ActiveTime.Id },
                Selects = ActiveTimeSelect.Id
            };

            int count = await UOW.ActiveTimeRepository.CountAll(ActiveTimeFilter);
            if (count == 0)
                ActiveTime.AddError(nameof(ActiveTimeValidator), nameof(ActiveTime.Id), ActiveTimeMessage.Error.IdNotExisted, ActiveTimeMessage);
            return ActiveTime.IsValidated;
        }

        private async Task<bool> ValidateActiveDate(ActiveTime ActiveTime)
        {       
            if(ActiveTime.ActiveDate <= new DateTime(2000, 1, 1))
            {
                ActiveTime.AddError(nameof(ActiveTimeValidator), nameof(ActiveTime.ActiveDate), ActiveTimeMessage.Error.StartAtEmpty, ActiveTimeMessage);
            }
            return true;
        }

        private async Task<bool> ValidateMentor(ActiveTime ActiveTime)
        {       
            if(ActiveTime.MentorId == 0)
            {
                ActiveTime.AddError(nameof(ActiveTimeValidator), nameof(ActiveTime.Mentor), ActiveTimeMessage.Error.MentorEmpty, ActiveTimeMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  ActiveTime.MentorId },
                });
                if(count == 0)
                {
                    ActiveTime.AddError(nameof(ActiveTimeValidator), nameof(ActiveTime.Mentor), ActiveTimeMessage.Error.MentorNotExisted, ActiveTimeMessage);
                }
            }
            return true;
        }
    }
}
