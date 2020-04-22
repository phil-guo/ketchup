using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ketchup.Core.EventBus;
using Ketchup.Core.Utilities;
using Ketchup.RabbitMQ.Utilities;

namespace Ketchup.RabbitMQ.Internal.Consumer
{
    public class DefaultConsumeConfigurator : IConsumeConfigurator
    {
        private readonly IEventBus _eventBus;

        public DefaultConsumeConfigurator(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Configure(List<Type> consumers)
        {
            foreach (var consumer in consumers)
            {
                if (consumer.GetTypeInfo().IsGenericType)
                    continue;

                var consumerType = consumer.GetInterfaces()
                    .Where(
                        d =>
                            d.GetTypeInfo().IsGenericType &&
                            d.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    .Select(d => d.GetGenericArguments().Single())
                    .First();

                try
                {
                    FastInvoker<DefaultConsumeConfigurator>.Current.FastInvoke(this, new[] { consumerType, consumer },
                        x => x.ConsumerTo<object, IEventHandler<object>>());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void Unconfigure(List<Type> consumers)
        {
            throw new NotImplementedException();
        }

        protected void ConsumerTo<TEvent, TConsumer>()
            where TConsumer : IEventHandler<TEvent>
            where TEvent : class
        {
            _eventBus.Subscribe<TEvent, TConsumer>
                (() => (TConsumer)ServiceLocator.GetService(typeof(TConsumer)));
        }

        protected void RemoveConsumer<TEvent, TConsumer>()
            where TConsumer : IEventHandler<TEvent>
            where TEvent : class
        {
            _eventBus.Unsubscribe<TEvent, TConsumer>();
        }
    }
}
