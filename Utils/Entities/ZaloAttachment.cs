using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Entities
{
    public class ZaloAttachment : DataEntity
    {
        public string type { get; set; }
        public ZaloPayload payload { get; set; }
    }
}
