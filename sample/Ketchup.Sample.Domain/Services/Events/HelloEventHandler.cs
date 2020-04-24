using System;
using System.Threading.Tasks;
using Ketchup.Core.EventBus.Events;
using Ketchup.RabbitMQ;
using Ketchup.RabbitMQ.Attributes;

namespace Ketchup.Sample.Domain.Services.Events
{
    [QueueConsumer(nameof(HelloEventHandler), QueueConsumerMode.Normal, QueueConsumerMode.Retry)]
    public class HelloEventHandler : BaseEventHandler<UserEvent>
    {
        public override Task Handle(UserEvent @event)
        {
            //Console.WriteLine($"消费1。{@event.Name}---{@event.Job}");

            Console.WriteLine($"消费。{@event.Name}---{@event.Job}");
            throw new Exception();
            return Task.CompletedTask;
        }

        public override Task Handled(EventContext context)
        {
            Console.WriteLine($"调用{context.Count}次。类型:{context.Type}");
            var model = context.Content as UserEvent;
            return Task.CompletedTask;
        }
    }

    //[QueueConsumer(nameof(HelloEventHandler), QueueConsumerMode.Retry)]
    //public class HelloEventRetryHandler : BaseEventHandler<UserEvent>
    //{
    //    public override Task Handle(UserEvent @event)
    //    {
    //        Console.WriteLine($"消费重试。{@event.Name}---{@event.Job}");
    //        return Task.CompletedTask;
    //    }
    //}

    [QueueConsumer(nameof(HelloEventHandler), QueueConsumerMode.Fail)]
    public class HelloEventFailHandler : BaseEventHandler<UserEvent>
    {
        public override Task Handle(UserEvent @event)
        {
            Console.WriteLine($"消费失败。{@event.Name}---{@event.Job}");
            return Task.CompletedTask;
        }
    }
}


