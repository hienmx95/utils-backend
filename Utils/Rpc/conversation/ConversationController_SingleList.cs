using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Utils.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using Utils.Entities;
using Utils.Services.MConversation;
using Utils.Services.MConversationMessage;
using Utils.Services.MConversationType;
using Utils.Services.MGlobalUser;

namespace Utils.Rpc.conversation
{
    public partial class ConversationController : RpcController
    {
        [Route(ConversationRoute.SingleListConversationMessage), HttpPost]
        public async Task<List<Conversation_ConversationMessageDTO>> SingleListConversationMessage([FromBody] Conversation_ConversationMessageFilterDTO Conversation_ConversationMessageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessageFilter ConversationMessageFilter = new ConversationMessageFilter();
            ConversationMessageFilter.Skip = 0;
            ConversationMessageFilter.Take = 20;
            ConversationMessageFilter.OrderBy = ConversationMessageOrder.Id;
            ConversationMessageFilter.OrderType = OrderType.ASC;
            ConversationMessageFilter.Selects = ConversationMessageSelect.ALL;
            ConversationMessageFilter.Id = Conversation_ConversationMessageFilterDTO.Id;
            ConversationMessageFilter.ConversationId = Conversation_ConversationMessageFilterDTO.ConversationId;
            ConversationMessageFilter.ConversationTypeId = Conversation_ConversationMessageFilterDTO.ConversationTypeId;
            ConversationMessageFilter.GlobalUserId = Conversation_ConversationMessageFilterDTO.GlobalUserId;
            ConversationMessageFilter.Content = Conversation_ConversationMessageFilterDTO.Content;
            List<ConversationMessage> ConversationMessages = await ConversationMessageService.List(ConversationMessageFilter);
            List<Conversation_ConversationMessageDTO> Conversation_ConversationMessageDTOs = ConversationMessages
                .Select(x => new Conversation_ConversationMessageDTO(x)).ToList();
            return Conversation_ConversationMessageDTOs;
        }
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
            GlobalUserFilter.Username = Conversation_GlobalUserFilterDTO.Username;
            GlobalUserFilter.DisplayName = Conversation_GlobalUserFilterDTO.DisplayName;
            GlobalUserFilter.RowId = Conversation_GlobalUserFilterDTO.RowId;
            List<GlobalUser> GlobalUsers = await GlobalUserService.List(GlobalUserFilter);
            List<Conversation_GlobalUserDTO> Conversation_GlobalUserDTOs = GlobalUsers
                .Select(x => new Conversation_GlobalUserDTO(x)).ToList();
            return Conversation_GlobalUserDTOs;
        }
     
    }
}

