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

namespace TrueCareer.Services.MConversationParticipant
{
    public interface IConversationParticipantService :  IServiceScoped
    {
        Task<int> Count(ConversationParticipantFilter ConversationParticipantFilter);
        Task<List<ConversationParticipant>> List(ConversationParticipantFilter ConversationParticipantFilter);
        Task<ConversationParticipant> Get(long Id);
        Task<ConversationParticipant> Create(ConversationParticipant ConversationParticipant);
        Task<ConversationParticipant> Update(ConversationParticipant ConversationParticipant);
        Task<ConversationParticipant> Delete(ConversationParticipant ConversationParticipant);
        Task<List<ConversationParticipant>> BulkDelete(List<ConversationParticipant> ConversationParticipants);
        Task<List<ConversationParticipant>> Import(List<ConversationParticipant> ConversationParticipants);
        Task<ConversationParticipantFilter> ToFilter(ConversationParticipantFilter ConversationParticipantFilter);
    }

    public class ConversationParticipantService : BaseService, IConversationParticipantService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IConversationParticipantValidator ConversationParticipantValidator;

        public ConversationParticipantService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IConversationParticipantValidator ConversationParticipantValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.ConversationParticipantValidator = ConversationParticipantValidator;
        }
        public async Task<int> Count(ConversationParticipantFilter ConversationParticipantFilter)
        {
            try
            {
                int result = await UOW.ConversationParticipantRepository.Count(ConversationParticipantFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationParticipantService));
            }
            return 0;
        }

        public async Task<List<ConversationParticipant>> List(ConversationParticipantFilter ConversationParticipantFilter)
        {
            try
            {
                List<ConversationParticipant> ConversationParticipants = await UOW.ConversationParticipantRepository.List(ConversationParticipantFilter);
                return ConversationParticipants;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationParticipantService));
            }
            return null;
        }

        public async Task<ConversationParticipant> Get(long Id)
        {
            ConversationParticipant ConversationParticipant = await UOW.ConversationParticipantRepository.Get(Id);
            await ConversationParticipantValidator.Get(ConversationParticipant);
            if (ConversationParticipant == null)
                return null;
            return ConversationParticipant;
        }
        
        public async Task<ConversationParticipant> Create(ConversationParticipant ConversationParticipant)
        {
            if (!await ConversationParticipantValidator.Create(ConversationParticipant))
                return ConversationParticipant;

            try
            {
                await UOW.ConversationParticipantRepository.Create(ConversationParticipant);
                ConversationParticipant = await UOW.ConversationParticipantRepository.Get(ConversationParticipant.Id);
                Logging.CreateAuditLog(ConversationParticipant, new { }, nameof(ConversationParticipantService));
                return ConversationParticipant;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationParticipantService));
            }
            return null;
        }

        public async Task<ConversationParticipant> Update(ConversationParticipant ConversationParticipant)
        {
            if (!await ConversationParticipantValidator.Update(ConversationParticipant))
                return ConversationParticipant;
            try
            {
                var oldData = await UOW.ConversationParticipantRepository.Get(ConversationParticipant.Id);

                await UOW.ConversationParticipantRepository.Update(ConversationParticipant);

                ConversationParticipant = await UOW.ConversationParticipantRepository.Get(ConversationParticipant.Id);
                Logging.CreateAuditLog(ConversationParticipant, oldData, nameof(ConversationParticipantService));
                return ConversationParticipant;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationParticipantService));
            }
            return null;
        }

        public async Task<ConversationParticipant> Delete(ConversationParticipant ConversationParticipant)
        {
            if (!await ConversationParticipantValidator.Delete(ConversationParticipant))
                return ConversationParticipant;

            try
            {
                await UOW.ConversationParticipantRepository.Delete(ConversationParticipant);
                Logging.CreateAuditLog(new { }, ConversationParticipant, nameof(ConversationParticipantService));
                return ConversationParticipant;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationParticipantService));
            }
            return null;
        }

        public async Task<List<ConversationParticipant>> BulkDelete(List<ConversationParticipant> ConversationParticipants)
        {
            if (!await ConversationParticipantValidator.BulkDelete(ConversationParticipants))
                return ConversationParticipants;

            try
            {
                await UOW.ConversationParticipantRepository.BulkDelete(ConversationParticipants);
                Logging.CreateAuditLog(new { }, ConversationParticipants, nameof(ConversationParticipantService));
                return ConversationParticipants;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationParticipantService));
            }
            return null;

        }
        
        public async Task<List<ConversationParticipant>> Import(List<ConversationParticipant> ConversationParticipants)
        {
            if (!await ConversationParticipantValidator.Import(ConversationParticipants))
                return ConversationParticipants;
            try
            {
                await UOW.ConversationParticipantRepository.BulkMerge(ConversationParticipants);

                Logging.CreateAuditLog(ConversationParticipants, new { }, nameof(ConversationParticipantService));
                return ConversationParticipants;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ConversationParticipantService));
            }
            return null;
        }     
        
        public async Task<ConversationParticipantFilter> ToFilter(ConversationParticipantFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ConversationParticipantFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ConversationParticipantFilter subFilter = new ConversationParticipantFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ConversationId))
                        subFilter.ConversationId = FilterBuilder.Merge(subFilter.ConversationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UserId))
                        subFilter.UserId = FilterBuilder.Merge(subFilter.UserId, FilterPermissionDefinition.IdFilter);
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

        private void Sync(List<ConversationParticipant> ConversationParticipants)
        {
            List<Conversation> Conversations = new List<Conversation>();
            List<AppUser> AppUsers = new List<AppUser>();
            Conversations.AddRange(ConversationParticipants.Select(x => new Conversation { Id = x.ConversationId }));
            AppUsers.AddRange(ConversationParticipants.Select(x => new AppUser { Id = x.UserId }));
            
            Conversations = Conversations.Distinct().ToList();
            AppUsers = AppUsers.Distinct().ToList();
            RabbitManager.PublishList(Conversations, RoutingKeyEnum.ConversationUsed.Code);
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed.Code);
        }

    }
}
