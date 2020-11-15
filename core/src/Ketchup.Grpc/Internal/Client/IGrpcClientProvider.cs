using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;

namespace Ketchup.Grpc.Internal.Client
{
    public interface IGrpcClientProvider
    {
        Task<TClient> FindGrpcClient<TClient>(string serverName) where TClient : ClientBase<TClient>;
        Task<TClient> FindGrpcClient<TClient>(string serverName, Interceptor[] interceptor) where TClient : ClientBase<TClient>;
        Task<TClient> FindGrpcClient<TClient>(string serverName, GrpcChannelOptions options, Interceptor[] interceptor = null) where TClient : ClientBase<TClient>;
        Task<object> GetClientAsync(string serverName, Type clientType);
    }
}
