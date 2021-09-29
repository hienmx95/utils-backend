using Utils.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Utils.Entities;

namespace Utils.Rpc.conversation_message
{
    public class ConversationMessage_ConversationDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        

        public ConversationMessage_ConversationDTO() {}
        public ConversationMessage_ConversationDTO(Conversation Conversation)
        {
            
            this.Id = Conversation.Id;
            
            this.Name = Conversation.Name;
            
            this.Errors = Conversation.Errors;
        }
    }

    public class ConversationMessage_ConversationFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public ConversationOrder OrderBy { get; set; }
    }
}