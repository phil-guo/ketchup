using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Google.Protobuf.Reflection;
using Grpc.Domain;
using Ketchup.Gateway.Configurations;
using Kong;
using Kong.Models;

namespace Ketchup.Gateway.Internal.Implementation
{
    public class GatewayProvider : IGatewayProvider
    {
        public List<MethodDescriptor> MethodDescriptors { get; set; }

        public Dictionary<string, Type> MapClients { get; set; }

        public GatewayProvider()
        {
            MethodDescriptors = new List<MethodDescriptor>();
            MapClients = new Dictionary<string, Type>();
        }

        public GatewayProvider MapServiceClient(Func<Dictionary<string, Type>> maps)
        {
            MapClients = maps?.Invoke();
            return this;
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

        public GatewayProvider SettingKongService()
        {
            var appConfig = new AppConfig();

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

            return this;
        }
    }
}
