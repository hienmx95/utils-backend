using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Rpc.zalo
{
    public class ZaloInfoDTO
    {
        public string address { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string name { get; set; }
        public string ward { get; set; }
    }
}
