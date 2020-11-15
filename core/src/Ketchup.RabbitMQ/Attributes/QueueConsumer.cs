using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ketchup.RabbitMQ.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class QueueConsumer : Attribute
    {
        public string Name { get; private set; }

        public QueueConsumerMode[] Types { get; private set; }

        public QueueConsumer(string name, params QueueConsumerMode[] types)
        {
            Name = name;
            Types = types.Any() ? types : new[] { QueueConsumerMode.Normal };
        }
    }
}
