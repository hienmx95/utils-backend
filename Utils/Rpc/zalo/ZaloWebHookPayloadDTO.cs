using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Rpc.zalo
{
    public class ZaloWebHookPayloadDTO
    {
        public string app_id { get; set; }
        public ZaloProductDTO product { get; set; }
        public string oa_id { get; set; }
        public string user_id_by_app { get; set; }
        public string event_name { get; set; }
        public ZaloSenderDTO sender { get; set; }
        public ZaloRecipientDTO recipient { get; set; }
        public ZaloMessageDTO message { get; set; }
        public ZaloTagDTO tag { get; set; }
        public ZaloFollowerDTO follower { get; set; }
        public string source { get; set; }
        public string timestamp { get; set; }
        public ZaloInfoDTO info { get; set; }
    }
}
