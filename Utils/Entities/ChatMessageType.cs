using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Entities
{
    public class ChatMessageType : DataEntity
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
