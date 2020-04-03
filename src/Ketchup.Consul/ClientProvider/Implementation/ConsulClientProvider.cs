using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;
using Ketchup.Consul.HealthCheck;
using Ketchup.Consul.Selector;
using Ketchup.Core;
using Ketchup.Core.Address;
using Ketchup.Core.Address.Selectors.Implementation;
using Ketchup.Core.Exceptions;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Ketchup.Consul.ClientProvider.Implementation
{
    public class ConsulClientProvider : IConsulClientProvider
    {
        private readonly ConcurrentDictionary<IpAddressModel, ConsulClient> _consulClients = new ConcurrentDictionary<IpAddressModel, ConsulClient>();

        private readonly IConsulAddressSelector _consulAddressSelector;
        private readonly IHealthCheckService _healthCheck;
        private readonly ILogger<ConsulClientProvider> _logger;

        public ConsulClientProvider(IHealthCheckService healthCheck,
            IConsulAddressSelector consulAddressSelector,
            ILogger<ConsulClientProvider> logger)
        {
            _healthCheck = healthCheck;
            _consulAddressSelector = consulAddressSelector;
            _logger = logger;
        }

        public Ketchup.Consul.Configurations.AppConfig AppConfig { get; set; }

        public ConsulClient GetConsulClient()
        {
            var client = new ConsulClient(
                config => { config.Address = new Uri($"http://{AppConfig.Consul.Ip}:{AppConfig.Consul?.Port}"); }, null, h =>
                {
                    h.UseProxy = false;
                    h.Proxy = null;
                });

            return client;
        }

        public async ValueTask<ConsulClient> GetClient()
        {
            ConsulClient result = null;
            var addresses = new List<IpAddressModel>();
            foreach (var addressModel in AppConfig.Addresses)
            {
                _healthCheck.Monitor(addressModel);
                var task = await _healthCheck.IsHealth(addressModel);
                if (!task)
                    continue;
                addresses.Add(addressModel);
            }

            if (addresses.Count == 0)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("找不到可用的注册中心地址。");
                return null;
            }

            var vt = _consulAddressSelector.SelectAsync(new AddressSelectContext
            {
                Descriptor = new ServiceDescriptor { Id = nameof(ConsulClientProvider) },
                Address = addresses
            });
            var address = vt.IsCompletedSuccessfully ? vt.Result : await vt;

            if (address != null)
            {
                var ipAddress = address as IpAddressModel;
                result = _consulClients.GetOrAdd(ipAddress, new ConsulClient(
                    config => { config.Address = new Uri($"http://{ipAddress?.Ip}:{ipAddress?.Port}"); }, null, h =>
                    {
                        h.UseProxy = false;
                        h.Proxy = null;
                    }));
            }

            return result;
        }

        public async ValueTask<IEnumerable<ConsulClient>> GetClients()
        {
            var result = new List<ConsulClient>();
            foreach (var ipAddress in AppConfig.Addresses)
                if (await _healthCheck.IsHealth(ipAddress))
                    result.Add(_consulClients.GetOrAdd(ipAddress, new ConsulClient(
                        config =>
                        {
                            config.Address = new Uri($"http://{ipAddress.Ip}:{ipAddress.Port}");
                        }, null, h =>
                        {
                            h.UseProxy = false;
                            h.Proxy = null;
                        })));
            return result;
        }

        public async ValueTask Check()
        {
            foreach (var address in AppConfig.Addresses)
                if (!await _healthCheck.IsHealth(address))
                    throw new KetchupPlatformException(string.Format("注册中心{0}连接异常，请联系管理员", address));
        }
    }
}