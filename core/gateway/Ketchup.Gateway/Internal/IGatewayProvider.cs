using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Google.Protobuf.Reflection;
using Ketchup.Gateway.Internal.Implementation;

namespace Ketchup.Gateway.Internal
{
    public interface IGatewayProvider
    {
        List<MessageDescriptor> MessageDescriptors { get; set; }
        List<MethodDescriptor> MethodDescriptors { get; set; }
        ConcurrentDictionary<string, Type> MapClients { get; set; }
        GatewayProvider InitGatewaySetting();
    }
}
