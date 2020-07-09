using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ketchup.Consul.Internal.ClientProvider;
using Ketchup.Consul.Internal.Selector;
using Ketchup.Consul.Internal.Selector.Implementation;
using Ketchup.Core;
using Ketchup.Core.Address;
using Ketchup.Core.Address.Selectors.Implementation;
using Ketchup.Core.Utilities;
using NConsul;
using AppConfig = Ketchup.Consul.Configurations.AppConfig;

namespace Ketchup.Consul.Internal.ConsulProvider.Implementation
{
    public class DefaultConsulProivder : IConsulProvider, IDisposable
    {
        private readonly IConsulClientProvider _consulClientProvider;
        private readonly ConcurrentDictionary<string, ServiceEntry[]> _dictionary = new ConcurrentDictionary<string, ServiceEntry[]>();
        private readonly Timer _timer;

        private readonly ConcurrentDictionary<string, IConsulAddressSelector> _addressSelectors =
            new ConcurrentDictionary<string, IConsulAddressSelector>();

        public AppConfig AppConfig { get; set; }

        public DefaultConsulProivder(IConsulClientProvider consulClientProvider, AppConfig appConfig)
        {
            AppConfig = appConfig;
            _consulClientProvider = consulClientProvider;

            var timeSpan = TimeSpan.FromSeconds(10);

            _timer = new Timer(async item => { await Check(); }, null, timeSpan, timeSpan);
        }

        public void AddLoadAddressSelector(KetchupPlatformContainer build)
        {
            foreach (var selector in Enum.GetValues(typeof(SelectorType)))
            {
                _addressSelectors.TryAdd(selector.ToString(), build.GetInstances<IConsulAddressSelector>(selector.ToString()));
            }
        }

        public async Task RegiserConsulAgent()
        {
            var consulClient = _consulClientProvider.GetConsulClient();
            var config = Core.Configurations.AppConfig.ServerOptions;

            if (!await IsExistAgent(consulClient, config.Name, config.Ip, config.Port))
                return;

            var agent = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString("N"),
                Name = config.Name,
                Address = config.Ip,
                Port = config.Port,
                Tags = new[] { $"urlprefix-/{config.Ip}:{config.Port}" },
                Meta = new Dictionary<string, string>() { { SelectorType.RandomWeight.ToString(), config.Weight.ToString() } }
            };


            if (AppConfig.Consul.IsHealthCheck)
            {
                var check = new AgentServiceCheck
                {
                    //consul服务注册之后几秒开始检查
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    //10秒检查一次
                    Interval = TimeSpan.FromSeconds(10),
                    GRPC = $"{config.Ip}:{config.Port}",
                    GRPCUseTLS = false,
                    Timeout = TimeSpan.FromSeconds(10)
                };

                agent.Check = check;
            }

            await consulClient.Agent.ServiceRegister(agent);

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                consulClient.Agent.ServiceDeregister(agent.ID).Wait();
            };
        }

        public async ValueTask<IpAddressModel> FindServiceEntry(string serverName)
        {
            var consulAddressSelector = ServiceLocator.GetService<IConsulAddressSelector>(AppConfig.Consul.Strategy);
            var client = _consulClientProvider.GetConsulClient();
            ServiceEntry[] healths = _dictionary.GetOrAdd(serverName, (await client.Health.Service(serverName, "", true)).Response);

            var ipAddressModels = new List<AddressModel>();

            healths.ToList().ForEach(service =>
            {
                ipAddressModels.Add(new IpAddressModel()
                {
                    Ip = service.Service.Address,
                    Port = service.Service.Port,
                    Meta = service.Service.Meta
                });
            });

            var ipAddressModel = await consulAddressSelector.SelectAsync(new AddressSelectContext()
            {
                Address = ipAddressModels,
                Name = serverName
            });

            return ipAddressModel as IpAddressModel;
        }

        private async Task<bool> IsExistAgent(ConsulClient consulClient, string serverName, string ip, int port)
        {
            var items = await consulClient.Agent.Services();

            var service = items.Response.Values.FirstOrDefault(item =>
                item.Service == serverName && item.Address == ip && item.Port == port);

            return service == null;
        }

        private async Task Check()
        {
            var client = _consulClientProvider.GetConsulClient();

            foreach (var key in _dictionary.Keys)
            {
                var healths = (await client.Health.Service(key, "", true)).Response;

                _dictionary[key] = healths;
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
