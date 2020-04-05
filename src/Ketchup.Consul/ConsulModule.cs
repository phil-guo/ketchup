using System;
using Autofac;
using Ketchup.Consul.Configurations;
using Ketchup.Consul.HealthCheck;
using Ketchup.Consul.Internal;
using Ketchup.Consul.Internal.ClientProvider;
using Ketchup.Consul.Internal.ClientProvider.Implementation;
using Ketchup.Consul.Internal.ConsulProvider;
using Ketchup.Consul.Internal.ConsulProvider.Implementation;
using Ketchup.Consul.Internal.Selector;
using Ketchup.Consul.Internal.Selector.Implementation;
using Ketchup.Consul.Selector;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ketchup.Consul
{
    public class ConsulModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            builder.GetInstances<IConsulProvider>().RegiserGrpcConsul();
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            var appConfig = new AppConfig();

            UseConsulAddressSelector(builder)
                .UseCounlClientProvider(builder, appConfig)
                .UseConsul(builder, appConfig);
        }

        public ConsulModule UseConsul(ContainerBuilderWrapper builder, AppConfig appConfig)
        {
            UseConul(builder, provider =>
                new DefaultConsulProivder(
                    provider.GetRequiredService<IConsulClientProvider>())
                {
                    AppConfig = appConfig
                });
            //builder.ContainerBuilder.RegisterType<DefaultConsulProivder>().As<IConsulProvider>().SingleInstance();
            return this;
        }

        public ConsulModule UseConsulAddressSelector(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<ConsulRandomAddressSelector>().As<IConsulAddressSelector>().SingleInstance();
            return this;
        }

        public ConsulModule UseCounlClientProvider(ContainerBuilderWrapper builder, Ketchup.Consul.Configurations.AppConfig appConfig)
        {
            UseCounlClientProvider(builder, provider =>
                new ConsulClientProvider(
                    provider.GetRequiredService<IConsulAddressSelector>())
                {
                    AppConfig = appConfig
                }
                );
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
