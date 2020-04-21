using System;
using System.Collections.Generic;

namespace Ketchup.Core.EventBus
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }

        event EventHandler<ValueTuple<string, string>> OnEventRemoved;
        void AddSubscription<T, TH>(Func<TH> handler, string consumerName)
            where TH : IEventHandler<T>;

        void RemoveSubscription<T, TH>()
            where TH : IEventHandler<T>;
        bool HasSubscriptionsForEvent<T>();
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<Delegate> GetHandlersForEvent<T>() where T : Events.EventHandler;
        IEnumerable<Delegate> GetHandlersForEvent(string eventName);
    }
}
