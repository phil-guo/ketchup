using System;
using Autofac;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Ketchup.Grpc.Internal.Channel;
using Ketchup.Grpc.Internal.Channel.Implementation;
using Ketchup.Grpc.Internal.Client;
using Ketchup.Grpc.Internal.Client.Implementation;
using Ketchup.Grpc.Internal.Intercept;

namespace Ketchup.Grpc
{
    public class GrpcModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            builder.ContainerBuilder.RegisterType<DefaultGrpcClientProvider>().As<IGrpcClientProvider>().SingleInstance();
            builder.ContainerBuilder.RegisterType<DefaultChannelPool>().As<IChannelPool>().SingleInstance();
            builder.ContainerBuilder.RegisterType<CommandProvider>().As<ICommandProvider>().SingleInstance();
        }
    }
}
