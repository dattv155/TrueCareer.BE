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

namespace TrueCareer.Services.MSchool
{
    public interface ISchoolValidator : IServiceScoped
    {
        Task Get(School School);
        Task<bool> Create(School School);
        Task<bool> Update(School School);
        Task<bool> Delete(School School);
        Task<bool> BulkDelete(List<School> Schools);
        Task<bool> Import(List<School> Schools);
    }

    public class SchoolValidator : ISchoolValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private SchoolMessage SchoolMessage;

        public SchoolValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.SchoolMessage = new SchoolMessage();
        }

        public async Task Get(School School)
        {
        }

        public async Task<bool> Create(School School)
        {
            await ValidateName(School);
            await ValidateDescription(School);
            return School.IsValidated;
        }

        public async Task<bool> Update(School School)
        {
            if (await ValidateId(School))
            {
                await ValidateName(School);
                await ValidateDescription(School);
            }
            return School.IsValidated;
        }

        public async Task<bool> Delete(School School)
        {
            if (await ValidateId(School))
            {
            }
            return School.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<School> Schools)
        {
            foreach (School School in Schools)
            {
                await Delete(School);
            }
            return Schools.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<School> Schools)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(School School)
        {
            SchoolFilter SchoolFilter = new SchoolFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = School.Id },
                Selects = SchoolSelect.Id
            };

            int count = await UOW.SchoolRepository.CountAll(SchoolFilter);
            if (count == 0)
                School.AddError(nameof(SchoolValidator), nameof(School.Id), SchoolMessage.Error.IdNotExisted, SchoolMessage);
            return School.IsValidated;
        }

        private async Task<bool> ValidateName(School School)
        {
            if(string.IsNullOrEmpty(School.Name))
            {
                School.AddError(nameof(SchoolValidator), nameof(School.Name), SchoolMessage.Error.NameEmpty, SchoolMessage);
            }
            else if(School.Name.Count() > 500)
            {
                School.AddError(nameof(SchoolValidator), nameof(School.Name), SchoolMessage.Error.NameOverLength, SchoolMessage);
            }
            return School.IsValidated;
        }
        private async Task<bool> ValidateDescription(School School)
        {
            if(string.IsNullOrEmpty(School.Description))
            {
                School.AddError(nameof(SchoolValidator), nameof(School.Description), SchoolMessage.Error.DescriptionEmpty, SchoolMessage);
            }
            else if(School.Description.Count() > 500)
            {
                School.AddError(nameof(SchoolValidator), nameof(School.Description), SchoolMessage.Error.DescriptionOverLength, SchoolMessage);
            }
            return School.IsValidated;
        }
    }
}
