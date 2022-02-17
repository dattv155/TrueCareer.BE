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

namespace TrueCareer.Services.MTopic
{
    public interface ITopicValidator : IServiceScoped
    {
        Task Get(Topic Topic);
        Task<bool> Create(Topic Topic);
        Task<bool> Update(Topic Topic);
        Task<bool> Delete(Topic Topic);
        Task<bool> BulkDelete(List<Topic> Topics);
        Task<bool> Import(List<Topic> Topics);
    }

    public class TopicValidator : ITopicValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private TopicMessage TopicMessage;

        public TopicValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.TopicMessage = new TopicMessage();
        }

        public async Task Get(Topic Topic)
        {
        }

        public async Task<bool> Create(Topic Topic)
        {
            await ValidateTitle(Topic);
            await ValidateDescription(Topic);
            await ValidateCost(Topic);
            return Topic.IsValidated;
        }

        public async Task<bool> Update(Topic Topic)
        {
            if (await ValidateId(Topic))
            {
                await ValidateTitle(Topic);
                await ValidateDescription(Topic);
                await ValidateCost(Topic);
            }
            return Topic.IsValidated;
        }

        public async Task<bool> Delete(Topic Topic)
        {
            if (await ValidateId(Topic))
            {
            }
            return Topic.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Topic> Topics)
        {
            foreach (Topic Topic in Topics)
            {
                await Delete(Topic);
            }
            return Topics.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Topic> Topics)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Topic Topic)
        {
            TopicFilter TopicFilter = new TopicFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Topic.Id },
                Selects = TopicSelect.Id
            };

            int count = await UOW.TopicRepository.CountAll(TopicFilter);
            if (count == 0)
                Topic.AddError(nameof(TopicValidator), nameof(Topic.Id), TopicMessage.Error.IdNotExisted, TopicMessage);
            return Topic.IsValidated;
        }

        private async Task<bool> ValidateTitle(Topic Topic)
        {
            if(string.IsNullOrEmpty(Topic.Title))
            {
                Topic.AddError(nameof(TopicValidator), nameof(Topic.Title), TopicMessage.Error.TitleEmpty, TopicMessage);
            }
            else if(Topic.Title.Count() > 500)
            {
                Topic.AddError(nameof(TopicValidator), nameof(Topic.Title), TopicMessage.Error.TitleOverLength, TopicMessage);
            }
            return Topic.IsValidated;
        }
        private async Task<bool> ValidateDescription(Topic Topic)
        {
            if(string.IsNullOrEmpty(Topic.Description))
            {
                Topic.AddError(nameof(TopicValidator), nameof(Topic.Description), TopicMessage.Error.DescriptionEmpty, TopicMessage);
            }
            else if(Topic.Description.Count() > 500)
            {
                Topic.AddError(nameof(TopicValidator), nameof(Topic.Description), TopicMessage.Error.DescriptionOverLength, TopicMessage);
            }
            return Topic.IsValidated;
        }
        private async Task<bool> ValidateCost(Topic Topic)
        {   
            return true;
        }
    }
}
