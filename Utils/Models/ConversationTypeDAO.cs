using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ConversationTypeDAO
    {
        public ConversationTypeDAO()
        {
            ConversationMessages = new HashSet<ConversationMessageDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ConversationMessageDAO> ConversationMessages { get; set; }
    }
}
