using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ConversationMessageDAO
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long ConversationTypeId { get; set; }
        public long GlobalUserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ConversationDAO Conversation { get; set; }
        public virtual ConversationTypeDAO ConversationType { get; set; }
        public virtual GlobalUserDAO GlobalUser { get; set; }
    }
}
