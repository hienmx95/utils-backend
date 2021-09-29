using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Rpc.zalo
{
    public class ZaloMessageDTO
    {
        public string msg_id { get; set; }
        public List<string> msg_ids { get; set; }
        public string text { get; set; }
        public List<ZaloAttachmentDTO> attachments { get; set; }
    }
}
