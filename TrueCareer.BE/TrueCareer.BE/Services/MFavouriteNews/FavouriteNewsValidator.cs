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

namespace TrueCareer.Services.MFavouriteNews
{
    public interface IFavouriteNewsValidator : IServiceScoped
    {
        Task Get(FavouriteNews FavouriteNews);
        Task<bool> Create(FavouriteNews FavouriteNews);
        Task<bool> Update(FavouriteNews FavouriteNews);
        Task<bool> Delete(FavouriteNews FavouriteNews);
        Task<bool> BulkDelete(List<FavouriteNews> FavouriteNews);
        Task<bool> Import(List<FavouriteNews> FavouriteNews);
    }

    public class FavouriteNewsValidator : IFavouriteNewsValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private FavouriteNewsMessage FavouriteNewsMessage;

        public FavouriteNewsValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.FavouriteNewsMessage = new FavouriteNewsMessage();
        }

        public async Task Get(FavouriteNews FavouriteNews)
        {
        }

        public async Task<bool> Create(FavouriteNews FavouriteNews)
        {
            await ValidateNews(FavouriteNews);
            await ValidateUser(FavouriteNews);
            return FavouriteNews.IsValidated;
        }

        public async Task<bool> Update(FavouriteNews FavouriteNews)
        {
            if (await ValidateId(FavouriteNews))
            {
                await ValidateNews(FavouriteNews);
                await ValidateUser(FavouriteNews);
            }
            return FavouriteNews.IsValidated;
        }

        public async Task<bool> Delete(FavouriteNews FavouriteNews)
        {
            if (await ValidateId(FavouriteNews))
            {
            }
            return FavouriteNews.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<FavouriteNews> FavouriteNews)
        {
            foreach (FavouriteNews FavouriteNew in FavouriteNews)
            {
                await Delete(FavouriteNew);
            }
            return FavouriteNews.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<FavouriteNews> FavouriteNews)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(FavouriteNews FavouriteNews)
        {
            FavouriteNewsFilter FavouriteNewsFilter = new FavouriteNewsFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = FavouriteNews.Id },
                Selects = FavouriteNewsSelect.Id
            };

            int count = await UOW.FavouriteNewsRepository.CountAll(FavouriteNewsFilter);
            if (count == 0)
                FavouriteNews.AddError(nameof(FavouriteNewsValidator), nameof(FavouriteNews.Id), FavouriteNewsMessage.Error.IdNotExisted, FavouriteNewsMessage);
            return FavouriteNews.IsValidated;
        }

        private async Task<bool> ValidateNews(FavouriteNews FavouriteNews)
        {       
            if(FavouriteNews.NewsId == 0)
            {
                FavouriteNews.AddError(nameof(FavouriteNewsValidator), nameof(FavouriteNews.News), FavouriteNewsMessage.Error.NewsEmpty, FavouriteNewsMessage);
            }
            else
            {
                int count = await UOW.NewsRepository.CountAll(new NewsFilter
                {
                    Id = new IdFilter{ Equal =  FavouriteNews.NewsId },
                });
                if(count == 0)
                {
                    FavouriteNews.AddError(nameof(FavouriteNewsValidator), nameof(FavouriteNews.News), FavouriteNewsMessage.Error.NewsNotExisted, FavouriteNewsMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateUser(FavouriteNews FavouriteNews)
        {       
            if(FavouriteNews.UserId == 0)
            {
                FavouriteNews.AddError(nameof(FavouriteNewsValidator), nameof(FavouriteNews.User), FavouriteNewsMessage.Error.UserEmpty, FavouriteNewsMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  FavouriteNews.UserId },
                });
                if(count == 0)
                {
                    FavouriteNews.AddError(nameof(FavouriteNewsValidator), nameof(FavouriteNews.User), FavouriteNewsMessage.Error.UserNotExisted, FavouriteNewsMessage);
                }
            }
            return true;
        }
    }
}
