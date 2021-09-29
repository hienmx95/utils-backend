using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Entities
{
    public class ChatMessage : DataEntity
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

        public ChatMessageType ChatMessageType { get; set; }
        public File? File { get; set; }
    }

    public class ChatMessageFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public GuidFilter SenderId { get; set; }
        public GuidFilter RecipientId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ChatMessageOrder OrderBy { get; set; }
    }

    public enum ChatMessageOrder
    {
        Id,
        CreatedAt,
        UpdatedAt
    }
}
