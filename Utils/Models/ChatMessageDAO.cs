using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ChatMessageDAO
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

        public virtual ChatMessageTypeDAO ChatMessageType { get; set; }
        public virtual FileDAO File { get; set; }
    }
}
