using System;
using System.Collections.Generic;
using Autofac;
using Ketchup.Core;
using Ketchup.Core.EventBus;
using Ketchup.Core.EventBus.Subscriptions;
using Ketchup.Core.Modules;
using Ketchup.RabbitMQ.Configurations;
using Ketchup.RabbitMQ.Internal;
using Ketchup.RabbitMQ.Internal.Client;
using Ketchup.RabbitMQ.Internal.Client.Implementation;
using Ketchup.RabbitMQ.Internal.Consumer;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Ketchup.RabbitMQ
{
    public class RabbitMqModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            var subscriptionAdapt = builder.GetInstances<ISubscriptionAdapt>();

            builder.GetInstances<IEventBus>().OnShutdown += (sender, args) =>
            {
                subscriptionAdapt.Unsubscribe();
            };

            builder.GetInstances<ISubscriptionAdapt>().SubscribeAt();
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<InMemoryEventBusSubscriptionsManager>()
                .As<IEventBusSubscriptionsManager>();
            builder.ContainerBuilder.RegisterType<EventBusRabbitMQ>().As<IEventBus>();
            builder.ContainerBuilder.RegisterType<DefaultConsumeConfigurator>().As<IConsumeConfigurator>();

            builder.ContainerBuilder.Register(provider =>
            {
                var appConfig = new AppConfig();
                var factory = new ConnectionFactory
                {
                    UserName = appConfig.RabbitMq.UserName,
                    HostName = appConfig.RabbitMq.Host,
                    Password = appConfig.RabbitMq.Password,
                    Port = appConfig.RabbitMq.Port
                };
                return new DefaultRabbitMqClientProvider(factory);
            }).As<IRabbitMqClientProvider>();

            AddRabbitMQAdapt(builder);
        }

        private void UseRabbitMQEventAdapt(ContainerBuilderWrapper builder, Func<IServiceProvider, ISubscriptionAdapt> adapt)
        {
            builder.ContainerBuilder.RegisterAdapter(adapt);
        }

        private void AddRabbitMQAdapt(ContainerBuilderWrapper builder)
        {
            UseRabbitMQEventAdapt(builder, provider =>
                new RabbitMqSubscriptionAdapt(
                    provider.GetService<IEnumerable<IEventHandler>>(),
                    provider.GetService<IConsumeConfigurator>()
                )
            );
        }
    }
}
