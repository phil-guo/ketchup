using Autofac;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Ketchup.Core.Utilities;
using Ketchup.Gateway.Configurations;
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
            var appConfig = new AppConfig();
            ServiceLocator.GetService<IGatewayProvider>().InitGatewaySetting().SettingKongService(appConfig);
        }

        public override void MapGrpcService(IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapControllers().RequireCors("cors");
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<GatewayProvider>().As<IGatewayProvider>().SingleInstance();
        }
    }
}