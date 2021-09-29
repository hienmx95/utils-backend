using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class FacebookMessageDAO
    {
        public long Id { get; set; }
        public long PageId { get; set; }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public string Text { get; set; }
        public long Timestamp { get; set; }

        public virtual FacebookPageDAO Page { get; set; }
        public virtual FacebookUserDAO Recipient { get; set; }
        public virtual FacebookUserDAO Sender { get; set; }
    }
}
