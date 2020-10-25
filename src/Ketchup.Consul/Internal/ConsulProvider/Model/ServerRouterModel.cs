using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Consul.Internal.ConsulProvider.Model
{
    public class ServerRouterModel
    {
        public string ServiceName { get; set; }
        public string ApiUrl { get; set; }
        public string Description { get; set; }
        public string Parameter { get; set; }
        public string ClientType { get; set; }
    }
}
