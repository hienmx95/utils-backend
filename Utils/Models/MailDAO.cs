using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class MailDAO
    {
        public MailDAO()
        {
            Attachments = new HashSet<AttachmentDAO>();
        }

        public long Id { get; set; }
        public string Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public long RetryCount { get; set; }
        public string Error { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual ICollection<AttachmentDAO> Attachments { get; set; }
    }
}
