using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Config
{
    public class RabbitConfig
    {
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string Port { get; set; }
    }
}
