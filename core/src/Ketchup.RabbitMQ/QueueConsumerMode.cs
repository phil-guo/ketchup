using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.RabbitMQ
{
    public enum QueueConsumerMode
    {
        Normal = 0,
        Retry,
        Fail,
    }
}
