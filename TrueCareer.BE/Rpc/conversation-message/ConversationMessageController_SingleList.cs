using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueCareer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using TrueCareer.Entities;
using TrueCareer.Services.MConversationMessage;
using TrueCareer.Services.MConversation;
using TrueCareer.Services.MConversationType;
using TrueCareer.Services.MGlobalUser;

namespace TrueCareer.Rpc.conversation_message
{
    public partial class ConversationMessageController : RpcController
    {
        [Route(ConversationMessageRoute.SingleListConversation), HttpPost]
        public async Task<List<ConversationMessage_ConversationDTO>> SingleListConversation([FromBody] ConversationMessage_ConversationFilterDTO ConversationMessage_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationFilter ConversationFilter = new ConversationFilter();
            ConversationFilter.Skip = 0;
            ConversationFilter.Take = 20;
            ConversationFilter.OrderBy = ConversationOrder.Id;
            ConversationFilter.OrderType = OrderType.ASC;
            ConversationFilter.Selects = ConversationSelect.ALL;
            ConversationFilter.Id = ConversationMessage_ConversationFilterDTO.Id;
            ConversationFilter.Name = ConversationMessage_ConversationFilterDTO.Name;
            List<Conversation> Conversations = await ConversationService.List(ConversationFilter);
            List<ConversationMessage_ConversationDTO> ConversationMessage_ConversationDTOs = Conversations
                .Select(x => new ConversationMessage_ConversationDTO(x)).ToList();
            return ConversationMessage_ConversationDTOs;
        }
        [Route(ConversationMessageRoute.SingleListConversationType), HttpPost]
        public async Task<List<ConversationMessage_ConversationTypeDTO>> SingleListConversationType([FromBody] ConversationMessage_ConversationTypeFilterDTO ConversationMessage_ConversationTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationTypeFilter ConversationTypeFilter = new ConversationTypeFilter();
            ConversationTypeFilter.Skip = 0;
            ConversationTypeFilter.Take = int.MaxValue;
            ConversationTypeFilter.Take = 20;
            ConversationTypeFilter.OrderBy = ConversationTypeOrder.Id;
            ConversationTypeFilter.OrderType = OrderType.ASC;
            ConversationTypeFilter.Selects = ConversationTypeSelect.ALL;
            List<ConversationType> ConversationTypes = await ConversationTypeService.List(ConversationTypeFilter);
            List<ConversationMessage_ConversationTypeDTO> ConversationMessage_ConversationTypeDTOs = ConversationTypes
                .Select(x => new ConversationMessage_ConversationTypeDTO(x)).ToList();
            return ConversationMessage_ConversationTypeDTOs;
        }
        [Route(ConversationMessageRoute.SingleListGlobalUser), HttpPost]
        public async Task<List<ConversationMessage_GlobalUserDTO>> SingleListGlobalUser([FromBody] ConversationMessage_GlobalUserFilterDTO ConversationMessage_GlobalUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GlobalUserFilter GlobalUserFilter = new GlobalUserFilter();
            GlobalUserFilter.Skip = 0;
            GlobalUserFilter.Take = 20;
            GlobalUserFilter.OrderBy = GlobalUserOrder.DisplayName;
            GlobalUserFilter.OrderType = OrderType.ASC;
            GlobalUserFilter.Selects = GlobalUserSelect.ALL;
            GlobalUserFilter.GlobalUserTypeId = ConversationMessage_GlobalUserFilterDTO.GlobalUserTypeId;
            GlobalUserFilter.Username = ConversationMessage_GlobalUserFilterDTO.Username;
            GlobalUserFilter.DisplayName = ConversationMessage_GlobalUserFilterDTO.DisplayName;
            List<GlobalUser> GlobalUsers = await GlobalUserService.List(GlobalUserFilter);
            List<ConversationMessage_GlobalUserDTO> ConversationMessage_GlobalUserDTOs = GlobalUsers
                .Select(x => new ConversationMessage_GlobalUserDTO(x)).ToList();
            return ConversationMessage_GlobalUserDTOs;
        }
    }
}

