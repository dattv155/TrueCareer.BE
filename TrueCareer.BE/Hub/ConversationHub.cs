using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TrueCareer.Entities;
using TrueCareer.Enums;
using TrueCareer.Repositories;
using Microsoft.Extensions.DependencyInjection;
namespace TrueCareer.Hub
{
    [Authorize]
    public class ConversationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private IServiceProvider ServiceProvider;
        public ConversationHub(IServiceProvider ServiceProvider)
        {
            this.ServiceProvider = ServiceProvider;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">rowid của appuser hoặc supplieruser</param>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string conversationId, string userId, string action, string conversationMessageId, string message)
        {
            IUOW UOW = ServiceProvider.GetService<IUOW>();
            long ConversationId = long.Parse(conversationId);
            long UserId = long.Parse(userId);
            long ConversationMessageId = long.Parse(conversationMessageId);
            Conversation Conversation = await UOW.ConversationRepository.Get(ConversationId);
            GlobalUser GlobalUser = await UOW.GlobalUserRepository.Get(UserId);
            ConversationMessage ConversationMessage = new ConversationMessage
            {
                Id = ConversationMessageId,
                ConversationId = ConversationId,
                GlobalUserId = UserId,
                Content = message,
                GlobalUser = GlobalUser,
            };
            switch(action)
            {
                case "CREATE":
                    ConversationMessage.Id = 0;
                    await UOW.ConversationMessageRepository.Create(ConversationMessage);
                    break;
                case "UPDATE":
                    await UOW.ConversationMessageRepository.Update(ConversationMessage);
                    break;
                case "DELETE":
                    await UOW.ConversationMessageRepository.Delete(ConversationMessage);
                    break;
            }    
            
            foreach (ConversationParticipant ConversationParticipant in Conversation.ConversationParticipants)
            {
                message = JsonConvert.SerializeObject(ConversationMessage);
                _ = Clients.User(ConversationParticipant.GlobalUser.RowId.ToString()).SendAsync("ReceiveMessage", conversationId, userId, message);
            }
        }
    }
}
