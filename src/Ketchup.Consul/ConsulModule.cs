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
using Microsoft.Extensions.DependencyInjection;

namespace Ketchup.Consul
{
    public class ConsulModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            builder.GetInstances<IConsulProvider>().RegiserConsulAgent();
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            var appConfig = new AppConfig();

            UseConsulAddressSelector(builder)
                .UseCounlClientProvider(builder, appConfig)
                .UseConsul(builder, appConfig);
        }

        public override void MapGrpcService(IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapGrpcService<DefaultHealthCheckService>();
        }


        public ConsulModule UseConsul(ContainerBuilderWrapper builder, AppConfig appConfig)
        {
            UseConul(builder, provider =>
                new DefaultConsulProivder(
                    provider.GetRequiredService<IConsulClientProvider>())
                {
                    AppConfig = appConfig
                });
            return this;
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

        public ConsulModule UseCounlClientProvider(ContainerBuilderWrapper builder, Ketchup.Consul.Configurations.AppConfig appConfig)
        {
            UseCounlClientProvider(builder, provider =>
                new ConsulClientProvider
                {
                    AppConfig = appConfig
                });
            return this;
        }

        public ContainerBuilderWrapper UseCounlClientProvider(ContainerBuilderWrapper builder,
            Func<IServiceProvider, IConsulClientProvider> factory)
        {
            builder.ContainerBuilder.RegisterAdapter(factory).InstancePerLifetimeScope();
            return builder;
        }

        public ContainerBuilderWrapper UseConul(ContainerBuilderWrapper builder,
            Func<IServiceProvider, IConsulProvider> factory)
        {
            builder.ContainerBuilder.RegisterAdapter(factory).InstancePerLifetimeScope();
            return builder;
        }
    }
}
