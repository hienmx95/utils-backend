using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Rpc.facebook
{
    public class FacebookMessagingDTO
    {
        public FacebookSenderDTO sender { get; set; }
        public FacebookRecipientDTO recipient { get; set; }
        public long timestamp { get; set; }
        public FacebookMessageDTO message { get; set; }
    }
}
