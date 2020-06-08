using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Ketchup.Gateway.Configurations
{
    public class AppConfig
    {
        public GatewayOption Gateway { get; set; } = new GatewayOption();

        public AppConfig()
        {
            GetGatewayAppConfig();
        }

        protected GatewayOption GetGatewayAppConfig()
        {
            var section = Core.Configurations.AppConfig.GetSection("Gateway");

            if (section.Exists())
                Gateway = section.Get<GatewayOption>();
            return Gateway;
        }
    }
}
