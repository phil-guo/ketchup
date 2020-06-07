using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Ketchup.Consul.Internal.ConsulProvider;
using Ketchup.Core.Address;
using Ketchup.Grpc.Internal.Channel;
using Ketchup.Grpc.Internal.Intercept;

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

        public async Task<TClient> FindGrpcClient<TClient>(string serverName, Interceptor[] interceptor)
            where TClient : ClientBase<TClient>
        {
            return await FindGrpcClient<TClient>(serverName, new GrpcChannelOptions(), interceptor);
        }

        public async Task<TClient> FindGrpcClient<TClient>(string serverName)
            where TClient : ClientBase<TClient>
        {
            return await FindGrpcClient<TClient>(serverName, new GrpcChannelOptions());
        }

        public async Task<TClient> FindGrpcClient<TClient>(string serverName, GrpcChannelOptions options, Interceptor[] interceptor = null)
            where TClient : ClientBase<TClient>
        {
            var address = await GetChannelAddress(serverName);

            var channel = _channelPool.GetOrAddChannelPool(address, options);

            if (interceptor == null)
                return Activator.CreateInstance(typeof(TClient), channel) as TClient;

            var invoker = channel.Intercept(interceptor);
            return Activator.CreateInstance(typeof(TClient), invoker) as TClient;
        }

        public async Task<object> GetClientAsync(string serverName, Type clientType)
        {
            var address = await GetChannelAddress(serverName);

            var channel = _channelPool.GetOrAddChannelPool(address, new GrpcChannelOptions());

            return Activator.CreateInstance(clientType, channel);
        }

        private async ValueTask<IpAddressModel> GetChannelAddress(string serverName)
        {
            var address = await _consulProvider.FindServiceEntry(serverName);
            return address;
        }
    }
}
