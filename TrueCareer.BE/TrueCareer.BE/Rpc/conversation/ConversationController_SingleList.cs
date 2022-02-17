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
using TrueCareer.Services.MConversation;
using TrueCareer.Services.MMessage;

namespace TrueCareer.Rpc.conversation
{
    public partial class ConversationController 
    {
        [Route(ConversationRoute.SingleListMessage), HttpPost]
        public async Task<List<Conversation_MessageDTO>> SingleListMessage([FromBody] Conversation_MessageFilterDTO Conversation_MessageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MessageFilter MessageFilter = new MessageFilter();
            MessageFilter.Skip = 0;
            MessageFilter.Take = 20;
            MessageFilter.OrderBy = MessageOrder.Id;
            MessageFilter.OrderType = OrderType.ASC;
            MessageFilter.Selects = MessageSelect.ALL;
            MessageFilter.Id = Conversation_MessageFilterDTO.Id;
            MessageFilter.UserId = Conversation_MessageFilterDTO.UserId;
            MessageFilter.Content = Conversation_MessageFilterDTO.Content;
            MessageFilter.ConversationId = Conversation_MessageFilterDTO.ConversationId;
            List<Message> Messages = await MessageService.List(MessageFilter);
            List<Conversation_MessageDTO> Conversation_MessageDTOs = Messages
                .Select(x => new Conversation_MessageDTO(x)).ToList();
            return Conversation_MessageDTOs;
        }
    }
}

