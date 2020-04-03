using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Consul.ClientProvider;
using Ketchup.Consul.Configurations;
using NConsul;

namespace Ketchup.Consul.Internal.Implementation
{
    public class DefaultConsulProivder : IConsulProvider
    {
        private readonly IConsulClientProvider _consulClientProvider;
        private readonly AppConfig _config;

        public DefaultConsulProivder(AppConfig config, IConsulClientProvider consulClientProvider)
        {
            _config = config;
            _consulClientProvider = consulClientProvider;
        }

        public async Task RegiserGrpcConsul(string name, string address, int port)
        {
            var consulClient = _consulClientProvider.GetConsulClient();

            var agent = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString("N"),
                Name = name,
                Address = address,
                Port = port,
                Tags = new[] { $"urlprefix-/{name}" }
            };

            if (_config.Consul.IsHealthCheck)
            {
                var check = new AgentServiceCheck
                {
                    //consul服务注册之后几秒开始检查
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    //10秒检查一次
                    Interval = TimeSpan.FromSeconds(10),
                    GRPC = $"{address}:{port}",
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
