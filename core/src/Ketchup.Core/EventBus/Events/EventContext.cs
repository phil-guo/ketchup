using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.EventBus.Events
{
    public class EventContext
    {
        public object Content { get; set; }

        public long Count { get; set; }

        public string Type { get; set; }
    }
}
