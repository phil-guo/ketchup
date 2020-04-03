using System.Collections.Generic;
using Ketchup.Core.Address;
using Microsoft.Extensions.Configuration;

namespace Ketchup.Consul.Configurations
{
    public class AppConfig
    {
        /// <summary>
        /// ip地址模型
        /// </summary>
        public IpAddressModel Addresse { get; set; }

        /// <summary>
        /// consul配置模型
        /// </summary>
        public ConsulOption Consul { get; set; } = new ConsulOption();

        public AppConfig()
        {
            GetConsulAppConfig();

            if (string.IsNullOrEmpty(Consul.ConnectionString))
                return;

            Addresse = ConvertToIpAddressModel(Consul.ConnectionString);
        }

        protected ConsulOption GetConsulAppConfig()
        {
            var section = Core.Configurations.AppConfig.GetSection("Consul");

            if (section.Exists())
                Consul = section.Get<ConsulOption>();
            return Consul;
        }

        protected IpAddressModel ConvertToIpAddressModel(string connectionString)
        {
            var address = connectionString.Split(":");
            if (address.Length <= 1)
                return null;

            int.TryParse(address[1], out var port);
            return new IpAddressModel
            {
                Ip = address[0],
                Port = port
            };
        }

    }
}