using System;
using System.Threading.Tasks;
using Ketchup.Core.EventBus.Events;
//using Ketchup.RabbitMQ;
//using Ketchup.RabbitMQ.Attributes;

namespace Ketchup.Sample.Domain.Services.Events
{
    //[QueueConsumer(nameof(HelloEventHandler), QueueConsumerMode.Normal, QueueConsumerMode.Retry, QueueConsumerMode.Fail)]
    public partial class HelloEventHandler : BaseEventHandler<UserEvent>
    {
        public override Task Handle(UserEvent @event)
        {

            Console.WriteLine($"消费。{@event.Name}---{@event.Job}");
            throw new Exception();
        }

        public override Task Handled(EventContext context)
        {
            Console.WriteLine($"调用{context.Count}次。类型:{context.Type}");
            var model = context.Content as UserEvent;
            return Task.CompletedTask;
        }


        public override Task FailHandler(EventContext context)
        {
            Console.WriteLine($"调用{context.Count}次。私信队列方法 ，方法,类型:{context.Type}");
            return base.FailHandler(context);
        }
    }
}


