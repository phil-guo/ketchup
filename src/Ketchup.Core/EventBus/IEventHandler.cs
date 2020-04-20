using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ketchup.Core.EventBus
{
    public interface IEventHandler<in TEventHander> : IEventHandler
    {
        Task Handle(TEventHander @event);
    }

    public interface IEventHandler { }
}
