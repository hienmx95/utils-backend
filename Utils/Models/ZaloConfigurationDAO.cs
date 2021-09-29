using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ZaloConfigurationDAO
    {
        public Guid Id { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string AppName { get; set; }
        public string OASecretKey { get; set; }
    }
}
