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

namespace TrueCareer.Services.MInformation
{
    public interface IInformationService :  IServiceScoped
    {
        Task<int> Count(InformationFilter InformationFilter);
        Task<List<Information>> List(InformationFilter InformationFilter);
        Task<Information> Get(long Id);
        Task<Information> Create(Information Information);
        Task<Information> Update(Information Information);
        Task<Information> Delete(Information Information);
        Task<List<Information>> BulkDelete(List<Information> Information);
        Task<List<Information>> Import(List<Information> Information);
        Task<InformationFilter> ToFilter(InformationFilter InformationFilter);
    }

    public class InformationService : BaseService, IInformationService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IInformationValidator InformationValidator;

        public InformationService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IInformationValidator InformationValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.InformationValidator = InformationValidator;
        }
        public async Task<int> Count(InformationFilter InformationFilter)
        {
            try
            {
                int result = await UOW.InformationRepository.Count(InformationFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(InformationService));
            }
            return 0;
        }

        public async Task<List<Information>> List(InformationFilter InformationFilter)
        {
            try
            {
                List<Information> Information = await UOW.InformationRepository.List(InformationFilter);
                return Information;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(InformationService));
            }
            return null;
        }

        public async Task<Information> Get(long Id)
        {
            Information Information = await UOW.InformationRepository.Get(Id);
            await InformationValidator.Get(Information);
            if (Information == null)
                return null;
            return Information;
        }
        
        public async Task<Information> Create(Information Information)
        {
            if (!await InformationValidator.Create(Information))
                return Information;

            try
            {
                await UOW.InformationRepository.Create(Information);
                Information = await UOW.InformationRepository.Get(Information.Id);
                Logging.CreateAuditLog(Information, new { }, nameof(InformationService));
                return Information;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(InformationService));
            }
            return null;
        }

        public async Task<Information> Update(Information Information)
        {
            if (!await InformationValidator.Update(Information))
                return Information;
            try
            {
                var oldData = await UOW.InformationRepository.Get(Information.Id);

                await UOW.InformationRepository.Update(Information);

                Information = await UOW.InformationRepository.Get(Information.Id);
                Logging.CreateAuditLog(Information, oldData, nameof(InformationService));
                return Information;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(InformationService));
            }
            return null;
        }

        public async Task<Information> Delete(Information Information)
        {
            if (!await InformationValidator.Delete(Information))
                return Information;

            try
            {
                await UOW.InformationRepository.Delete(Information);
                Logging.CreateAuditLog(new { }, Information, nameof(InformationService));
                return Information;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(InformationService));
            }
            return null;
        }

        public async Task<List<Information>> BulkDelete(List<Information> Information)
        {
            if (!await InformationValidator.BulkDelete(Information))
                return Information;

            try
            {
                await UOW.InformationRepository.BulkDelete(Information);
                Logging.CreateAuditLog(new { }, Information, nameof(InformationService));
                return Information;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(InformationService));
            }
            return null;

        }
        
        public async Task<List<Information>> Import(List<Information> Information)
        {
            if (!await InformationValidator.Import(Information))
                return Information;
            try
            {
                await UOW.InformationRepository.BulkMerge(Information);

                Logging.CreateAuditLog(Information, new { }, nameof(InformationService));
                return Information;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(InformationService));
            }
            return null;
        }     
        
        public async Task<InformationFilter> ToFilter(InformationFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<InformationFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                InformationFilter subFilter = new InformationFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.InformationTypeId))
                        subFilter.InformationTypeId = FilterBuilder.Merge(subFilter.InformationTypeId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = FilterBuilder.Merge(subFilter.Description, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StartAt))
                        subFilter.StartAt = FilterBuilder.Merge(subFilter.StartAt, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Role))
                        subFilter.Role = FilterBuilder.Merge(subFilter.Role, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Image))
                        subFilter.Image = FilterBuilder.Merge(subFilter.Image, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TopicId))
                        subFilter.TopicId = FilterBuilder.Merge(subFilter.TopicId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UserId))
                        subFilter.UserId = FilterBuilder.Merge(subFilter.UserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EndAt))
                        subFilter.EndAt = FilterBuilder.Merge(subFilter.EndAt, FilterPermissionDefinition.DateFilter);
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

        private void Sync(List<Information> Information)
        {
            List<InformationType> InformationTypes = new List<InformationType>();
            List<Topic> Topics = new List<Topic>();
            List<AppUser> AppUsers = new List<AppUser>();
            InformationTypes.AddRange(Information.Select(x => new InformationType { Id = x.InformationTypeId }));
            Topics.AddRange(Information.Select(x => new Topic { Id = x.TopicId }));
            AppUsers.AddRange(Information.Select(x => new AppUser { Id = x.UserId }));
            
            InformationTypes = InformationTypes.Distinct().ToList();
            Topics = Topics.Distinct().ToList();
            AppUsers = AppUsers.Distinct().ToList();
            RabbitManager.PublishList(InformationTypes, RoutingKeyEnum.InformationTypeUsed.Code);
            RabbitManager.PublishList(Topics, RoutingKeyEnum.TopicUsed.Code);
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
        }

    }
}
