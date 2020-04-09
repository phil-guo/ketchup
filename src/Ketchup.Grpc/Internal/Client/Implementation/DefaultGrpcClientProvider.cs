using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Ketchup.Consul.Internal.ConsulProvider;
using Ketchup.Core.Address;
using Ketchup.Grpc.Internal.Channel;

namespace Ketchup.Grpc.Internal.Client.Implementation
{
    public class DefaultGrpcClientProvider : IGrpcClientProvider
    {
        private readonly IConsulProvider _consulProvider;
        private readonly IChannelPool _channelPool;

        public DefaultGrpcClientProvider(IConsulProvider consulProvider, IChannelPool channelPool)
        {
            _consulProvider = consulProvider;
            _channelPool = channelPool;
        }

        public async Task<TClient> FindGrpcClient<TClient>(string serverName, GrpcChannelOptions options)
            where TClient : ClientBase<TClient>
        {
            var address = await GetChannelAddress(serverName);

            var channel = _channelPool.GetOrAddChannelPool(address, options);
            return Activator.CreateInstance(typeof(TClient), channel) as TClient;
        }

        public async Task<TClient> FindGrpcClient<TClient>(string serverName)
            where TClient : ClientBase<TClient>
        {
            return await FindGrpcClient<TClient>(serverName, new GrpcChannelOptions());
        }

        private async ValueTask<IpAddressModel> GetChannelAddress(string serverName)
        {
            var address = await _consulProvider.FindServiceEntry(serverName);
            return address;
        }
    }
}
