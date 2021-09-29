using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ChatMessageTypeDAO
    {
        public ChatMessageTypeDAO()
        {
            ChatMessages = new HashSet<ChatMessageDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ChatMessageDAO> ChatMessages { get; set; }
    }
}
