using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class FacebookUserDAO
    {
        public FacebookUserDAO()
        {
            FacebookMessageRecipients = new HashSet<FacebookMessageDAO>();
            FacebookMessageSenders = new HashSet<FacebookMessageDAO>();
        }

        public long Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }

        public virtual ICollection<FacebookMessageDAO> FacebookMessageRecipients { get; set; }
        public virtual ICollection<FacebookMessageDAO> FacebookMessageSenders { get; set; }
    }
}
