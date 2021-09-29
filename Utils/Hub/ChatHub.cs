using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Utils.Entities;
using Utils.Enums;
using Utils.Repositories;

namespace Utils.Hub
{
    [Authorize]
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private IUOW UOW;
        public ChatHub(IUOW UOW)
        {
            this.UOW = UOW;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">rowid của appuser hoặc supplieruser</param>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Send(string conversationId, string userName, string message)
        {
            long id = long.Parse(conversationId);
            Conversation Conversation = await UOW.ConversationRepository.Get(id);
            List<GlobalUser> GlobalUsers = await UOW.GlobalUserRepository.List(new GlobalUserFilter
            {
                Username = new Common.StringFilter { Equal = userName },
                Skip = 0,
                Take = 1,
            });
            GlobalUser GlobalUser = GlobalUsers.FirstOrDefault();
            ConversationMessage ConversationMessage = new ConversationMessage
            {
                ConversationId = id,
                ConversationTypeId = ConversationTypeEnum.LOCAL.Id,
                GlobalUserId = GlobalUser.Id,
                Content = message,
            };
            await UOW.ConversationMessageRepository.Create(ConversationMessage);
            foreach (ConversationParticipant ConversationParticipant in Conversation.ConversationParticipants)
            {
                _ =  Clients.User(ConversationParticipant.GlobalUser.RowId.ToString()).SendAsync("Receive", conversationId, userName, message);
            }
        }
    }
}
