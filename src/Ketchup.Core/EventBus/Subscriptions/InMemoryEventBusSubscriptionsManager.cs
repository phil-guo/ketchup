using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventHandler = Ketchup.Core.EventBus.Events.EventHandler;

namespace Ketchup.Core.EventBus.Subscriptions
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly List<Type> _eventTypes;
        private readonly Dictionary<string, List<Delegate>> _handlers;
        public bool IsEmpty { get; }
        public event EventHandler<(string, string)> OnEventRemoved;

        public InMemoryEventBusSubscriptionsManager()
        {
            _eventTypes = new List<Type>();
            _handlers = new Dictionary<string, List<Delegate>>();
        }

        public void AddSubscription<T, TH>(Func<TH> handler, string consumerName) where TH : IEventHandler<T>
        {
            var key = typeof(T).Name;
            if (!HasSubscriptionsForEvent<T>())
                _handlers.Add(key, new List<Delegate>());

            _handlers[key].Add(handler);
            _eventTypes.Add(typeof(T));
        }

        public void RemoveSubscription<T, TH>() where TH : IEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionsForEvent<T>()
        {
            return HasSubscriptionsForEvent(typeof(T).Name);
        }

        public bool HasSubscriptionsForEvent(string eventName)
        {
            return _handlers.ContainsKey(eventName);
        }

        public Type GetEventTypeByName(string eventName)
        {
            return _eventTypes.SingleOrDefault(item => item.Name == eventName);
        }

        public void Clear()
        {
            _handlers.Clear();
        }

        public IEnumerable<Delegate> GetHandlersForEvent<T>() where T : EventHandler
        {
            return GetHandlersForEvent(typeof(T).Name);
        }

        public IEnumerable<Delegate> GetHandlersForEvent(string eventName)
        {
            return _handlers[eventName];
        }
    }
}
