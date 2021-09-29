using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ConversationDAO
    {
        public ConversationDAO()
        {
            ConversationMessages = new HashSet<ConversationMessageDAO>();
            ConversationParticipants = new HashSet<ConversationParticipantDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<ConversationMessageDAO> ConversationMessages { get; set; }
        public virtual ICollection<ConversationParticipantDAO> ConversationParticipants { get; set; }
    }
}
