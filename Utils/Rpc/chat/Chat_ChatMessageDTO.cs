using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Utils.Entities;

namespace Utils.Rpc.chat
{
    public class Chat_ChatMessageDTO : DataDTO
    {
        public long Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public string Content { get; set; }
        public long? FileId { get; set; }
        public long ChatMessageTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Chat_ChatMessageTypeDTO ChatMessageType { get; set; }
        public Chat_FileDTO? File { get; set; }

        public Chat_ChatMessageDTO(ChatMessage ChatMessage)
        {
            this.Id = ChatMessage.Id;
            this.SenderId = ChatMessage.SenderId;
            this.RecipientId = ChatMessage.RecipientId;
            this.Content = ChatMessage.Content;
            this.FileId = ChatMessage.FileId;
            this.ChatMessageTypeId = ChatMessage.ChatMessageTypeId;
            this.CreatedAt = ChatMessage.CreatedAt;
            this.UpdatedAt = ChatMessage.UpdatedAt;
            this.DeletedAt = ChatMessage.DeletedAt;

            this.ChatMessageType = new Chat_ChatMessageTypeDTO(ChatMessage.ChatMessageType);
            this.File = ChatMessage.File == null ? null : new Chat_FileDTO(ChatMessage.File);
        }
    }

    public class Chat_ChatMessageFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public GuidFilter SenderId { get; set; }
        public GuidFilter RecipientId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
    }
}
