using System;
using System.Collections.Generic;
using Autofac;
using Grpc.Domain;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Ketchup.Core.Utilities;
using Ketchup.Gateway.Internal;
using Ketchup.Gateway.Internal.Implementation;
using Ketchup.Permission;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Ketchup.Gateway
{
    public class GatewayModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            ServiceLocator.GetService<IGatewayProvider>()
                .InitGatewaySetting()
                .SettingKongService()
                .MapServiceClient(ClientMaps);
        }

        public override void MapGrpcService(IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapControllers();
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<GatewayProvider>().As<IGatewayProvider>().SingleInstance();
        }

        private Dictionary<string, Type> ClientMaps()
        {
            return new Dictionary<string, Type>()
            {
                //{"SayHello", typeof(RpcTest.RpcTestClient)},
                {"menus.PageSerach", typeof(RpcMenu.RpcMenuClient)},
                {"menus.CreateOrEdit", typeof(RpcMenu.RpcMenuClient)}
            };
        }
    }
}
