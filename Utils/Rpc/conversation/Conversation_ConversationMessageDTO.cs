using Utils.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Utils.Entities;

namespace Utils.Rpc.conversation
{
    public class Conversation_ConversationMessageDTO : DataDTO
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long ConversationTypeId { get; set; }
        public long GlobalUserId { get; set; }
        public string Content { get; set; }
        public Conversation_ConversationTypeDTO ConversationType { get; set; }   
        public Conversation_GlobalUserDTO GlobalUser { get; set; }   

        public Conversation_ConversationMessageDTO() {}
        public Conversation_ConversationMessageDTO(ConversationMessage ConversationMessage)
        {
            this.Id = ConversationMessage.Id;
            this.ConversationId = ConversationMessage.ConversationId;
            this.ConversationTypeId = ConversationMessage.ConversationTypeId;
            this.GlobalUserId = ConversationMessage.GlobalUserId;
            this.Content = ConversationMessage.Content;
            this.ConversationType = ConversationMessage.ConversationType == null ? null : new Conversation_ConversationTypeDTO(ConversationMessage.ConversationType);
            this.GlobalUser = ConversationMessage.GlobalUser == null ? null : new Conversation_GlobalUserDTO(ConversationMessage.GlobalUser);
            this.Errors = ConversationMessage.Errors;
        }
    }

    public class Conversation_ConversationMessageFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter ConversationId { get; set; }
        
        public IdFilter ConversationTypeId { get; set; }
        
        public IdFilter GlobalUserId { get; set; }
        
        public StringFilter Content { get; set; }
        
        public ConversationMessageOrder OrderBy { get; set; }
    }
}