using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ketchup.Core.EventBus.Events
{
    public abstract class BaseEventHandler<TEvent> : IEventHandler<TEvent>
    {
        public abstract Task Handle(TEvent @event);

        public virtual async Task Handled(EventContext context)
        {
            await Task.CompletedTask;
        }

        public virtual async Task FailHandler(EventContext context)
        {
            await Task.CompletedTask;
        }
    }
}
