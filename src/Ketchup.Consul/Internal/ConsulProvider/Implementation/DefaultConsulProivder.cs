using System;
using System.Threading.Tasks;
using Ketchup.Consul.Internal.ClientProvider;
using NConsul;
using AppConfig = Ketchup.Consul.Configurations.AppConfig;

namespace Ketchup.Consul.Internal.ConsulProvider.Implementation
{
    public class DefaultConsulProivder : IConsulProvider
    {
        private readonly IConsulClientProvider _consulClientProvider;
        public AppConfig AppConfig { get; set; }

        public DefaultConsulProivder(IConsulClientProvider consulClientProvider)
        {
            _consulClientProvider = consulClientProvider;
        }

        public async Task RegiserGrpcConsul()
        {
            var consulClient = _consulClientProvider.GetConsulClient();

            var config = Core.Configurations.AppConfig.ServerOptions;

            var agent = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString("N"),
                Name = config.ServerName,
                Address = AppConfig.Addresse.Ip,
                Port = AppConfig.Addresse.Port,
                Tags = new[] { $"urlprefix-/{config.ServerName}" }
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
}
