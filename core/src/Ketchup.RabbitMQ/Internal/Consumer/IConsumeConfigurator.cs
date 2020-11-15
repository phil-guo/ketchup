using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.RabbitMQ.Internal.Consumer
{
    public interface IConsumeConfigurator
    {
        void Configure(List<Type> consumers);
        void Unconfigure(List<Type> consumers);
    }
}
