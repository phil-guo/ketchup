using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;

namespace Ketchup.Grpc.Internal.Client
{
    public interface IGrpcClientProvider
    {
        Task<TClient> FindGrpcClient<TClient>(string serverName) where TClient : ClientBase<TClient>;
        Task<TClient> FindGrpcClient<TClient>(string serverName, GrpcChannelOptions options) where TClient : ClientBase<TClient>;
    }
}
