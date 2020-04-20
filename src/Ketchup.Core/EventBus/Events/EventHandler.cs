using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.EventBus.Events
{
    public class EventHandler
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
