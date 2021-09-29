using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Rpc.zalo
{
    public class ZaloTagDTO
    {
        public string name { get; set; }
        public List<string> user_ids { get; set; }
    }
}
