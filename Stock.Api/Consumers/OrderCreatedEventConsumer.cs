using MassTransit;
using Shared.OrderEvents;
using System.Threading.Tasks;

namespace Stock.Api.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        public Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
