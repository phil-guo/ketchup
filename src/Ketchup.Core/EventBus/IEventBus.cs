using System;

namespace Ketchup.Core.EventBus
{
    public interface IEventBus
    {
        void Subscribe<T, TH>(Func<TH> handler)
            where TH : IEventHandler<T>;
        void Unsubscribe<T, TH>()
            where TH : IEventHandler<T>;

        void Publish(Events.EventHandler @event);

        event System.EventHandler OnShutdown;
    }
}
