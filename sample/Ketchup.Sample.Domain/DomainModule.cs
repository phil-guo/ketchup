using Ketchup.Core;
using Ketchup.Core.Kong;
using Ketchup.Core.Modules;
using Ketchup.Sample.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Ketchup.Sample.Domain
{
    public class DomainModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            builder.GetInstances<IKongNetProvider>().AddKongSetting();
        }

        public override void MapGrpcService(IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapGrpcService<HelloService>();
            endpointRoute.MapGrpcService<TestService>();
        }
    }
}
