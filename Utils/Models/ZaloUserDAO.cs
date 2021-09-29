using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ZaloUserDAO
    {
        public ZaloUserDAO()
        {
            ZaloMessageRecipients = new HashSet<ZaloMessageDAO>();
            ZaloMessageSenders = new HashSet<ZaloMessageDAO>();
        }

        public long Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }

        public virtual ICollection<ZaloMessageDAO> ZaloMessageRecipients { get; set; }
        public virtual ICollection<ZaloMessageDAO> ZaloMessageSenders { get; set; }
    }
}
