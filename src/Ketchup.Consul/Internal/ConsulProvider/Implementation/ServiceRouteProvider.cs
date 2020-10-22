using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Consul.Internal.ClientProvider;
using Ketchup.Core.Attributes;
using Ketchup.Core.Utilities;
using NConsul;

namespace Ketchup.Consul.Internal.ConsulProvider.Implementation
{
    public class ServiceRouteProvider : IServiceRouteProvider
    {
        private readonly IConsulClientProvider _consulClientProvider;
        private readonly Type[] _types;

        public ServiceRouteProvider(Type[] types)
        {
            _consulClientProvider = ServiceLocator.GetService<IConsulClientProvider>();
            _types = _types = types.Where(type =>
            {
                var typeInfo = type.GetTypeInfo();
                return typeInfo.IsClass && typeInfo.GetCustomAttribute<ServiceAttribute>() != null;
            }).Distinct().ToArray();
        }

        public async Task AddCustomerServerRoute()
        {
            var consulClient = _consulClientProvider.GetConsulClient();
            foreach (var service in _types)
            {
                var serviceAttribute = service.GetCustomAttribute<ServiceAttribute>();

                if (string.IsNullOrEmpty(serviceAttribute.Package) || string.IsNullOrEmpty(serviceAttribute.TypeClientName))
                    continue;

                foreach (var method in service.GetMethods())
                {
                    var attribute = method.GetCustomAttribute<ServiceRouteAttribute>();
                    if (attribute == null)
                        continue;
                    if (string.IsNullOrEmpty(attribute.Name) || string.IsNullOrEmpty(attribute.MethodName))
                        continue;

                    var ketValuePair = new KVPair($"serviceRoutes/{attribute?.Name}.{attribute?.MethodName}")
                    {
                        Value = Encoding.UTF8.GetBytes($"{serviceAttribute?.Package}.{serviceAttribute.Name}+{serviceAttribute?.TypeClientName}")
                    };
                    await consulClient.KV.Put(ketValuePair);
                }
            }
        }

        public async Task<string> GetCustomerServerRoute(string key)
        {
            var consulClient = _consulClientProvider.GetConsulClient();
            //var result = await consulClient.KV.Get($"serviceRoutes/{key}");
            var result = await consulClient.KV.Get($"{key}");
            return Encoding.UTF8.GetString(result.Response?.Value);
        }
    }
}