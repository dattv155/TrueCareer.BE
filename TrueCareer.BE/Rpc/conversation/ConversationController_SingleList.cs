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
using TrueCareer.Services.MConversation;
using TrueCareer.Services.MConversationMessage;
using TrueCareer.Services.MConversationType;
using TrueCareer.Services.MGlobalUser;

namespace TrueCareer.Rpc.conversation
{
    public partial class ConversationController : RpcController
    {
        [Route(ConversationRoute.SingleListConversationType), HttpPost]
        public async Task<List<Conversation_ConversationTypeDTO>> SingleListConversationType([FromBody] Conversation_ConversationTypeFilterDTO Conversation_ConversationTypeFilterDTO)
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
            List<Conversation_ConversationTypeDTO> Conversation_ConversationTypeDTOs = ConversationTypes
                .Select(x => new Conversation_ConversationTypeDTO(x)).ToList();
            return Conversation_ConversationTypeDTOs;
        }
        [Route(ConversationRoute.SingleListGlobalUser), HttpPost]
        public async Task<List<Conversation_GlobalUserDTO>> SingleListGlobalUser([FromBody] Conversation_GlobalUserFilterDTO Conversation_GlobalUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GlobalUserFilter GlobalUserFilter = new GlobalUserFilter();
            GlobalUserFilter.Skip = 0;
            GlobalUserFilter.Take = 20;
            GlobalUserFilter.OrderBy = GlobalUserOrder.DisplayName;
            GlobalUserFilter.OrderType = OrderType.ASC;
            GlobalUserFilter.Selects = GlobalUserSelect.ALL;
            GlobalUserFilter.Id = Conversation_GlobalUserFilterDTO.Id;
            GlobalUserFilter.Username = Conversation_GlobalUserFilterDTO.Username;
            GlobalUserFilter.DisplayName = Conversation_GlobalUserFilterDTO.DisplayName;
            GlobalUserFilter.GlobalUserTypeId = Conversation_GlobalUserFilterDTO.GlobalUserTypeId;
            GlobalUserFilter.RowId = Conversation_GlobalUserFilterDTO.RowId;
            List<GlobalUser> GlobalUsers = await GlobalUserService.List(GlobalUserFilter);
            List<Conversation_GlobalUserDTO> Conversation_GlobalUserDTOs = GlobalUsers
                .Select(x => new Conversation_GlobalUserDTO(x)).ToList();
            return Conversation_GlobalUserDTOs;
        }
     
    }
}

