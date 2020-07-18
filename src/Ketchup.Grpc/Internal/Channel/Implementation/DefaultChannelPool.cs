using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Ketchup.Core.Address;

namespace Ketchup.Grpc.Internal.Channel.Implementation
{
    public class DefaultChannelPool : IChannelPool
    {
        public ConcurrentDictionary<string, ChannelModel> Dictionary = new ConcurrentDictionary<string, ChannelModel>();

        public GrpcChannel GetOrAddChannelPool(IpAddressModel address, GrpcChannelOptions options)
        {
            var key = $"{address.Ip}:{address.Port}";

            var model = Dictionary.GetOrAdd(key, new ChannelModel
            {
                IsUse = true,
                Channel = GrpcChannel.ForAddress($"http://{address.Ip}:{address.Port}", options),
                LastTime = DateTime.Now
            });

            return model.Channel;
        }
    }
}
