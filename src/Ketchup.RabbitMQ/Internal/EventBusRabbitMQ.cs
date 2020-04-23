using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Core.EventBus;
using Ketchup.Core.EventBus.Events;
using Ketchup.Core.Utilities;
using Ketchup.RabbitMQ.Attributes;
using Ketchup.RabbitMQ.Configurations;
using Ketchup.RabbitMQ.Internal.Client;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using EventHandler = Ketchup.Core.EventBus.Events.EventHandler;

namespace Ketchup.RabbitMQ.Internal
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly IRabbitMqClientProvider _rabbitMqClient;
        private readonly IDictionary<Tuple<string, QueueConsumerMode>, IModel> _consumerChannels;
        private readonly IEventBusSubscriptionsManager _subscriptionsManager;
        private readonly ConcurrentDictionary<ValueTuple<Type, string>, object> _initializers;
        private readonly int _retryCount;
        private readonly int _rollbackCount;
        private readonly int _messageTTL;
        string BROKER_NAME = "ketchup";

        public EventBusRabbitMQ(IRabbitMqClientProvider rabbitMqClient, IEventBusSubscriptionsManager subscriptionsManager)
        {
            var appConfig = new AppConfig();

            _retryCount = appConfig.RabbitMq.RetryCount;
            _rollbackCount = appConfig.RabbitMq.FailCount;
            _messageTTL = appConfig.RabbitMq.MessageTTL;

            _rabbitMqClient = rabbitMqClient;
            _subscriptionsManager = subscriptionsManager;
            _consumerChannels = new Dictionary<Tuple<string, QueueConsumerMode>, IModel>();
            _initializers = new ConcurrentDictionary<ValueTuple<Type, string>, object>();
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
            var attribute = typeof(TH).GetCustomAttribute<QueueConsumer>();
            if (attribute == null)
                return;

            var types = Enum.GetNames(typeof(QueueConsumerMode));

            if (!_rabbitMqClient.IsConnected)
                _rabbitMqClient.TryConnect();

            using (var channel = _rabbitMqClient.CreateModel())
            {
                foreach (var item in types)
                {
                    var type = Enum.Parse<QueueConsumerMode>(item);

                    var queueName = type == QueueConsumerMode.Normal
                        ? attribute.Name
                        : $"{attribute.Name}@{type.ToString()}";

                    var exchange = type == QueueConsumerMode.Normal ? BROKER_NAME : $"{BROKER_NAME}@{type.ToString()}";

                    var key = new Tuple<string, QueueConsumerMode>(queueName, type);
                    if (_consumerChannels.ContainsKey(key))
                    {
                        _consumerChannels[key].Close();
                        _consumerChannels.Remove(key);
                    }

                    _consumerChannels.Add(key, CreateConsumerChannel(attribute, eventName, type));
                    channel.QueueBind(queue: queueName, exchange: exchange, routingKey: eventName);
                }
            }

            if (!_subscriptionsManager.HasSubscriptionsForEvent<T>())
                _subscriptionsManager.AddSubscription<T, TH>(handler, attribute.Name);
        }

        public void Unsubscribe<T, TH>() where TH : IEventHandler<T>
        {
        }

        public event System.EventHandler OnShutdown;

        public void Dispose()
        {
            foreach (var key in _consumerChannels.Keys)
            {
                if (_consumerChannels[key] != null)
                {
                    _consumerChannels[key].Dispose();
                }
            }
            _subscriptionsManager.Clear();
        }

        private IModel CreateConsumerChannel(QueueConsumer queueConsumer, string routeKey,
            QueueConsumerMode type)
        {
            IModel result = null;

            switch (type)
            {
                case QueueConsumerMode.Retry:
                    result = CreateRetryConsumerChannel(queueConsumer.Name, routeKey, type);
                    break;
                case QueueConsumerMode.Fail:
                    result = CreateFailConsumerChannel(queueConsumer.Name, type);
                    break;
                default:
                    result = CreateConsumerChannel(queueConsumer.Name, type);
                    break;
            }

            return result;
        }

        private IModel CreateConsumerChannel(string queueName, QueueConsumerMode type)
        {
            if (!_rabbitMqClient.IsConnected)
                _rabbitMqClient.TryConnect();

            var channel = _rabbitMqClient.CreateModel();
            channel.ExchangeDeclare(exchange: BROKER_NAME,
                type: ExchangeType.Direct);
            channel.QueueDeclare(queueName, true, false, false, null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                await ProcessEvent(eventName, ea.Body, type, ea.BasicProperties);
                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.Close();

            return channel;
        }

        private IModel CreateRetryConsumerChannel(string queueName, string routeKey, QueueConsumerMode type)
        {
            if (!_rabbitMqClient.IsConnected)
                _rabbitMqClient.TryConnect();

            IDictionary<String, Object> arguments = new Dictionary<String, Object>();
            arguments.Add("x-dead-letter-exchange", $"{BROKER_NAME}@{QueueConsumerMode.Fail.ToString()}");
            arguments.Add("x-message-ttl", _messageTTL);
            arguments.Add("x-dead-letter-routing-key", routeKey);
            var channel = _rabbitMqClient.CreateModel();
            var retryQueueName = $"{queueName}@{type.ToString()}";
            channel.ExchangeDeclare(exchange: $"{BROKER_NAME}@{type.ToString()}",
                type: ExchangeType.Direct);
            channel.QueueDeclare(retryQueueName, true, false, false, arguments);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                await ProcessEvent(eventName, ea.Body, type, ea.BasicProperties);
                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.Close();

            return channel;
        }

        private IModel CreateFailConsumerChannel(string queueName, QueueConsumerMode type)
        {
            if (!_rabbitMqClient.IsConnected)
                _rabbitMqClient.TryConnect();

            var channel = _rabbitMqClient.CreateModel();
            channel.ExchangeDeclare(exchange: $"{BROKER_NAME}@{type.ToString()}",
                type: ExchangeType.Direct);
            var failQueueName = $"{queueName}@{type.ToString()}";
            channel.QueueDeclare(failQueueName, true, false, false, null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                await ProcessEvent(eventName, ea.Body, type, ea.BasicProperties);
                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.Close();
            return channel;
        }

        private async Task ProcessEvent(string eventName, ReadOnlyMemory<byte> body, QueueConsumerMode mode, IBasicProperties properties)
        {
            var message = Encoding.UTF8.GetString(body.ToArray());

            var eventType = _subscriptionsManager.GetEventTypeByName(eventName);
            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

            var handlers = _subscriptionsManager.GetHandlersForEvent(eventName);
            var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

            foreach (var item in handlers)
            {
                var handler = item.DynamicInvoke();
                long retryCount = 1;
                try
                {
                    var fastInvoker = GetHandler($"{concreteType.FullName}.Handle", concreteType.GetMethod("Handle"));
                    await (Task)fastInvoker(handler, new object[] { integrationEvent });
                }
                catch
                {
                    if (!_rabbitMqClient.IsConnected)
                        _rabbitMqClient.TryConnect();


                    retryCount = GetRetryCount(properties);
                    using (var channel = _rabbitMqClient.CreateModel())
                    {
                        if (retryCount > _retryCount)
                        {
                            // 重试次数大于设置次数，则自动加入到死信队列
                            var rollbackCount = retryCount - _retryCount;
                            if (rollbackCount < _rollbackCount)
                            {
                                IDictionary<String, Object> headers = new Dictionary<String, Object>();
                                if (!headers.ContainsKey("x-orig-routing-key"))
                                    headers.Add("x-orig-routing-key", GetOrigRoutingKey(properties, eventName));
                                retryCount = rollbackCount;
                                channel.BasicPublish($"{BROKER_NAME}@{QueueConsumerMode.Fail.ToString()}", eventName,
                                    CreateOverrideProperties(properties, headers), body);
                            }
                        }
                        else
                        {
                            IDictionary<String, Object> headers = properties.Headers;
                            if (headers == null)
                            {
                                headers = new Dictionary<String, Object>();
                            }

                            if (!headers.ContainsKey("x-orig-routing-key"))
                                headers.Add("x-orig-routing-key", GetOrigRoutingKey(properties, eventName));
                            channel.BasicPublish($"{BROKER_NAME}@{QueueConsumerMode.Retry.ToString()}", eventName,
                                CreateOverrideProperties(properties, headers), body);
                        }
                    }
                }
                finally
                {
                    var baseConcreteType = typeof(BaseEventHandler<>).MakeGenericType(eventType);
                    if (handler.GetType().BaseType == baseConcreteType)
                    {
                        var context = new EventContext()
                        {
                            Content = integrationEvent,
                            Count = retryCount,
                            Type = mode.ToString()
                        };
                        var fastInvoker = GetHandler($"{baseConcreteType.FullName}.Handled", baseConcreteType.GetMethod("Handled"));
                        await (Task)fastInvoker(handler, new object[] { context });
                    }
                }
            }
        }

        private IBasicProperties CreateOverrideProperties(IBasicProperties properties,
            IDictionary<String, Object> headers)
        {
            properties.ContentType ??= "";
            properties.ContentEncoding ??= "";
            properties.Headers = properties.Headers;
            if (properties.Headers == null)
                properties.Headers = new Dictionary<string, object>();
            foreach (var key in headers.Keys)
            {
                if (!properties.Headers.ContainsKey(key))
                    properties.Headers.Add(key, headers[key]);
            }
            properties.DeliveryMode = properties.DeliveryMode;
            return properties;
        }

        private String GetOrigRoutingKey(IBasicProperties properties,
            String defaultValue)
        {
            String routingKey = defaultValue;
            if (properties != null)
            {
                IDictionary<String, Object> headers = properties.Headers;
                if (headers != null && headers.Count > 0)
                {
                    if (headers.ContainsKey("x-orig-routing-key"))
                    {
                        routingKey = headers["x-orig-routing-key"].ToString();
                    }
                }
            }
            return routingKey;
        }

        private long GetRetryCount(IBasicProperties properties)
        {
            long retryCount = 1L;
            try
            {
                if (properties != null)
                {

                    IDictionary<String, Object> headers = properties.Headers;
                    if (headers != null)
                    {
                        if (headers.ContainsKey("x-death"))
                        {
                            retryCount = GetRetryCount(properties, headers);
                        }
                        else
                        {
                            var death = new Dictionary<string, object>();
                            death.Add("count", retryCount);
                            headers.Add("x-death", death);
                        }
                    }
                }
            }
            catch { }
            return retryCount;
        }

        private long GetRetryCount(IBasicProperties properties, IDictionary<String, Object> headers)
        {
            var retryCount = 1L;
            if (headers["x-death"] is List<object>)
            {
                var deaths = (List<object>)headers["x-death"];
                if (deaths.Count > 0)
                {
                    IDictionary<String, Object> death = deaths[0] as Dictionary<String, Object>;
                    retryCount = (long)death["count"];
                    death["count"] = ++retryCount;
                }
            }
            else
            {
                Dictionary<String, Object> death = (Dictionary<String, Object>)headers["x-death"];
                if (death != null)
                {
                    retryCount = (long)death["count"];
                    death["count"] = ++retryCount;
                    properties.Headers = headers;
                }
            }
            return retryCount;
        }

        private FastInvoke.FastInvokeHandler GetHandler(string key, MethodInfo method)
        {
            _initializers.TryGetValue(ValueTuple.Create((Type)null, key == null ? null : key.ToString()), out var result);

            if (result == null)
            {
                result = FastInvoke.GetMethodInvoker(method);
                _initializers.GetOrAdd(ValueTuple.Create((Type)null, key), result);
            }
            return result as FastInvoke.FastInvokeHandler;
        }

        private void Connection_OnEventShutDown(object sender, ShutdownEventArgs reason)
        {
            OnShutdown(this, new EventArgs());
        }
    }
}
