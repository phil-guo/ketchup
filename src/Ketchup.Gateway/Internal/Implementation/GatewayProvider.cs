using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Google.Protobuf.Reflection;
using Grpc.Domain;
using Ketchup.Gateway.Internal.Attribute;
using Newtonsoft.Json;

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

        public GatewayProvider MapServiceClient()
        {
            MapClients.Add("SayHello", typeof(RpcTest.RpcTestClient));

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
    }
}
