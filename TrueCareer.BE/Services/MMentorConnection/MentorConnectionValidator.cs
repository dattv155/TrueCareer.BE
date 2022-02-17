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

namespace TrueCareer.Services.MMentorConnection
{
    public interface IMentorConnectionValidator : IServiceScoped
    {
        Task Get(MentorConnection MentorConnection);
        Task<bool> Create(MentorConnection MentorConnection);
        Task<bool> Update(MentorConnection MentorConnection);
        Task<bool> Delete(MentorConnection MentorConnection);
        Task<bool> BulkDelete(List<MentorConnection> MentorConnections);
        Task<bool> Import(List<MentorConnection> MentorConnections);
    }

    public class MentorConnectionValidator : IMentorConnectionValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private MentorConnectionMessage MentorConnectionMessage;

        public MentorConnectionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.MentorConnectionMessage = new MentorConnectionMessage();
        }

        public async Task Get(MentorConnection MentorConnection)
        {
        }

        public async Task<bool> Create(MentorConnection MentorConnection)
        {
            await ValidateUrl(MentorConnection);
            await ValidateConnectionType(MentorConnection);
            await ValidateMentor(MentorConnection);
            return MentorConnection.IsValidated;
        }

        public async Task<bool> Update(MentorConnection MentorConnection)
        {
            if (await ValidateId(MentorConnection))
            {
                await ValidateUrl(MentorConnection);
                await ValidateConnectionType(MentorConnection);
                await ValidateMentor(MentorConnection);
            }
            return MentorConnection.IsValidated;
        }

        public async Task<bool> Delete(MentorConnection MentorConnection)
        {
            if (await ValidateId(MentorConnection))
            {
            }
            return MentorConnection.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<MentorConnection> MentorConnections)
        {
            foreach (MentorConnection MentorConnection in MentorConnections)
            {
                await Delete(MentorConnection);
            }
            return MentorConnections.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<MentorConnection> MentorConnections)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(MentorConnection MentorConnection)
        {
            MentorConnectionFilter MentorConnectionFilter = new MentorConnectionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = MentorConnection.Id },
                Selects = MentorConnectionSelect.Id
            };

            int count = await UOW.MentorConnectionRepository.CountAll(MentorConnectionFilter);
            if (count == 0)
                MentorConnection.AddError(nameof(MentorConnectionValidator), nameof(MentorConnection.Id), MentorConnectionMessage.Error.IdNotExisted, MentorConnectionMessage);
            return MentorConnection.IsValidated;
        }

        private async Task<bool> ValidateUrl(MentorConnection MentorConnection)
        {
            if(string.IsNullOrEmpty(MentorConnection.Url))
            {
                MentorConnection.AddError(nameof(MentorConnectionValidator), nameof(MentorConnection.Url), MentorConnectionMessage.Error.UrlEmpty, MentorConnectionMessage);
            }
            else if(MentorConnection.Url.Count() > 500)
            {
                MentorConnection.AddError(nameof(MentorConnectionValidator), nameof(MentorConnection.Url), MentorConnectionMessage.Error.UrlOverLength, MentorConnectionMessage);
            }
            return MentorConnection.IsValidated;
        }
        private async Task<bool> ValidateConnectionType(MentorConnection MentorConnection)
        {       
            if(MentorConnection.ConnectionTypeId == 0)
            {
                MentorConnection.AddError(nameof(MentorConnectionValidator), nameof(MentorConnection.ConnectionType), MentorConnectionMessage.Error.ConnectionTypeEmpty, MentorConnectionMessage);
            }
            else
            {
                if(!ConnectionTypeEnum.ConnectionTypeEnumList.Any(x => MentorConnection.ConnectionTypeId == x.Id))
                {
                    MentorConnection.AddError(nameof(MentorConnectionValidator), nameof(MentorConnection.ConnectionType), MentorConnectionMessage.Error.ConnectionTypeNotExisted, MentorConnectionMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateMentor(MentorConnection MentorConnection)
        {       
            if(MentorConnection.MentorId == 0)
            {
                MentorConnection.AddError(nameof(MentorConnectionValidator), nameof(MentorConnection.Mentor), MentorConnectionMessage.Error.MentorEmpty, MentorConnectionMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  MentorConnection.MentorId },
                });
                if(count == 0)
                {
                    MentorConnection.AddError(nameof(MentorConnectionValidator), nameof(MentorConnection.Mentor), MentorConnectionMessage.Error.MentorNotExisted, MentorConnectionMessage);
                }
            }
            return true;
        }
    }
}
