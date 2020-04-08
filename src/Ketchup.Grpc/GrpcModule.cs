using System;
using Autofac;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Ketchup.Grpc.Internal.Client;
using Ketchup.Grpc.Internal.Client.Implementation;

namespace Ketchup.Grpc
{
    public class GrpcModule : KernelModule
    {
        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<DefaultGrpcClientProvider>().As<IGrpcClientProvider>().SingleInstance();
        }
    }
}
