using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ketchup.Consul.Internal.ClientProvider;
using Ketchup.Consul.Internal.Selector;
using Ketchup.Core.Address;
using Ketchup.Core.Address.Selectors.Implementation;
using NConsul;
using AppConfig = Ketchup.Consul.Configurations.AppConfig;

namespace Ketchup.Consul.Internal.ConsulProvider.Implementation
{
    public class DefaultConsulProivder : IConsulProvider
    {
        private readonly IConsulClientProvider _consulClientProvider;
        private readonly IConsulAddressSelector _consulAddressSelector;

        public AppConfig AppConfig { get; set; }

        public DefaultConsulProivder(IConsulClientProvider consulClientProvider, IConsulAddressSelector consulAddressSelector)
        {
            _consulClientProvider = consulClientProvider;
            _consulAddressSelector = consulAddressSelector;
        }

        public async Task RegiserConsulAgent()
        {
            using (var consulClient = _consulClientProvider.GetConsulClient())
            {
                var config = Core.Configurations.AppConfig.ServerOptions;

                if (!await IsExistAgent(consulClient, config.Name, config.Ip, config.Port))
                    return;

                var agent = new AgentServiceRegistration()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    Name = config.Name,
                    Address = config.Ip,
                    Port = config.Port,
                    Tags = new[] { $"urlprefix-/{config.Ip}:{config.Port}" }
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
        }

        public async ValueTask<IpAddressModel> FindServiceEntry(string serverName)
        {

            using (var client = _consulClientProvider.GetConsulClient())
            {
                ServiceEntry[] healths = (await client.Health.Service(serverName, "", true)).Response;

                var ipAddressModels = new List<AddressModel>();

                healths.ToList().ForEach(service =>
                {
                    ipAddressModels.Add(new IpAddressModel()
                    {
                        Ip = service.Service.Address,
                        Port = service.Service.Port
                    });
                });

                var ipAddressModel = await _consulAddressSelector.SelectAsync(new AddressSelectContext()
                {
                    Address = ipAddressModels,
                });

                return ipAddressModel as IpAddressModel;
            }
        }

        private async Task<bool> IsExistAgent(ConsulClient consulClient, string serverName, string ip, int port)
        {
            var items = await consulClient.Agent.Services();

            var service = items.Response.Values.FirstOrDefault(item =>
                item.Service == serverName && item.Address == ip && item.Port == port);

            return service == null;
        }
    }
}
