using System;
using System.Threading.Tasks;
using Ketchup.Core.EventBus.Events;
using Ketchup.RabbitMQ.Attributes;

namespace Ketchup.Sample.Domain.Services.Events
{
    [QueueConsumer("HelloEventHandler")]
    public class HelloEventHandler : BaseEventHandler<UserEvent>
    {
        public override Task Handle(UserEvent @event)
        {
            Console.WriteLine($"消费1。");

            //Console.WriteLine($"消费1失败。");
            //throw new Exception();
            return Task.CompletedTask;
        }

        public override Task Handled(EventContext context)
        {
            Console.WriteLine($"调用{context.Count}次。类型:{context.Type}");
            var model = context.Content as UserEvent;
            return Task.CompletedTask;
        }
    }
}
