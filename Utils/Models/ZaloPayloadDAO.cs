using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ZaloPayloadDAO
    {
        public long Id { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string ComputedHeader { get; set; }
    }
}
