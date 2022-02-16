using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using TrueCareer.Entities;
using TrueCareer.Services.MConversationParticipant;
using TrueCareer.Services.MConversation;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.conversation_participant
{
    public partial class ConversationParticipantController 
    {
        [Route(ConversationParticipantRoute.FilterListConversation), HttpPost]
        public async Task<List<ConversationParticipant_ConversationDTO>> FilterListConversation([FromBody] ConversationParticipant_ConversationFilterDTO ConversationParticipant_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationFilter ConversationFilter = new ConversationFilter();
            ConversationFilter.Skip = 0;
            ConversationFilter.Take = 20;
            ConversationFilter.OrderBy = ConversationOrder.Id;
            ConversationFilter.OrderType = OrderType.ASC;
            ConversationFilter.Selects = ConversationSelect.ALL;
            ConversationFilter.Id = ConversationParticipant_ConversationFilterDTO.Id;
            ConversationFilter.LatestContent = ConversationParticipant_ConversationFilterDTO.LatestContent;
            ConversationFilter.LatestUserId = ConversationParticipant_ConversationFilterDTO.LatestUserId;
            ConversationFilter.Hash = ConversationParticipant_ConversationFilterDTO.Hash;

            List<Conversation> Conversations = await ConversationService.List(ConversationFilter);
            List<ConversationParticipant_ConversationDTO> ConversationParticipant_ConversationDTOs = Conversations
                .Select(x => new ConversationParticipant_ConversationDTO(x)).ToList();
            return ConversationParticipant_ConversationDTOs;
        }
        [Route(ConversationParticipantRoute.FilterListAppUser), HttpPost]
        public async Task<List<ConversationParticipant_AppUserDTO>> FilterListAppUser([FromBody] ConversationParticipant_AppUserFilterDTO ConversationParticipant_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ConversationParticipant_AppUserFilterDTO.Id;
            AppUserFilter.Username = ConversationParticipant_AppUserFilterDTO.Username;
            AppUserFilter.Email = ConversationParticipant_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ConversationParticipant_AppUserFilterDTO.Phone;
            AppUserFilter.Password = ConversationParticipant_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = ConversationParticipant_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = ConversationParticipant_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = ConversationParticipant_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = ConversationParticipant_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = ConversationParticipant_AppUserFilterDTO.CoverImage;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ConversationParticipant_AppUserDTO> ConversationParticipant_AppUserDTOs = AppUsers
                .Select(x => new ConversationParticipant_AppUserDTO(x)).ToList();
            return ConversationParticipant_AppUserDTOs;
        }
    }
}

