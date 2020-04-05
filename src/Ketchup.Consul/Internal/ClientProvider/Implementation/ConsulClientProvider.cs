using System;
using Ketchup.Consul.Internal.Selector;
using NConsul;

namespace Ketchup.Consul.Internal.ClientProvider.Implementation
{
    public class ConsulClientProvider : IConsulClientProvider
    {
        private readonly IConsulAddressSelector _consulAddressSelector;

        public ConsulClientProvider(IConsulAddressSelector consulAddressSelector)
        {
            _consulAddressSelector = consulAddressSelector;
        }

        public Configurations.AppConfig AppConfig { get; set; }

        public ConsulClient GetConsulClient()
        {
            var client = new ConsulClient(
                config => { config.Address = new Uri($"http://{AppConfig.Addresse.Ip}:{AppConfig.Addresse?.Port}"); }, null, h =>
                {
                    h.UseProxy = false;
                    h.Proxy = null;
                });

            return client;
        }
    }
}