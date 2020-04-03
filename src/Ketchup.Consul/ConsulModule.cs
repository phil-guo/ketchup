using System;
using Autofac;
using Ketchup.Consul.ClientProvider;
using Ketchup.Consul.ClientProvider.Implementation;
using Ketchup.Consul.HealthCheck;
using Ketchup.Consul.HealthCheck.Implementation;
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
            var appConfig = new Ketchup.Consul.Configurations.AppConfig();

            UseConsulAddressSelector(builder).UseHealthCheck(builder).UseCounlClientProvider(builder, appConfig);
        }

        public ConsulModule UseConsulAddressSelector(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<ConsulRandomAddressSelector>().As<IConsulAddressSelector>().SingleInstance();
            return this;
        }

        public ConsulModule UseHealthCheck(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<DefaultHealthCheckService>().As<IHealthCheckService>().SingleInstance();
            return this;
        }

        public ConsulModule UseCounlClientProvider(ContainerBuilderWrapper builder, Ketchup.Consul.Configurations.AppConfig appConfig)
        {
            UseCounlClientProvider(builder, provider =>
                new ConsulClientProvider(
                    provider.GetRequiredService<IHealthCheckService>(),
                    provider.GetRequiredService<IConsulAddressSelector>(),
                    provider.GetRequiredService<ILogger<ConsulClientProvider>>())
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
    }
}
