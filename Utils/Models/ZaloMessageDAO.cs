using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ZaloMessageDAO
    {
        public ZaloMessageDAO()
        {
            ZaloAttachments = new HashSet<ZaloAttachmentDAO>();
        }

        public long Id { get; set; }
        public string MsgId { get; set; }
        public string Text { get; set; }
        public long Timestamp { get; set; }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }

        public virtual ZaloUserDAO Recipient { get; set; }
        public virtual ZaloUserDAO Sender { get; set; }
        public virtual ICollection<ZaloAttachmentDAO> ZaloAttachments { get; set; }
    }
}
