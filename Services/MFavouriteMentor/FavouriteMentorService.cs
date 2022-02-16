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

namespace TrueCareer.Services.MFavouriteMentor
{
    public interface IFavouriteMentorService :  IServiceScoped
    {
        Task<int> Count(FavouriteMentorFilter FavouriteMentorFilter);
        Task<List<FavouriteMentor>> List(FavouriteMentorFilter FavouriteMentorFilter);
        Task<FavouriteMentor> Get(long Id);
        Task<FavouriteMentor> Create(FavouriteMentor FavouriteMentor);
        Task<FavouriteMentor> Update(FavouriteMentor FavouriteMentor);
        Task<FavouriteMentor> Delete(FavouriteMentor FavouriteMentor);
        Task<List<FavouriteMentor>> BulkDelete(List<FavouriteMentor> FavouriteMentors);
        Task<List<FavouriteMentor>> Import(List<FavouriteMentor> FavouriteMentors);
        Task<FavouriteMentorFilter> ToFilter(FavouriteMentorFilter FavouriteMentorFilter);
    }

    public class FavouriteMentorService : BaseService, IFavouriteMentorService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IFavouriteMentorValidator FavouriteMentorValidator;

        public FavouriteMentorService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IFavouriteMentorValidator FavouriteMentorValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.FavouriteMentorValidator = FavouriteMentorValidator;
        }
        public async Task<int> Count(FavouriteMentorFilter FavouriteMentorFilter)
        {
            try
            {
                int result = await UOW.FavouriteMentorRepository.Count(FavouriteMentorFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteMentorService));
            }
            return 0;
        }

        public async Task<List<FavouriteMentor>> List(FavouriteMentorFilter FavouriteMentorFilter)
        {
            try
            {
                List<FavouriteMentor> FavouriteMentors = await UOW.FavouriteMentorRepository.List(FavouriteMentorFilter);
                return FavouriteMentors;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteMentorService));
            }
            return null;
        }

        public async Task<FavouriteMentor> Get(long Id)
        {
            FavouriteMentor FavouriteMentor = await UOW.FavouriteMentorRepository.Get(Id);
            await FavouriteMentorValidator.Get(FavouriteMentor);
            if (FavouriteMentor == null)
                return null;
            return FavouriteMentor;
        }
        
        public async Task<FavouriteMentor> Create(FavouriteMentor FavouriteMentor)
        {
            if (!await FavouriteMentorValidator.Create(FavouriteMentor))
                return FavouriteMentor;

            try
            {
                await UOW.FavouriteMentorRepository.Create(FavouriteMentor);
                FavouriteMentor = await UOW.FavouriteMentorRepository.Get(FavouriteMentor.Id);
                Logging.CreateAuditLog(FavouriteMentor, new { }, nameof(FavouriteMentorService));
                return FavouriteMentor;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteMentorService));
            }
            return null;
        }

        public async Task<FavouriteMentor> Update(FavouriteMentor FavouriteMentor)
        {
            if (!await FavouriteMentorValidator.Update(FavouriteMentor))
                return FavouriteMentor;
            try
            {
                var oldData = await UOW.FavouriteMentorRepository.Get(FavouriteMentor.Id);

                await UOW.FavouriteMentorRepository.Update(FavouriteMentor);

                FavouriteMentor = await UOW.FavouriteMentorRepository.Get(FavouriteMentor.Id);
                Logging.CreateAuditLog(FavouriteMentor, oldData, nameof(FavouriteMentorService));
                return FavouriteMentor;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteMentorService));
            }
            return null;
        }

        public async Task<FavouriteMentor> Delete(FavouriteMentor FavouriteMentor)
        {
            if (!await FavouriteMentorValidator.Delete(FavouriteMentor))
                return FavouriteMentor;

            try
            {
                await UOW.FavouriteMentorRepository.Delete(FavouriteMentor);
                Logging.CreateAuditLog(new { }, FavouriteMentor, nameof(FavouriteMentorService));
                return FavouriteMentor;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteMentorService));
            }
            return null;
        }

        public async Task<List<FavouriteMentor>> BulkDelete(List<FavouriteMentor> FavouriteMentors)
        {
            if (!await FavouriteMentorValidator.BulkDelete(FavouriteMentors))
                return FavouriteMentors;

            try
            {
                await UOW.FavouriteMentorRepository.BulkDelete(FavouriteMentors);
                Logging.CreateAuditLog(new { }, FavouriteMentors, nameof(FavouriteMentorService));
                return FavouriteMentors;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteMentorService));
            }
            return null;

        }
        
        public async Task<List<FavouriteMentor>> Import(List<FavouriteMentor> FavouriteMentors)
        {
            if (!await FavouriteMentorValidator.Import(FavouriteMentors))
                return FavouriteMentors;
            try
            {
                await UOW.FavouriteMentorRepository.BulkMerge(FavouriteMentors);

                Logging.CreateAuditLog(FavouriteMentors, new { }, nameof(FavouriteMentorService));
                return FavouriteMentors;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FavouriteMentorService));
            }
            return null;
        }     
        
        public async Task<FavouriteMentorFilter> ToFilter(FavouriteMentorFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<FavouriteMentorFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                FavouriteMentorFilter subFilter = new FavouriteMentorFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UserId))
                        subFilter.UserId = FilterBuilder.Merge(subFilter.UserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.MentorId))
                        subFilter.MentorId = FilterBuilder.Merge(subFilter.MentorId, FilterPermissionDefinition.IdFilter);
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

        private void Sync(List<FavouriteMentor> FavouriteMentors)
        {
            List<AppUser> AppUsers = new List<AppUser>();
            AppUsers.AddRange(FavouriteMentors.Select(x => new AppUser { Id = x.MentorId }));
            AppUsers.AddRange(FavouriteMentors.Select(x => new AppUser { Id = x.UserId }));
            
            AppUsers = AppUsers.Distinct().ToList();
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
        }

    }
}
