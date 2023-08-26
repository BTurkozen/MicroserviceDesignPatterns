using MassTransit;
using Shared.PaymentEvent;
using System.Threading.Tasks;

namespace Order.Api.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        public Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
