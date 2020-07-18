using System;
using System.Collections.Generic;
using System.Text;
using Ketchup.Core.EventBus;
using Ketchup.RabbitMQ.Internal.Consumer;

namespace Ketchup.RabbitMQ.Internal
{
    public class RabbitMqSubscriptionAdapt : ISubscriptionAdapt
    {
        private readonly IEnumerable<IEventHandler> _handlers;
        private readonly IConsumeConfigurator _consumeConfigurator;

        public RabbitMqSubscriptionAdapt(IEnumerable<IEventHandler> handlers, IConsumeConfigurator consumeConfigurator)
        {
            _handlers = handlers;
            _consumeConfigurator = consumeConfigurator;
        }

        public void SubscribeAt()
        {
            _consumeConfigurator.Configure(GetQueueConsumers());
        }

        public void Unsubscribe()
        {
            _consumeConfigurator.Unconfigure(GetQueueConsumers());
        }

        private List<Type> GetQueueConsumers()
        {
            var result = new List<Type>();
            foreach (var consumer in _handlers)
            {
                var type = consumer.GetType();
                result.Add(type);
            }
            return result;
        }
    }
}
