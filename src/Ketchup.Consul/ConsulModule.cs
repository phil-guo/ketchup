using System;
using Autofac;
using Ketchup.Consul.ClientProvider;
using Ketchup.Consul.ClientProvider.Implementation;
using Ketchup.Consul.Configurations;
using Ketchup.Consul.HealthCheck;
using Ketchup.Consul.Internal;
using Ketchup.Consul.Internal.Implementation;
using Ketchup.Consul.Selector;
using Ketchup.Consul.Selector.Implementation;
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
            base.Initialize(builder);
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
                new DefaultConsulProivder(appConfig, provider.GetRequiredService<IConsulClientProvider>()));
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
