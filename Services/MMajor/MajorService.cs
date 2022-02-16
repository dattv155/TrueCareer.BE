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

namespace TrueCareer.Services.MMajor
{
    public interface IMajorService :  IServiceScoped
    {
        Task<int> Count(MajorFilter MajorFilter);
        Task<List<Major>> List(MajorFilter MajorFilter);
        Task<Major> Get(long Id);
        Task<Major> Create(Major Major);
        Task<Major> Update(Major Major);
        Task<Major> Delete(Major Major);
        Task<List<Major>> BulkDelete(List<Major> Majors);
        Task<List<Major>> Import(List<Major> Majors);
        Task<MajorFilter> ToFilter(MajorFilter MajorFilter);
    }

    public class MajorService : BaseService, IMajorService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IMajorValidator MajorValidator;

        public MajorService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IMajorValidator MajorValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.MajorValidator = MajorValidator;
        }
        public async Task<int> Count(MajorFilter MajorFilter)
        {
            try
            {
                int result = await UOW.MajorRepository.Count(MajorFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MajorService));
            }
            return 0;
        }

        public async Task<List<Major>> List(MajorFilter MajorFilter)
        {
            try
            {
                List<Major> Majors = await UOW.MajorRepository.List(MajorFilter);
                return Majors;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MajorService));
            }
            return null;
        }

        public async Task<Major> Get(long Id)
        {
            Major Major = await UOW.MajorRepository.Get(Id);
            await MajorValidator.Get(Major);
            if (Major == null)
                return null;
            return Major;
        }
        
        public async Task<Major> Create(Major Major)
        {
            if (!await MajorValidator.Create(Major))
                return Major;

            try
            {
                await UOW.MajorRepository.Create(Major);
                Major = await UOW.MajorRepository.Get(Major.Id);
                Logging.CreateAuditLog(Major, new { }, nameof(MajorService));
                return Major;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MajorService));
            }
            return null;
        }

        public async Task<Major> Update(Major Major)
        {
            if (!await MajorValidator.Update(Major))
                return Major;
            try
            {
                var oldData = await UOW.MajorRepository.Get(Major.Id);

                await UOW.MajorRepository.Update(Major);

                Major = await UOW.MajorRepository.Get(Major.Id);
                Logging.CreateAuditLog(Major, oldData, nameof(MajorService));
                return Major;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MajorService));
            }
            return null;
        }

        public async Task<Major> Delete(Major Major)
        {
            if (!await MajorValidator.Delete(Major))
                return Major;

            try
            {
                await UOW.MajorRepository.Delete(Major);
                Logging.CreateAuditLog(new { }, Major, nameof(MajorService));
                return Major;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MajorService));
            }
            return null;
        }

        public async Task<List<Major>> BulkDelete(List<Major> Majors)
        {
            if (!await MajorValidator.BulkDelete(Majors))
                return Majors;

            try
            {
                await UOW.MajorRepository.BulkDelete(Majors);
                Logging.CreateAuditLog(new { }, Majors, nameof(MajorService));
                return Majors;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MajorService));
            }
            return null;

        }
        
        public async Task<List<Major>> Import(List<Major> Majors)
        {
            if (!await MajorValidator.Import(Majors))
                return Majors;
            try
            {
                await UOW.MajorRepository.BulkMerge(Majors);

                Logging.CreateAuditLog(Majors, new { }, nameof(MajorService));
                return Majors;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MajorService));
            }
            return null;
        }     
        
        public async Task<MajorFilter> ToFilter(MajorFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<MajorFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                MajorFilter subFilter = new MajorFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = FilterBuilder.Merge(subFilter.Description, FilterPermissionDefinition.StringFilter);
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

        private void Sync(List<Major> Majors)
        {
            
        }

    }
}
