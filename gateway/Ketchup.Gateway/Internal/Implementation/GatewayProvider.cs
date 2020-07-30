using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Google.Protobuf.Reflection;
using Ketchup.Core.Kong.Models;
using Ketchup.Gateway.Configurations;
using Kong;
using Kong.Models;

namespace Ketchup.Gateway.Internal.Implementation
{
    public class GatewayProvider : IGatewayProvider
    {
        public const string KongAuthName = "ketchupAuth";

        public List<MethodDescriptor> MethodDescriptors { get; set; }

        public ConcurrentDictionary<string, Type> MapClients { get; set; }

        public GatewayProvider()
        {
            MethodDescriptors = new List<MethodDescriptor>();
            MapClients = new ConcurrentDictionary<string, Type>();
        }
        public GatewayProvider InitGatewaySetting()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes().Where(type => type.Name.EndsWith("Reflection")).ToArray();

            foreach (var type in types)
            {
                var property = type.GetProperties(BindingFlags.Static | BindingFlags.Public)
                    .FirstOrDefault(t => t.Name == "Descriptor");

                foreach (var fileDescriptorService in (property.GetValue(null) as FileDescriptor).Services)
                {
                    foreach (var methodDescriptor in fileDescriptorService.Methods)
                    {
                        MethodDescriptors.Add(methodDescriptor);
                    }
                }
            }

            return this;
        }

        public GatewayProvider SettingKongService(AppConfig appConfig)
        {
            if (string.IsNullOrEmpty(appConfig.Gateway.KongAddress))
                return this;

            var options = new KongClientOptions(httpClient: new System.Net.Http.HttpClient(), host: $"http://{appConfig.Gateway.KongAddress}");
            var client = new KongClient(options);

            var service = new ServiceInfo
            {
                Name = appConfig.Gateway.Name,
                Id = Guid.NewGuid(),
                Port = appConfig.Gateway.Port,
                Protocol = appConfig.Gateway.Protocol,
                Path = appConfig.Gateway.Path,
                Tags = new string[] { appConfig.Gateway.Name },
                Host = appConfig.Gateway.Address,
                Connect_timeout = 60000,
                Read_timeout = 60000,
                Write_timeout = 60000,
            };

            client.Service?.UpdateOrCreate(service);

            SettingAuthSerice(appConfig, client);

            return this;
        }

        private void SettingAuthSerice(AppConfig appConfig, KongClient client)
        {
            if (!appConfig.Gateway.EnableAuth)
                return;

            if (string.IsNullOrEmpty(appConfig.Gateway.JwtAuth))
                return;

            var authService = new ServiceInfo()
            {
                Name = KongAuthName,
                Id = Guid.NewGuid(),
                Port = appConfig.Gateway.Port,
                Protocol = appConfig.Gateway.Protocol,
                Path = "/",
                Tags = new string[] { "auth" },
                Host = appConfig.Gateway.Address,
                Connect_timeout = 60000,
                Read_timeout = 60000,
                Write_timeout = 60000,
            };
            client.Service?.UpdateOrCreate(authService);

            SettingAuthRoute(client, appConfig);

            SettingConsumer(client, appConfig);
        }

        private void SettingAuthRoute(KongClient client, AppConfig appConfig)
        {
            Task.Run(async () =>
            {
                var kongService = await client.Service.Get(KongAuthName);
                if (kongService == null)
                    return;

                await client.Route.UpdateOrCreate(new RouteInfo()
                {
                    Id = Guid.NewGuid(),
                    Name = "auth",
                    Methods = new[] { "POST", "OPTIONS" },
                    Protocols = new[] { "http" },
                    Https_redirect_status_code = 426,
                    Paths = new[] { appConfig.Gateway.JwtAuth },
                    Tags = new[] { "token" },
                    Service = new RouteInfo.ServiceId()
                    {
                        Id = (Guid)kongService.Id
                    }
                });
            }).Wait();
        }

        public void SettingConsumer(KongClient client, AppConfig appConfig)
        {
            if (!appConfig.Gateway.EnableAuth)
                return;

            Task.Run(async () =>
            {
                Consumer consumer = new Consumer()
                {
                    Id = Guid.NewGuid(),
                    UserName = "ketchup",
                    Custom_id = "ketchup",
                    Tags = new string[] { "ketchup.zero" }
                };
                await client.Consumer.UpdateOrCreate(consumer);
            }).Wait();
        }
    }
}
