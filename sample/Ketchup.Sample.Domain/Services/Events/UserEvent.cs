using System;
using System.Collections.Generic;
using System.Text;
using EventHandler = Ketchup.Core.EventBus.Events.EventHandler;

namespace Ketchup.Sample.Domain.Services.Events
{
    public class UserEvent : EventHandler
    {
        public string Name { get; set; }
        public string Job { get; set; }
    }
}
