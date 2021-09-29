using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Entities
{
    public class ZaloMessage : DataEntity
    {
        public string message_id { get; set; }
        public long user_id { get; set; }
        public string content { get; set; }
        public string image_url { get; set; }
        public string file_attachment_id { get; set; }
        public string attachment_id { get; set; }
        public ZaloAttachment attachment { get; set; }
    }
}
