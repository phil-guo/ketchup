using Autofac;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Ketchup.RabbitMQ.Configurations;
using Ketchup.RabbitMQ.Internal.Client;
using Ketchup.RabbitMQ.Internal.Client.Implementation;
using RabbitMQ.Client;

namespace Ketchup.RabbitMQ
{
    public class RabbitMqModule : KernelModule
    {
        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
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
        }
    }
}
