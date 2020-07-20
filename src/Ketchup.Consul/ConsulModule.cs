using System;
using Autofac;
using Ketchup.Consul.Configurations;
using Ketchup.Consul.Internal.ClientProvider;
using Ketchup.Consul.Internal.ClientProvider.Implementation;
using Ketchup.Consul.Internal.ConsulProvider;
using Ketchup.Consul.Internal.ConsulProvider.Implementation;
using Ketchup.Consul.Internal.HealthCheck.Implementation;
using Ketchup.Consul.Internal.Selector;
using Ketchup.Consul.Internal.Selector.Implementation;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Ketchup.Consul
{
    public class ConsulModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            builder.GetInstances<IConsulProvider>().RegisterConsulAgent();
            builder.GetInstances<IServiceRouteProvider>().AddCustomerServerRoute().Wait();
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            var appConfig = new AppConfig();

            UseConsulAddressSelector(builder)
                .UseConsulClientProvider(builder, appConfig);

            builder.ContainerBuilder.RegisterType<DefaultConsulProvider>().As<IConsulProvider>()
                .WithParameter(new TypedParameter(typeof(AppConfig), appConfig))
                .SingleInstance();

            builder.ContainerBuilder.RegisterType<ServiceRouteProvider>().As<IServiceRouteProvider>()
                .WithParameter(new TypedParameter(typeof(Type[]), ContainerBuilderExtensions.GetTypes()))
                .SingleInstance();
        }

        public override void MapGrpcService(IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapGrpcService<DefaultHealthCheckService>();
        }

        public ConsulModule UseConsulAddressSelector(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<ConsulRandomAddressSelector>()
                .Named<IConsulAddressSelector>(SelectorType.Random.ToString()).SingleInstance();
            builder.ContainerBuilder.RegisterType<PollingAddressSelector>()
                .Named<IConsulAddressSelector>(SelectorType.Polling.ToString()).SingleInstance();
            builder.ContainerBuilder.RegisterType<RandomWeightAddressSelector>()
                .Named<IConsulAddressSelector>(SelectorType.RandomWeight.ToString()).SingleInstance();
            return this;
        }

        public ConsulModule UseConsulClientProvider(ContainerBuilderWrapper builder, Ketchup.Consul.Configurations.AppConfig appConfig)
        {
            UseConsulClientProvider(builder, provider =>
                new ConsulClientProvider
                {
                    AppConfig = appConfig
                });
            return this;
        }

        public ContainerBuilderWrapper UseConsulClientProvider(ContainerBuilderWrapper builder,
            Func<IServiceProvider, IConsulClientProvider> factory)
        {
            builder.ContainerBuilder.RegisterAdapter(factory).InstancePerLifetimeScope();
            return builder;
        }
    }
}
