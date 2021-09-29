using Utils.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Utils.Entities;

namespace Utils.Rpc.conversation
{
    public class Conversation_ConversationDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Conversation_ConversationParticipantDTO> ConversationParticipants { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Conversation_ConversationDTO() {}
        public Conversation_ConversationDTO(Conversation Conversation)
        {
            this.Id = Conversation.Id;
            this.Name = Conversation.Name;
            this.ConversationParticipants = Conversation.ConversationParticipants?.Select(x => new Conversation_ConversationParticipantDTO(x)).ToList();
            this.CreatedAt = Conversation.CreatedAt;
            this.UpdatedAt = Conversation.UpdatedAt;
            this.Errors = Conversation.Errors;
        }
    }

    public class Conversation_ConversationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ConversationOrder OrderBy { get; set; }
    }
}
