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
using TrueCareer.Services.MMessage;
using TrueCareer.Services.MConversation;

namespace TrueCareer.Rpc.message
{
    public partial class MessageController 
    {
        [Route(MessageRoute.SingleListConversation), HttpPost]
        public async Task<List<Message_ConversationDTO>> SingleListConversation([FromBody] Message_ConversationFilterDTO Message_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationFilter ConversationFilter = new ConversationFilter();
            ConversationFilter.Skip = 0;
            ConversationFilter.Take = 20;
            ConversationFilter.OrderBy = ConversationOrder.Id;
            ConversationFilter.OrderType = OrderType.ASC;
            ConversationFilter.Selects = ConversationSelect.ALL;
            ConversationFilter.Id = Message_ConversationFilterDTO.Id;
            ConversationFilter.LatestContent = Message_ConversationFilterDTO.LatestContent;
            ConversationFilter.LatestUserId = Message_ConversationFilterDTO.LatestUserId;
            ConversationFilter.Hash = Message_ConversationFilterDTO.Hash;
            List<Conversation> Conversations = await ConversationService.List(ConversationFilter);
            List<Message_ConversationDTO> Message_ConversationDTOs = Conversations
                .Select(x => new Message_ConversationDTO(x)).ToList();
            return Message_ConversationDTOs;
        }
    }
}

