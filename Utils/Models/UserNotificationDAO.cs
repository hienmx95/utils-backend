﻿using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class UserNotificationDAO
    {
        public long Id { get; set; }
        public long SiteId { get; set; }
        public string TitleWeb { get; set; }
        public string ContentWeb { get; set; }
        public string TitleMobile { get; set; }
        public string ContentMobile { get; set; }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public bool Unread { get; set; }
        public DateTime Time { get; set; }
        public string LinkWebsite { get; set; }
        public string LinkMobile { get; set; }
        public Guid RowId { get; set; }

        public virtual AppUserDAO Recipient { get; set; }
        public virtual AppUserDAO Sender { get; set; }
    }
}
