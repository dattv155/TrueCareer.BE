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

namespace TrueCareer.Services.MFavouriteMentor
{
    public interface IFavouriteMentorValidator : IServiceScoped
    {
        Task Get(FavouriteMentor FavouriteMentor);
        Task<bool> Create(FavouriteMentor FavouriteMentor);
        Task<bool> Update(FavouriteMentor FavouriteMentor);
        Task<bool> Delete(FavouriteMentor FavouriteMentor);
        Task<bool> BulkDelete(List<FavouriteMentor> FavouriteMentors);
        Task<bool> Import(List<FavouriteMentor> FavouriteMentors);
    }

    public class FavouriteMentorValidator : IFavouriteMentorValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private FavouriteMentorMessage FavouriteMentorMessage;

        public FavouriteMentorValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.FavouriteMentorMessage = new FavouriteMentorMessage();
        }

        public async Task Get(FavouriteMentor FavouriteMentor)
        {
        }

        public async Task<bool> Create(FavouriteMentor FavouriteMentor)
        {
            await ValidateMentor(FavouriteMentor);
            await ValidateUser(FavouriteMentor);
            return FavouriteMentor.IsValidated;
        }

        public async Task<bool> Update(FavouriteMentor FavouriteMentor)
        {
            if (await ValidateId(FavouriteMentor))
            {
                await ValidateMentor(FavouriteMentor);
                await ValidateUser(FavouriteMentor);
            }
            return FavouriteMentor.IsValidated;
        }

        public async Task<bool> Delete(FavouriteMentor FavouriteMentor)
        {
            if (await ValidateId(FavouriteMentor))
            {
            }
            return FavouriteMentor.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<FavouriteMentor> FavouriteMentors)
        {
            foreach (FavouriteMentor FavouriteMentor in FavouriteMentors)
            {
                await Delete(FavouriteMentor);
            }
            return FavouriteMentors.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<FavouriteMentor> FavouriteMentors)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(FavouriteMentor FavouriteMentor)
        {
            FavouriteMentorFilter FavouriteMentorFilter = new FavouriteMentorFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = FavouriteMentor.Id },
                Selects = FavouriteMentorSelect.Id
            };

            int count = await UOW.FavouriteMentorRepository.CountAll(FavouriteMentorFilter);
            if (count == 0)
                FavouriteMentor.AddError(nameof(FavouriteMentorValidator), nameof(FavouriteMentor.Id), FavouriteMentorMessage.Error.IdNotExisted, FavouriteMentorMessage);
            return FavouriteMentor.IsValidated;
        }

        private async Task<bool> ValidateMentor(FavouriteMentor FavouriteMentor)
        {       
            if(FavouriteMentor.MentorId == 0)
            {
                FavouriteMentor.AddError(nameof(FavouriteMentorValidator), nameof(FavouriteMentor.Mentor), FavouriteMentorMessage.Error.MentorEmpty, FavouriteMentorMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  FavouriteMentor.MentorId },
                });
                if(count == 0)
                {
                    FavouriteMentor.AddError(nameof(FavouriteMentorValidator), nameof(FavouriteMentor.Mentor), FavouriteMentorMessage.Error.MentorNotExisted, FavouriteMentorMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateUser(FavouriteMentor FavouriteMentor)
        {       
            if(FavouriteMentor.UserId == 0)
            {
                FavouriteMentor.AddError(nameof(FavouriteMentorValidator), nameof(FavouriteMentor.User), FavouriteMentorMessage.Error.UserEmpty, FavouriteMentorMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  FavouriteMentor.UserId },
                });
                if(count == 0)
                {
                    FavouriteMentor.AddError(nameof(FavouriteMentorValidator), nameof(FavouriteMentor.User), FavouriteMentorMessage.Error.UserNotExisted, FavouriteMentorMessage);
                }
            }
            return true;
        }
    }
}
