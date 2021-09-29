using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class GlobalUserDAO
    {
        public GlobalUserDAO()
        {
            ConversationMessages = new HashSet<ConversationMessageDAO>();
            ConversationParticipants = new HashSet<ConversationParticipantDAO>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public Guid RowId { get; set; }

        public virtual ICollection<ConversationMessageDAO> ConversationMessages { get; set; }
        public virtual ICollection<ConversationParticipantDAO> ConversationParticipants { get; set; }
    }
}
