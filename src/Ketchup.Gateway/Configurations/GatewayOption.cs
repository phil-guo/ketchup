using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ketchup.Gateway.Configurations
{
    public class GatewayOption
    {
        public string Address { get; set; }
        public int Port { get; set; } = 8090;
        public string Name { get; set; } = "gateway";
        public string KongAddress { get; set; }
        public string Protocol { get; set; } = "http";
        public string Path { get; set; } = "/api";
        public string JwtAuth { get; set; } = "/auth/token";
        
        /// <summary>
        /// 开启kong jwt授权
        /// </summary>
        public bool EnableAuth { get; set; } = false;

        public string Key { get; set; }
        public string Secret { get; set; }
        public int AuthExpired { get; set; }
    }
}
