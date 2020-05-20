using Ketchup.Core.Modules;
using Ketchup.Sample.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Ketchup.Sample.Domain
{
    public class DomainModule : KernelModule
    {
        public override void MapGrpcService(IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapGrpcService<HelloService>();
        }
    }
}
