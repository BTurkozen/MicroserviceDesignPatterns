using MassTransit;
using Shared.PaymentEvent;
using System.Threading.Tasks;

namespace Order.Api.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        public Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
