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

namespace TrueCareer.Services.MNews
{
    public interface INewsValidator : IServiceScoped
    {
        Task Get(News News);
        Task<bool> Create(News News);
        Task<bool> Update(News News);
        Task<bool> Delete(News News);
        Task<bool> BulkDelete(List<News> News);
        Task<bool> Import(List<News> News);
    }

    public class NewsValidator : INewsValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private NewsMessage NewsMessage;

        public NewsValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.NewsMessage = new NewsMessage();
        }

        public async Task Get(News News)
        {
        }

        public async Task<bool> Create(News News)
        {
            await ValidateNewsContent(News);
            await ValidateLikeCounting(News);
            await ValidateWatchCounting(News);
            await ValidateCreator(News);
            await ValidateNewsStatus(News);
            return News.IsValidated;
        }

        public async Task<bool> Update(News News)
        {
            if (await ValidateId(News))
            {
                await ValidateNewsContent(News);
                await ValidateLikeCounting(News);
                await ValidateWatchCounting(News);
                await ValidateCreator(News);
                await ValidateNewsStatus(News);
            }
            return News.IsValidated;
        }

        public async Task<bool> Delete(News News)
        {
            if (await ValidateId(News))
            {
            }
            return News.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<News> News)
        {
            foreach (News News in News)
            {
                await Delete(News);
            }
            return News.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<News> News)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(News News)
        {
            NewsFilter NewsFilter = new NewsFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = News.Id },
                Selects = NewsSelect.Id
            };

            int count = await UOW.NewsRepository.CountAll(NewsFilter);
            if (count == 0)
                News.AddError(nameof(NewsValidator), nameof(News.Id), NewsMessage.Error.IdNotExisted, NewsMessage);
            return News.IsValidated;
        }

        private async Task<bool> ValidateNewsContent(News News)
        {
            if(string.IsNullOrEmpty(News.NewsContent))
            {
                News.AddError(nameof(NewsValidator), nameof(News.NewsContent), NewsMessage.Error.NewsContentEmpty, NewsMessage);
            }
            else if(News.NewsContent.Count() > 500)
            {
                News.AddError(nameof(NewsValidator), nameof(News.NewsContent), NewsMessage.Error.NewsContentOverLength, NewsMessage);
            }
            return News.IsValidated;
        }
        private async Task<bool> ValidateLikeCounting(News News)
        {   
            return true;
        }
        private async Task<bool> ValidateWatchCounting(News News)
        {   
            return true;
        }
        private async Task<bool> ValidateCreator(News News)
        {       
            if(News.CreatorId == 0)
            {
                News.AddError(nameof(NewsValidator), nameof(News.Creator), NewsMessage.Error.CreatorEmpty, NewsMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  News.CreatorId },
                });
                if(count == 0)
                {
                    News.AddError(nameof(NewsValidator), nameof(News.Creator), NewsMessage.Error.CreatorNotExisted, NewsMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateNewsStatus(News News)
        {       
            if(News.NewsStatusId == 0)
            {
                News.AddError(nameof(NewsValidator), nameof(News.NewsStatus), NewsMessage.Error.NewsStatusEmpty, NewsMessage);
            }
            else
            {
                if(!NewsStatusEnum.NewsStatusEnumList.Any(x => News.NewsStatusId == x.Id))
                {
                    News.AddError(nameof(NewsValidator), nameof(News.NewsStatus), NewsMessage.Error.NewsStatusNotExisted, NewsMessage);
                }
            }
            return true;
        }
    }
}
