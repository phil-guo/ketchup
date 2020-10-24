using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Consul.Internal.ConsulProvider.Model
{
    public class ServerModel
    {
        public string Name { get; set; }

        public List<ServerClusterModel> Cluster { get; set; } = new List<ServerClusterModel>();
    }
}
