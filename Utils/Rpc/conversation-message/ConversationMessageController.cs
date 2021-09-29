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
using System.Dynamic;
using Utils.Entities;
using Utils.Services.MConversationMessage;
using Utils.Services.MConversation;
using Utils.Services.MConversationType;
using Utils.Services.MGlobalUser;

namespace Utils.Rpc.conversation_message
{
    public partial class ConversationMessageController : RpcController
    {
        private IConversationService ConversationService;
        private IConversationTypeService ConversationTypeService;
        private IGlobalUserService GlobalUserService;
        private IConversationMessageService ConversationMessageService;
        private ICurrentContext CurrentContext;
        public ConversationMessageController(
            IConversationService ConversationService,
            IConversationTypeService ConversationTypeService,
            IGlobalUserService GlobalUserService,
            IConversationMessageService ConversationMessageService,
            ICurrentContext CurrentContext
        )
        {
            this.ConversationService = ConversationService;
            this.ConversationTypeService = ConversationTypeService;
            this.GlobalUserService = GlobalUserService;
            this.ConversationMessageService = ConversationMessageService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ConversationMessageRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ConversationMessage_ConversationMessageFilterDTO ConversationMessage_ConversationMessageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessageFilter ConversationMessageFilter = ConvertFilterDTOToFilterEntity(ConversationMessage_ConversationMessageFilterDTO);
            int count = await ConversationMessageService.Count(ConversationMessageFilter);
            return count;
        }

        [Route(ConversationMessageRoute.List), HttpPost]
        public async Task<ActionResult<List<ConversationMessage_ConversationMessageDTO>>> List([FromBody] ConversationMessage_ConversationMessageFilterDTO ConversationMessage_ConversationMessageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessageFilter ConversationMessageFilter = ConvertFilterDTOToFilterEntity(ConversationMessage_ConversationMessageFilterDTO);
            List<ConversationMessage> ConversationMessages = await ConversationMessageService.List(ConversationMessageFilter);
            List<ConversationMessage_ConversationMessageDTO> ConversationMessage_ConversationMessageDTOs = ConversationMessages
                .Select(c => new ConversationMessage_ConversationMessageDTO(c)).ToList();
            return ConversationMessage_ConversationMessageDTOs;
        }

        [Route(ConversationMessageRoute.Get), HttpPost]
        public async Task<ActionResult<ConversationMessage_ConversationMessageDTO>> Get([FromBody]ConversationMessage_ConversationMessageDTO ConversationMessage_ConversationMessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessage ConversationMessage = await ConversationMessageService.Get(ConversationMessage_ConversationMessageDTO.Id);
            return new ConversationMessage_ConversationMessageDTO(ConversationMessage);
        }

        [Route(ConversationMessageRoute.Create), HttpPost]
        public async Task<ActionResult<ConversationMessage_ConversationMessageDTO>> Create([FromBody] ConversationMessage_ConversationMessageDTO ConversationMessage_ConversationMessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            ConversationMessage ConversationMessage = ConvertDTOToEntity(ConversationMessage_ConversationMessageDTO);
            ConversationMessage = await ConversationMessageService.Create(ConversationMessage);
            ConversationMessage_ConversationMessageDTO = new ConversationMessage_ConversationMessageDTO(ConversationMessage);
            if (ConversationMessage.IsValidated)
                return ConversationMessage_ConversationMessageDTO;
            else
                return BadRequest(ConversationMessage_ConversationMessageDTO);
        }

        [Route(ConversationMessageRoute.Update), HttpPost]
        public async Task<ActionResult<ConversationMessage_ConversationMessageDTO>> Update([FromBody] ConversationMessage_ConversationMessageDTO ConversationMessage_ConversationMessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            ConversationMessage ConversationMessage = ConvertDTOToEntity(ConversationMessage_ConversationMessageDTO);
            ConversationMessage = await ConversationMessageService.Update(ConversationMessage);
            ConversationMessage_ConversationMessageDTO = new ConversationMessage_ConversationMessageDTO(ConversationMessage);
            if (ConversationMessage.IsValidated)
                return ConversationMessage_ConversationMessageDTO;
            else
                return BadRequest(ConversationMessage_ConversationMessageDTO);
        }

        [Route(ConversationMessageRoute.Delete), HttpPost]
        public async Task<ActionResult<ConversationMessage_ConversationMessageDTO>> Delete([FromBody] ConversationMessage_ConversationMessageDTO ConversationMessage_ConversationMessageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationMessage ConversationMessage = ConvertDTOToEntity(ConversationMessage_ConversationMessageDTO);
            ConversationMessage = await ConversationMessageService.Delete(ConversationMessage);
            ConversationMessage_ConversationMessageDTO = new ConversationMessage_ConversationMessageDTO(ConversationMessage);
            if (ConversationMessage.IsValidated)
                return ConversationMessage_ConversationMessageDTO;
            else
                return BadRequest(ConversationMessage_ConversationMessageDTO);
        }
        
        private ConversationMessage ConvertDTOToEntity(ConversationMessage_ConversationMessageDTO ConversationMessage_ConversationMessageDTO)
        {
            ConversationMessage ConversationMessage = new ConversationMessage();
            ConversationMessage.Id = ConversationMessage_ConversationMessageDTO.Id;
            ConversationMessage.ConversationId = ConversationMessage_ConversationMessageDTO.ConversationId;
            ConversationMessage.ConversationTypeId = ConversationMessage_ConversationMessageDTO.ConversationTypeId;
            ConversationMessage.GlobalUserId = ConversationMessage_ConversationMessageDTO.GlobalUserId;
            ConversationMessage.Content = ConversationMessage_ConversationMessageDTO.Content;
            ConversationMessage.Conversation = ConversationMessage_ConversationMessageDTO.Conversation == null ? null : new Conversation
            {
                Id = ConversationMessage_ConversationMessageDTO.Conversation.Id,
                Name = ConversationMessage_ConversationMessageDTO.Conversation.Name,
            };
            ConversationMessage.ConversationType = ConversationMessage_ConversationMessageDTO.ConversationType == null ? null : new ConversationType
            {
                Id = ConversationMessage_ConversationMessageDTO.ConversationType.Id,
                Code = ConversationMessage_ConversationMessageDTO.ConversationType.Code,
                Name = ConversationMessage_ConversationMessageDTO.ConversationType.Name,
            };
            ConversationMessage.GlobalUser = ConversationMessage_ConversationMessageDTO.GlobalUser == null ? null : new GlobalUser
            {
                Id = ConversationMessage_ConversationMessageDTO.GlobalUser.Id,
                Username = ConversationMessage_ConversationMessageDTO.GlobalUser.Username,
                DisplayName = ConversationMessage_ConversationMessageDTO.GlobalUser.DisplayName,
                RowId = ConversationMessage_ConversationMessageDTO.GlobalUser.RowId,
            };
            ConversationMessage.BaseLanguage = CurrentContext.Language;
            return ConversationMessage;
        }

        private ConversationMessageFilter ConvertFilterDTOToFilterEntity(ConversationMessage_ConversationMessageFilterDTO ConversationMessage_ConversationMessageFilterDTO)
        {
            ConversationMessageFilter ConversationMessageFilter = new ConversationMessageFilter();
            ConversationMessageFilter.Selects = ConversationMessageSelect.ALL;
            ConversationMessageFilter.Skip = ConversationMessage_ConversationMessageFilterDTO.Skip;
            ConversationMessageFilter.Take = ConversationMessage_ConversationMessageFilterDTO.Take;
            ConversationMessageFilter.OrderBy = ConversationMessage_ConversationMessageFilterDTO.OrderBy;
            ConversationMessageFilter.OrderType = ConversationMessage_ConversationMessageFilterDTO.OrderType;

            ConversationMessageFilter.Id = ConversationMessage_ConversationMessageFilterDTO.Id;
            ConversationMessageFilter.ConversationId = ConversationMessage_ConversationMessageFilterDTO.ConversationId;
            ConversationMessageFilter.ConversationTypeId = ConversationMessage_ConversationMessageFilterDTO.ConversationTypeId;
            ConversationMessageFilter.GlobalUserId = ConversationMessage_ConversationMessageFilterDTO.GlobalUserId;
            ConversationMessageFilter.Content = ConversationMessage_ConversationMessageFilterDTO.Content;
            ConversationMessageFilter.CreatedAt = ConversationMessage_ConversationMessageFilterDTO.CreatedAt;
            ConversationMessageFilter.UpdatedAt = ConversationMessage_ConversationMessageFilterDTO.UpdatedAt;
            return ConversationMessageFilter;
        }
    }
}

