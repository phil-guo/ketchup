using Autofac;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Ketchup.Core.Utilities;
using Ketchup.Gateway.Internal;
using Ketchup.Gateway.Internal.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Ketchup.Gateway
{
    public class GatewayModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            ServiceLocator.GetService<IGatewayProvider>().InitGatewaySetting().MapServiceClient();
        }

        public override void MapGrpcService(IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapControllers();
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<GatewayProvider>().As<IGatewayProvider>().SingleInstance();
        }
    }
}
