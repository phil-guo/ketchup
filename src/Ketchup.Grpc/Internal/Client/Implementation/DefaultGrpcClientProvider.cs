using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Ketchup.Consul.Internal.ConsulProvider;

namespace Ketchup.Grpc.Internal.Client.Implementation
{
    public class DefaultGrpcClientProvider : IGrpcClientProvider
    {
        private readonly IConsulProvider _consulProvider;

        public DefaultGrpcClientProvider(IConsulProvider consulProvider)
        {
            _consulProvider = consulProvider;
        }

        public async Task<TClient> FindGrpcClient<TClient>(string serverName, GrpcChannelOptions options)
            where TClient : ClientBase<TClient>
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var address = await GetChannelAddress(serverName);

            //todo 从grcp通道池中获取通道
            var channel = GrpcChannel.ForAddress(address, options);
            return Activator.CreateInstance(typeof(TClient), channel) as TClient;
        }

        public async Task<TClient> FindGrpcClient<TClient>(string serverName)
            where TClient : ClientBase<TClient>
        {
            return await FindGrpcClient<TClient>(serverName, new GrpcChannelOptions());
        }

        private async ValueTask<string> GetChannelAddress(string serverName)
        {
            var address = await _consulProvider.FindServiceEntry(serverName);
            return $"http://{address.Ip}:{address.Port}";
        }
    }
}
