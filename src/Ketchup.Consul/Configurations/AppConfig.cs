using System.Collections.Generic;
using Ketchup.Core.Address;
using Microsoft.Extensions.Configuration;

namespace Ketchup.Consul.Configurations
{
    public class AppConfig
    {
        /// <summary>
        /// consul 上下文配置
        /// </summary>
        public static IConfigurationRoot AppConfigRoot { get; set; }

        /// <summary>
        /// ip地址模型集合
        /// </summary>
        public IEnumerable<IpAddressModel> Addresses { get; set; }

        /// <summary>
        /// consul配置模型
        /// </summary>
        public ConsulOption Consul { get; set; } = new ConsulOption();

        public AppConfig()
        {
            GetConsulAppConfig();

            if (string.IsNullOrEmpty(Consul.ConnectionString))
                return;

            Addresses = new[]
            {
                ConvertToIpAddressModel(Consul.ConnectionString)
            };
        }

        protected ConsulOption GetConsulAppConfig()
        {
            if (AppConfigRoot == null)
                return Consul;

            Consul = AppConfigRoot.Get<ConsulOption>();
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