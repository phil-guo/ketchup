using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Health.V1;

namespace Ketchup.Consul.HealthCheck.Implementation
{
    public class DefaultHealthCheckService : Health.HealthBase
    {
        public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HealthCheckResponse() { Status = HealthCheckResponse.Types.ServingStatus.Serving });
        }
    }
}