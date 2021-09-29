using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class AttachmentDAO
    {
        public long Id { get; set; }
        public long MailId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Url { get; set; }

        public virtual MailDAO Mail { get; set; }
    }
}
