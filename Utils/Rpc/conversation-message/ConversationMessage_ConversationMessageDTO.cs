using Utils.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Utils.Entities;

namespace Utils.Rpc.conversation_message
{
    public class ConversationMessage_ConversationMessageDTO : DataDTO
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long ConversationTypeId { get; set; }
        public long GlobalUserId { get; set; }
        public string Content { get; set; }
        public ConversationMessage_ConversationDTO Conversation { get; set; }
        public ConversationMessage_ConversationTypeDTO ConversationType { get; set; }
        public ConversationMessage_GlobalUserDTO GlobalUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ConversationMessage_ConversationMessageDTO() {}
        public ConversationMessage_ConversationMessageDTO(ConversationMessage ConversationMessage)
        {
            this.Id = ConversationMessage.Id;
            this.ConversationId = ConversationMessage.ConversationId;
            this.ConversationTypeId = ConversationMessage.ConversationTypeId;
            this.GlobalUserId = ConversationMessage.GlobalUserId;
            this.Content = ConversationMessage.Content;
            this.Conversation = ConversationMessage.Conversation == null ? null : new ConversationMessage_ConversationDTO(ConversationMessage.Conversation);
            this.ConversationType = ConversationMessage.ConversationType == null ? null : new ConversationMessage_ConversationTypeDTO(ConversationMessage.ConversationType);
            this.GlobalUser = ConversationMessage.GlobalUser == null ? null : new ConversationMessage_GlobalUserDTO(ConversationMessage.GlobalUser);
            this.CreatedAt = ConversationMessage.CreatedAt;
            this.UpdatedAt = ConversationMessage.UpdatedAt;
            this.Errors = ConversationMessage.Errors;
        }
    }

    public class ConversationMessage_ConversationMessageFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter ConversationId { get; set; }
        public IdFilter ConversationTypeId { get; set; }
        public IdFilter GlobalUserId { get; set; }
        public StringFilter Content { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ConversationMessageOrder OrderBy { get; set; }
    }
}
