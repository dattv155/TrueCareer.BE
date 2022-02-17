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

namespace TrueCareer.Services.MMentorReview
{
    public interface IMentorReviewValidator : IServiceScoped
    {
        Task Get(MentorReview MentorReview);
        Task<bool> Create(MentorReview MentorReview);
        Task<bool> Update(MentorReview MentorReview);
        Task<bool> Delete(MentorReview MentorReview);
        Task<bool> BulkDelete(List<MentorReview> MentorReviews);
        Task<bool> Import(List<MentorReview> MentorReviews);
    }

    public class MentorReviewValidator : IMentorReviewValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private MentorReviewMessage MentorReviewMessage;

        public MentorReviewValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.MentorReviewMessage = new MentorReviewMessage();
        }

        public async Task Get(MentorReview MentorReview)
        {
        }

        public async Task<bool> Create(MentorReview MentorReview)
        {
            await ValidateDescription(MentorReview);
            await ValidateContentReview(MentorReview);
            await ValidateStar(MentorReview);
            await ValidateTime(MentorReview);
            await ValidateCreator(MentorReview);
            await ValidateMentor(MentorReview);
            return MentorReview.IsValidated;
        }

        public async Task<bool> Update(MentorReview MentorReview)
        {
            if (await ValidateId(MentorReview))
            {
                await ValidateDescription(MentorReview);
                await ValidateContentReview(MentorReview);
                await ValidateStar(MentorReview);
                await ValidateTime(MentorReview);
                await ValidateCreator(MentorReview);
                await ValidateMentor(MentorReview);
            }
            return MentorReview.IsValidated;
        }

        public async Task<bool> Delete(MentorReview MentorReview)
        {
            if (await ValidateId(MentorReview))
            {
            }
            return MentorReview.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<MentorReview> MentorReviews)
        {
            foreach (MentorReview MentorReview in MentorReviews)
            {
                await Delete(MentorReview);
            }
            return MentorReviews.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<MentorReview> MentorReviews)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(MentorReview MentorReview)
        {
            MentorReviewFilter MentorReviewFilter = new MentorReviewFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = MentorReview.Id },
                Selects = MentorReviewSelect.Id
            };

            int count = await UOW.MentorReviewRepository.CountAll(MentorReviewFilter);
            if (count == 0)
                MentorReview.AddError(nameof(MentorReviewValidator), nameof(MentorReview.Id), MentorReviewMessage.Error.IdNotExisted, MentorReviewMessage);
            return MentorReview.IsValidated;
        }

        private async Task<bool> ValidateDescription(MentorReview MentorReview)
        {
            if(string.IsNullOrEmpty(MentorReview.Description))
            {
                MentorReview.AddError(nameof(MentorReviewValidator), nameof(MentorReview.Description), MentorReviewMessage.Error.DescriptionEmpty, MentorReviewMessage);
            }
            else if(MentorReview.Description.Count() > 500)
            {
                MentorReview.AddError(nameof(MentorReviewValidator), nameof(MentorReview.Description), MentorReviewMessage.Error.DescriptionOverLength, MentorReviewMessage);
            }
            return MentorReview.IsValidated;
        }
        private async Task<bool> ValidateContentReview(MentorReview MentorReview)
        {
            if(string.IsNullOrEmpty(MentorReview.ContentReview))
            {
                MentorReview.AddError(nameof(MentorReviewValidator), nameof(MentorReview.ContentReview), MentorReviewMessage.Error.ContentReviewEmpty, MentorReviewMessage);
            }
            else if(MentorReview.ContentReview.Count() > 500)
            {
                MentorReview.AddError(nameof(MentorReviewValidator), nameof(MentorReview.ContentReview), MentorReviewMessage.Error.ContentReviewOverLength, MentorReviewMessage);
            }
            return MentorReview.IsValidated;
        }
        private async Task<bool> ValidateStar(MentorReview MentorReview)
        {   
            return true;
        }
        private async Task<bool> ValidateTime(MentorReview MentorReview)
        {       
            if(MentorReview.Time <= new DateTime(2000, 1, 1))
            {
                MentorReview.AddError(nameof(MentorReviewValidator), nameof(MentorReview.Time), MentorReviewMessage.Error.TimeEmpty, MentorReviewMessage);
            }
            return true;
        }
        private async Task<bool> ValidateCreator(MentorReview MentorReview)
        {       
            if(MentorReview.CreatorId == 0)
            {
                MentorReview.AddError(nameof(MentorReviewValidator), nameof(MentorReview.Creator), MentorReviewMessage.Error.CreatorEmpty, MentorReviewMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  MentorReview.CreatorId },
                });
                if(count == 0)
                {
                    MentorReview.AddError(nameof(MentorReviewValidator), nameof(MentorReview.Creator), MentorReviewMessage.Error.CreatorNotExisted, MentorReviewMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateMentor(MentorReview MentorReview)
        {       
            if(MentorReview.MentorId == 0)
            {
                MentorReview.AddError(nameof(MentorReviewValidator), nameof(MentorReview.Mentor), MentorReviewMessage.Error.MentorEmpty, MentorReviewMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  MentorReview.MentorId },
                });
                if(count == 0)
                {
                    MentorReview.AddError(nameof(MentorReviewValidator), nameof(MentorReview.Mentor), MentorReviewMessage.Error.MentorNotExisted, MentorReviewMessage);
                }
            }
            return true;
        }
    }
}
