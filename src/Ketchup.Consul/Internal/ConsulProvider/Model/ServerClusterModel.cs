using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Consul.Internal.ConsulProvider.Model
{
    public class ServerClusterModel
    {
        public string Key { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public bool IsHealth { get; set; } = true;
    }
}
