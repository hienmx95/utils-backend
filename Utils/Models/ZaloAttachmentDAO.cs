using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ZaloAttachmentDAO
    {
        public long Id { get; set; }
        public long ZaloMessageId { get; set; }
        public long ZaloAttachmentTypeId { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public string Size { get; set; }
        public string Name { get; set; }
        public string Checksum { get; set; }
        public string Type { get; set; }

        public virtual ZaloAttachmentTypeDAO ZaloAttachmentType { get; set; }
        public virtual ZaloMessageDAO ZaloMessage { get; set; }
    }
}
