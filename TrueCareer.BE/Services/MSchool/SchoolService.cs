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

namespace TrueCareer.Services.MSchool
{
    public interface ISchoolService :  IServiceScoped
    {
        Task<int> Count(SchoolFilter SchoolFilter);
        Task<List<School>> List(SchoolFilter SchoolFilter);
        Task<School> Get(long Id);
        Task<School> Create(School School);
        Task<School> Update(School School);
        Task<School> Delete(School School);
        Task<List<School>> BulkDelete(List<School> Schools);
        Task<List<School>> Import(List<School> Schools);
        Task<SchoolFilter> ToFilter(SchoolFilter SchoolFilter);
    }

    public class SchoolService : BaseService, ISchoolService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private ISchoolValidator SchoolValidator;

        public SchoolService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            ISchoolValidator SchoolValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.SchoolValidator = SchoolValidator;
        }
        public async Task<int> Count(SchoolFilter SchoolFilter)
        {
            try
            {
                int result = await UOW.SchoolRepository.Count(SchoolFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SchoolService));
            }
            return 0;
        }

        public async Task<List<School>> List(SchoolFilter SchoolFilter)
        {
            try
            {
                List<School> Schools = await UOW.SchoolRepository.List(SchoolFilter);
                return Schools;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SchoolService));
            }
            return null;
        }

        public async Task<School> Get(long Id)
        {
            School School = await UOW.SchoolRepository.Get(Id);
            await SchoolValidator.Get(School);
            if (School == null)
                return null;
            return School;
        }
        
        public async Task<School> Create(School School)
        {
            if (!await SchoolValidator.Create(School))
                return School;

            try
            {
                await UOW.SchoolRepository.Create(School);
                School = await UOW.SchoolRepository.Get(School.Id);
                Logging.CreateAuditLog(School, new { }, nameof(SchoolService));
                return School;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SchoolService));
            }
            return null;
        }

        public async Task<School> Update(School School)
        {
            if (!await SchoolValidator.Update(School))
                return School;
            try
            {
                var oldData = await UOW.SchoolRepository.Get(School.Id);

                await UOW.SchoolRepository.Update(School);

                School = await UOW.SchoolRepository.Get(School.Id);
                Logging.CreateAuditLog(School, oldData, nameof(SchoolService));
                return School;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SchoolService));
            }
            return null;
        }

        public async Task<School> Delete(School School)
        {
            if (!await SchoolValidator.Delete(School))
                return School;

            try
            {
                await UOW.SchoolRepository.Delete(School);
                Logging.CreateAuditLog(new { }, School, nameof(SchoolService));
                return School;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SchoolService));
            }
            return null;
        }

        public async Task<List<School>> BulkDelete(List<School> Schools)
        {
            if (!await SchoolValidator.BulkDelete(Schools))
                return Schools;

            try
            {
                await UOW.SchoolRepository.BulkDelete(Schools);
                Logging.CreateAuditLog(new { }, Schools, nameof(SchoolService));
                return Schools;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SchoolService));
            }
            return null;

        }
        
        public async Task<List<School>> Import(List<School> Schools)
        {
            if (!await SchoolValidator.Import(Schools))
                return Schools;
            try
            {
                await UOW.SchoolRepository.BulkMerge(Schools);

                Logging.CreateAuditLog(Schools, new { }, nameof(SchoolService));
                return Schools;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SchoolService));
            }
            return null;
        }     
        
        public async Task<SchoolFilter> ToFilter(SchoolFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<SchoolFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                SchoolFilter subFilter = new SchoolFilter();
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Rating))
                        subFilter.Rating = FilterBuilder.Merge(subFilter.Rating, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CompleteTime))
                        subFilter.CompleteTime = FilterBuilder.Merge(subFilter.CompleteTime, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StudentCount))
                        subFilter.StudentCount = FilterBuilder.Merge(subFilter.StudentCount, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PhoneNumber))
                        subFilter.PhoneNumber = FilterBuilder.Merge(subFilter.PhoneNumber, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Address))
                        subFilter.Address = FilterBuilder.Merge(subFilter.Address, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SchoolImage))
                        subFilter.SchoolImage = FilterBuilder.Merge(subFilter.SchoolImage, FilterPermissionDefinition.StringFilter);
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

        private void Sync(List<School> Schools)
        {
            
        }

    }
}
