using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Rpc.zalo
{
    public class ZaloPayloadDTO
    {
        public string thumbnail { get; set; }
        public string url { get; set; }
        public string size { get; set; }
        public string name { get; set; }
        public string checksum { get; set; }
        public string type { get; set; }
    }
}