using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Consul.Internal.ClientProvider;
using Ketchup.Consul.Internal.ConsulProvider.Model;
using Ketchup.Core.Attributes;
using Ketchup.Core.Utilities;
using NConsul;

namespace Ketchup.Consul.Internal.ConsulProvider.Implementation
{
    public class ServiceRouterProvider : IServiceRouteProvider
    {
        private readonly IConsulClientProvider _consulClientProvider;
        private readonly Type[] _types;

        public ServiceRouterProvider(Type[] types, IConsulClientProvider consulClientProvider)
        {
            _consulClientProvider = consulClientProvider;
            _types = _types = types.Where(type =>
            {
                var typeInfo = type.GetTypeInfo();
                return typeInfo.IsClass && typeInfo.GetCustomAttribute<ServiceAttribute>() != null;
            }).Distinct().ToArray();
        }

        public async Task AddCustomerServerRouter()
        {
            var config = Core.Configurations.AppConfig.ServerOptions;
            var consulClient = _consulClientProvider.GetConsulClient();
            foreach (var service in _types)
            {
                var serviceAttribute = service.GetCustomAttribute<ServiceAttribute>();

                if (string.IsNullOrEmpty(serviceAttribute?.Package) || string.IsNullOrEmpty(serviceAttribute?.TypeClientName))
                    continue;

                foreach (var method in service.GetMethods())
                {
                    var attribute = method.GetCustomAttribute<ServiceRouterAttribute>();
                    if (attribute == null)
                        continue;
                    if (string.IsNullOrEmpty(attribute.Name) || string.IsNullOrEmpty(attribute.MethodName))
                        continue;

                    var ketValuePair = new KVPair($"serviceRoutes/{config?.Name}/{attribute?.Name}/{attribute?.MethodName}")
                    {
                        Value = Encoding.UTF8.GetBytes($"{serviceAttribute?.Package}.{serviceAttribute.Name}+{serviceAttribute?.TypeClientName}")
                    };
                    await consulClient.KV.Put(ketValuePair);
                }
            }
        }

        public async Task<string> GetCustomerServerRouter(string key)
        {
            var consulClient = _consulClientProvider.GetConsulClient();
            var result = await consulClient.KV.Get($"serviceRoutes/{key}");
            return Encoding.UTF8.GetString(result.Response?.Value ?? Array.Empty<byte>());
        }
    }
}