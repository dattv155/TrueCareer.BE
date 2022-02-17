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

namespace TrueCareer.Services.MMajor
{
    public interface IMajorValidator : IServiceScoped
    {
        Task Get(Major Major);
        Task<bool> Create(Major Major);
        Task<bool> Update(Major Major);
        Task<bool> Delete(Major Major);
        Task<bool> BulkDelete(List<Major> Majors);
        Task<bool> Import(List<Major> Majors);
    }

    public class MajorValidator : IMajorValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private MajorMessage MajorMessage;

        public MajorValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.MajorMessage = new MajorMessage();
        }

        public async Task Get(Major Major)
        {
        }

        public async Task<bool> Create(Major Major)
        {
            await ValidateName(Major);
            await ValidateDescription(Major);
            return Major.IsValidated;
        }

        public async Task<bool> Update(Major Major)
        {
            if (await ValidateId(Major))
            {
                await ValidateName(Major);
                await ValidateDescription(Major);
            }
            return Major.IsValidated;
        }

        public async Task<bool> Delete(Major Major)
        {
            if (await ValidateId(Major))
            {
            }
            return Major.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Major> Majors)
        {
            foreach (Major Major in Majors)
            {
                await Delete(Major);
            }
            return Majors.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Major> Majors)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Major Major)
        {
            MajorFilter MajorFilter = new MajorFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Major.Id },
                Selects = MajorSelect.Id
            };

            int count = await UOW.MajorRepository.CountAll(MajorFilter);
            if (count == 0)
                Major.AddError(nameof(MajorValidator), nameof(Major.Id), MajorMessage.Error.IdNotExisted, MajorMessage);
            return Major.IsValidated;
        }

        private async Task<bool> ValidateName(Major Major)
        {
            if(string.IsNullOrEmpty(Major.Name))
            {
                Major.AddError(nameof(MajorValidator), nameof(Major.Name), MajorMessage.Error.NameEmpty, MajorMessage);
            }
            else if(Major.Name.Count() > 500)
            {
                Major.AddError(nameof(MajorValidator), nameof(Major.Name), MajorMessage.Error.NameOverLength, MajorMessage);
            }
            return Major.IsValidated;
        }
        private async Task<bool> ValidateDescription(Major Major)
        {
            if(string.IsNullOrEmpty(Major.Description))
            {
                Major.AddError(nameof(MajorValidator), nameof(Major.Description), MajorMessage.Error.DescriptionEmpty, MajorMessage);
            }
            else if(Major.Description.Count() > 500)
            {
                Major.AddError(nameof(MajorValidator), nameof(Major.Description), MajorMessage.Error.DescriptionOverLength, MajorMessage);
            }
            return Major.IsValidated;
        }
    }
}
