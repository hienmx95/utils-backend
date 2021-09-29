using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Rpc.facebook
{
    public class FacebookEntryDTO
    {
        public string id { get; set; }
        public long time { get; set; }
        public List<FacebookMessagingDTO> messaging { get; set; }
    }
}
