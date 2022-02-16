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

namespace TrueCareer.Services.MTopic
{
    public interface ITopicService :  IServiceScoped
    {
        Task<int> Count(TopicFilter TopicFilter);
        Task<List<Topic>> List(TopicFilter TopicFilter);
        Task<Topic> Get(long Id);
        Task<Topic> Create(Topic Topic);
        Task<Topic> Update(Topic Topic);
        Task<Topic> Delete(Topic Topic);
        Task<List<Topic>> BulkDelete(List<Topic> Topics);
        Task<List<Topic>> Import(List<Topic> Topics);
        Task<TopicFilter> ToFilter(TopicFilter TopicFilter);
    }

    public class TopicService : BaseService, ITopicService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private ITopicValidator TopicValidator;

        public TopicService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            ITopicValidator TopicValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.TopicValidator = TopicValidator;
        }
        public async Task<int> Count(TopicFilter TopicFilter)
        {
            try
            {
                int result = await UOW.TopicRepository.Count(TopicFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(TopicService));
            }
            return 0;
        }

        public async Task<List<Topic>> List(TopicFilter TopicFilter)
        {
            try
            {
                List<Topic> Topics = await UOW.TopicRepository.List(TopicFilter);
                return Topics;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(TopicService));
            }
            return null;
        }

        public async Task<Topic> Get(long Id)
        {
            Topic Topic = await UOW.TopicRepository.Get(Id);
            await TopicValidator.Get(Topic);
            if (Topic == null)
                return null;
            return Topic;
        }
        
        public async Task<Topic> Create(Topic Topic)
        {
            if (!await TopicValidator.Create(Topic))
                return Topic;

            try
            {
                await UOW.TopicRepository.Create(Topic);
                Topic = await UOW.TopicRepository.Get(Topic.Id);
                Logging.CreateAuditLog(Topic, new { }, nameof(TopicService));
                return Topic;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(TopicService));
            }
            return null;
        }

        public async Task<Topic> Update(Topic Topic)
        {
            if (!await TopicValidator.Update(Topic))
                return Topic;
            try
            {
                var oldData = await UOW.TopicRepository.Get(Topic.Id);

                await UOW.TopicRepository.Update(Topic);

                Topic = await UOW.TopicRepository.Get(Topic.Id);
                Logging.CreateAuditLog(Topic, oldData, nameof(TopicService));
                return Topic;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(TopicService));
            }
            return null;
        }

        public async Task<Topic> Delete(Topic Topic)
        {
            if (!await TopicValidator.Delete(Topic))
                return Topic;

            try
            {
                await UOW.TopicRepository.Delete(Topic);
                Logging.CreateAuditLog(new { }, Topic, nameof(TopicService));
                return Topic;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(TopicService));
            }
            return null;
        }

        public async Task<List<Topic>> BulkDelete(List<Topic> Topics)
        {
            if (!await TopicValidator.BulkDelete(Topics))
                return Topics;

            try
            {
                await UOW.TopicRepository.BulkDelete(Topics);
                Logging.CreateAuditLog(new { }, Topics, nameof(TopicService));
                return Topics;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(TopicService));
            }
            return null;

        }
        
        public async Task<List<Topic>> Import(List<Topic> Topics)
        {
            if (!await TopicValidator.Import(Topics))
                return Topics;
            try
            {
                await UOW.TopicRepository.BulkMerge(Topics);

                Logging.CreateAuditLog(Topics, new { }, nameof(TopicService));
                return Topics;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(TopicService));
            }
            return null;
        }     
        
        public async Task<TopicFilter> ToFilter(TopicFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<TopicFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                TopicFilter subFilter = new TopicFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Title))
                        subFilter.Title = FilterBuilder.Merge(subFilter.Title, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = FilterBuilder.Merge(subFilter.Description, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Cost))
                        subFilter.Cost = FilterBuilder.Merge(subFilter.Cost, FilterPermissionDefinition.DecimalFilter);
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

        private void Sync(List<Topic> Topics)
        {
            
        }

    }
}
