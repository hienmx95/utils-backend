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
using Utils.Services.MConversation;
using Utils.Services.MConversationMessage;
using Utils.Services.MConversationType;
using Utils.Services.MGlobalUser;

namespace Utils.Rpc.conversation
{
    public partial class ConversationController : RpcController
    {
        private IConversationMessageService ConversationMessageService;
        private IConversationTypeService ConversationTypeService;
        private IGlobalUserService GlobalUserService;
        private IConversationService ConversationService;
        private ICurrentContext CurrentContext;
        public ConversationController(
            IConversationMessageService ConversationMessageService,
            IConversationTypeService ConversationTypeService,
            IGlobalUserService GlobalUserService,
            IConversationService ConversationService,
            ICurrentContext CurrentContext
        )
        {
            this.ConversationMessageService = ConversationMessageService;
            this.ConversationTypeService = ConversationTypeService;
            this.GlobalUserService = GlobalUserService;
            this.ConversationService = ConversationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ConversationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Conversation_ConversationFilterDTO Conversation_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationFilter ConversationFilter = ConvertFilterDTOToFilterEntity(Conversation_ConversationFilterDTO);
            int count = await ConversationService.Count(ConversationFilter);
            return count;
        }

        [Route(ConversationRoute.List), HttpPost]
        public async Task<ActionResult<List<Conversation_ConversationDTO>>> List([FromBody] Conversation_ConversationFilterDTO Conversation_ConversationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConversationFilter ConversationFilter = ConvertFilterDTOToFilterEntity(Conversation_ConversationFilterDTO);
            List<Conversation> Conversations = await ConversationService.List(ConversationFilter);
            List<Conversation_ConversationDTO> Conversation_ConversationDTOs = Conversations
                .Select(c => new Conversation_ConversationDTO(c)).ToList();
            return Conversation_ConversationDTOs;
        }

        [Route(ConversationRoute.Get), HttpPost]
        public async Task<ActionResult<Conversation_ConversationDTO>> Get([FromBody]Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Conversation Conversation = await ConversationService.Get(Conversation_ConversationDTO.Id);
            return new Conversation_ConversationDTO(Conversation);
        }

        [Route(ConversationRoute.Create), HttpPost]
        public async Task<ActionResult<Conversation_ConversationDTO>> Create([FromBody] Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Conversation Conversation = ConvertDTOToEntity(Conversation_ConversationDTO);
            Conversation = await ConversationService.Create(Conversation);
            Conversation_ConversationDTO = new Conversation_ConversationDTO(Conversation);
            if (Conversation.IsValidated)
                return Conversation_ConversationDTO;
            else
                return BadRequest(Conversation_ConversationDTO);
        }

        [Route(ConversationRoute.Update), HttpPost]
        public async Task<ActionResult<Conversation_ConversationDTO>> Update([FromBody] Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Conversation Conversation = ConvertDTOToEntity(Conversation_ConversationDTO);
            Conversation = await ConversationService.Update(Conversation);
            Conversation_ConversationDTO = new Conversation_ConversationDTO(Conversation);
            if (Conversation.IsValidated)
                return Conversation_ConversationDTO;
            else
                return BadRequest(Conversation_ConversationDTO);
        }

        [Route(ConversationRoute.Delete), HttpPost]
        public async Task<ActionResult<Conversation_ConversationDTO>> Delete([FromBody] Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Conversation Conversation = ConvertDTOToEntity(Conversation_ConversationDTO);
            Conversation = await ConversationService.Delete(Conversation);
            Conversation_ConversationDTO = new Conversation_ConversationDTO(Conversation);
            if (Conversation.IsValidated)
                return Conversation_ConversationDTO;
            else
                return BadRequest(Conversation_ConversationDTO);
        }
        

        private Conversation ConvertDTOToEntity(Conversation_ConversationDTO Conversation_ConversationDTO)
        {
            Conversation Conversation = new Conversation();
            Conversation.Id = Conversation_ConversationDTO.Id;
            Conversation.Name = Conversation_ConversationDTO.Name;
            Conversation.ConversationParticipants = Conversation_ConversationDTO.ConversationParticipants?
                .Select(x => new ConversationParticipant
                {
                    Id = x.Id,
                    GlobalUserId = x.GlobalUserId,
                    GlobalUser = x.GlobalUser == null ? null : new GlobalUser
                    {
                        Id = x.GlobalUser.Id,
                        Username = x.GlobalUser.Username,
                        DisplayName = x.GlobalUser.DisplayName,
                        RowId = x.GlobalUser.RowId,
                    },
                }).ToList();
            Conversation.BaseLanguage = CurrentContext.Language;
            return Conversation;
        }

        private ConversationFilter ConvertFilterDTOToFilterEntity(Conversation_ConversationFilterDTO Conversation_ConversationFilterDTO)
        {
            ConversationFilter ConversationFilter = new ConversationFilter();
            ConversationFilter.Selects = ConversationSelect.ALL;
            ConversationFilter.Skip = Conversation_ConversationFilterDTO.Skip;
            ConversationFilter.Take = Conversation_ConversationFilterDTO.Take;
            ConversationFilter.OrderBy = Conversation_ConversationFilterDTO.OrderBy;
            ConversationFilter.OrderType = Conversation_ConversationFilterDTO.OrderType;

            ConversationFilter.Id = Conversation_ConversationFilterDTO.Id;
            ConversationFilter.Name = Conversation_ConversationFilterDTO.Name;
            ConversationFilter.CreatedAt = Conversation_ConversationFilterDTO.CreatedAt;
            ConversationFilter.UpdatedAt = Conversation_ConversationFilterDTO.UpdatedAt;
            return ConversationFilter;
        }
    }
}

