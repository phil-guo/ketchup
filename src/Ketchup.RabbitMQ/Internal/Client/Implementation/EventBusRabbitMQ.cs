using System;
using System.Text;
using Ketchup.Core.EventBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using EventHandler = Ketchup.Core.EventBus.Events.EventHandler;

namespace Ketchup.RabbitMQ.Internal.Client.Implementation
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly IRabbitMqClientProvider _rabbitMqClient;
        string BROKER_NAME = "ketchup";

        public EventBusRabbitMQ(IRabbitMqClientProvider rabbitMqClient)
        {
            _rabbitMqClient = rabbitMqClient;
            _rabbitMqClient.OnRabbitConnectionShutdown += Connection_OnEventShutDown;
        }

        public void Publish(EventHandler @event)
        {
            if (!_rabbitMqClient.IsConnected)
                _rabbitMqClient.TryConnect();

            using (var channel = _rabbitMqClient.CreateModel())
            {
                var eventName = @event.GetType().Name;

                channel.ExchangeDeclare(exchange: BROKER_NAME,
                    type: ExchangeType.Direct);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: BROKER_NAME,
                    routingKey: eventName,
                    basicProperties: properties,
                    body: body);
            }
        }

        public void Subscribe<T, TH>(Func<TH> handler) where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
        }

        public void Unsubscribe<T, TH>() where TH : IEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public event System.EventHandler OnShutdown;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void Connection_OnEventShutDown(object sender, ShutdownEventArgs reason)
        {
            OnShutdown(this, new EventArgs());
        }
    }
}
