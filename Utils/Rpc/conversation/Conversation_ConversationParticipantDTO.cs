using Utils.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Utils.Entities;

namespace Utils.Rpc.conversation
{
    public class Conversation_ConversationParticipantDTO : DataDTO
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long GlobalUserId { get; set; }
        public Conversation_GlobalUserDTO GlobalUser { get; set; }   

        public Conversation_ConversationParticipantDTO() {}
        public Conversation_ConversationParticipantDTO(ConversationParticipant ConversationParticipant)
        {
            this.Id = ConversationParticipant.Id;
            this.ConversationId = ConversationParticipant.ConversationId;
            this.GlobalUserId = ConversationParticipant.GlobalUserId;
            this.GlobalUser = ConversationParticipant.GlobalUser == null ? null : new Conversation_GlobalUserDTO(ConversationParticipant.GlobalUser);
            this.Errors = ConversationParticipant.Errors;
        }
    }

    public class Conversation_ConversationParticipantFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter ConversationId { get; set; }
        
        public IdFilter GlobalUserId { get; set; }
        
        public ConversationParticipantOrder OrderBy { get; set; }
    }
}