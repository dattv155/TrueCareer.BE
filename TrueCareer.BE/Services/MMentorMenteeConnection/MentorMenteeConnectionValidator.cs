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

namespace TrueCareer.Services.MMentorMenteeConnection
{
    public interface IMentorMenteeConnectionValidator : IServiceScoped
    {
        Task Get(MentorMenteeConnection MentorMenteeConnection);
        Task<bool> Create(MentorMenteeConnection MentorMenteeConnection);
        Task<bool> Update(MentorMenteeConnection MentorMenteeConnection);
        Task<bool> Delete(MentorMenteeConnection MentorMenteeConnection);
        Task<bool> BulkDelete(List<MentorMenteeConnection> MentorMenteeConnections);
        Task<bool> Import(List<MentorMenteeConnection> MentorMenteeConnections);
    }

    public class MentorMenteeConnectionValidator : IMentorMenteeConnectionValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private MentorMenteeConnectionMessage MentorMenteeConnectionMessage;

        public MentorMenteeConnectionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.MentorMenteeConnectionMessage = new MentorMenteeConnectionMessage();
        }

        public async Task Get(MentorMenteeConnection MentorMenteeConnection)
        {
        }

        public async Task<bool> Create(MentorMenteeConnection MentorMenteeConnection)
        {
            await ValidateFirstMessage(MentorMenteeConnection);


            await ValidateMentee(MentorMenteeConnection);
            await ValidateMentor(MentorMenteeConnection);
            return MentorMenteeConnection.IsValidated;
        }

        public async Task<bool> Update(MentorMenteeConnection MentorMenteeConnection)
        {
            if (await ValidateId(MentorMenteeConnection))
            {
                // await ValidateFirstMessage(MentorMenteeConnection);
                // await ValidateConnection(MentorMenteeConnection);
                // await ValidateConnectionStatus(MentorMenteeConnection);
                // await ValidateMentee(MentorMenteeConnection);
                // await ValidateMentor(MentorMenteeConnection);
            }
            return MentorMenteeConnection.IsValidated;
        }

        public async Task<bool> Delete(MentorMenteeConnection MentorMenteeConnection)
        {
            if (await ValidateId(MentorMenteeConnection))
            {
            }
            return MentorMenteeConnection.IsValidated;
        }

        public async Task<bool> BulkDelete(List<MentorMenteeConnection> MentorMenteeConnections)
        {
            foreach (MentorMenteeConnection MentorMenteeConnection in MentorMenteeConnections)
            {
                await Delete(MentorMenteeConnection);
            }
            return MentorMenteeConnections.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<MentorMenteeConnection> MentorMenteeConnections)
        {
            return true;
        }

        private async Task<bool> ValidateId(MentorMenteeConnection MentorMenteeConnection)
        {
            MentorMenteeConnectionFilter MentorMenteeConnectionFilter = new MentorMenteeConnectionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = MentorMenteeConnection.Id },
                Selects = MentorMenteeConnectionSelect.Id
            };

            int count = await UOW.MentorMenteeConnectionRepository.CountAll(MentorMenteeConnectionFilter);
            if (count == 0)
                MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.Id), MentorMenteeConnectionMessage.Error.IdNotExisted, MentorMenteeConnectionMessage);
            return MentorMenteeConnection.IsValidated;
        }

        private async Task<bool> ValidateFirstMessage(MentorMenteeConnection MentorMenteeConnection)
        {
            if (string.IsNullOrEmpty(MentorMenteeConnection.FirstMessage))
            {
                MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.FirstMessage), MentorMenteeConnectionMessage.Error.FirstMessageEmpty, MentorMenteeConnectionMessage);
            }
            else if (MentorMenteeConnection.FirstMessage.Count() > 500)
            {
                MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.FirstMessage), MentorMenteeConnectionMessage.Error.FirstMessageOverLength, MentorMenteeConnectionMessage);
            }
            return MentorMenteeConnection.IsValidated;
        }
        private async Task<bool> ValidateConnection(MentorMenteeConnection MentorMenteeConnection)
        {
            if (MentorMenteeConnection.ConnectionId == 0)
            {
                MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.Connection), MentorMenteeConnectionMessage.Error.ConnectionEmpty, MentorMenteeConnectionMessage);
            }
            else
            {
                int count = await UOW.MentorConnectionRepository.CountAll(new MentorConnectionFilter
                {
                    Id = new IdFilter { Equal = MentorMenteeConnection.ConnectionId },
                });
                if (count == 0)
                {
                    MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.Connection), MentorMenteeConnectionMessage.Error.ConnectionNotExisted, MentorMenteeConnectionMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateConnectionStatus(MentorMenteeConnection MentorMenteeConnection)
        {
            if (MentorMenteeConnection.ConnectionStatusId == 0)
            {
                MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.ConnectionStatus), MentorMenteeConnectionMessage.Error.ConnectionStatusEmpty, MentorMenteeConnectionMessage);
            }
            else
            {
                if (!ConnectionStatusEnum.ConnectionStatusEnumList.Any(x => MentorMenteeConnection.ConnectionStatusId == x.Id))
                {
                    MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.ConnectionStatus), MentorMenteeConnectionMessage.Error.ConnectionStatusNotExisted, MentorMenteeConnectionMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateMentee(MentorMenteeConnection MentorMenteeConnection)
        {
            if (MentorMenteeConnection.MenteeId == 0)
            {
                MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.Mentee), MentorMenteeConnectionMessage.Error.MenteeEmpty, MentorMenteeConnectionMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter { Equal = MentorMenteeConnection.MenteeId },
                });
                if (count == 0)
                {
                    MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.Mentee), MentorMenteeConnectionMessage.Error.MenteeNotExisted, MentorMenteeConnectionMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateMentor(MentorMenteeConnection MentorMenteeConnection)
        {
            if (MentorMenteeConnection.MentorId == 0)
            {
                MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.Mentor), MentorMenteeConnectionMessage.Error.MentorEmpty, MentorMenteeConnectionMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter { Equal = MentorMenteeConnection.MentorId },
                });
                if (count == 0)
                {
                    MentorMenteeConnection.AddError(nameof(MentorMenteeConnectionValidator), nameof(MentorMenteeConnection.Mentor), MentorMenteeConnectionMessage.Error.MentorNotExisted, MentorMenteeConnectionMessage);
                }
            }
            return true;
        }
    }
}
