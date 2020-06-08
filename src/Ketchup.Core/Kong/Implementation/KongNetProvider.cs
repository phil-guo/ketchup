using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ketchup.Core.Attributes;
using Ketchup.Core.Configurations;
using Ketchup.Core.Kong.Attribute;
using Kong;
using Kong.Models;

namespace Ketchup.Core.Kong.Implementation
{
    public class KongNetProvider : IKongNetProvider
    {
        private KongClient _client;

        private readonly Type[] _types;

        public KongNetProvider(Type[] types)
        {
            _types = _types = types.Where(type =>
            {
                var typeInfo = type.GetTypeInfo();
                return typeInfo.IsClass && typeInfo.GetCustomAttribute<ServiceAttribute>() != null;
            }).Distinct().ToArray();

            _client = new KongClient(new KongClientOptions(httpClient: new System.Net.Http.HttpClient(),
                host: $"http://{AppConfig.ServerOptions.KongAddress}"));
        }

        public void AddKongSetting()
        {
            foreach (var service in _types)
            {
                foreach (var methodInfo in service.GetMethods())
                {
                    var attribute = methodInfo.GetCustomAttribute<KongRouteAttribute>();

                    if (attribute == null)
                        return;

                    Task.Run(async () =>
                    {
                        var kongService = await _client.Service.Get("gateway");
                        await _client.Route.UpdateOrCreate(new RouteInfo()
                        {
                            Id = Guid.NewGuid(),
                            Name = attribute.Name.ToLower(),
                            Hosts = attribute.Hosts,
                            Methods = attribute.Methods,
                            Protocols = attribute.Protocols,
                            Https_redirect_status_code = attribute.Https_redirect_status_code,
                            Paths = attribute.Paths,
                            Tags = attribute.Tags,
                            Service = new RouteInfo.ServiceId()
                            {
                                Id = (Guid)kongService.Id
                            }
                        });
                    }).Wait();
                }
            }
        }
    }
}
