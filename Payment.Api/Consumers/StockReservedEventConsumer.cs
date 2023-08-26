using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.PaymentEvent;
using Shared.StockEvents;
using System.Threading.Tasks;

namespace Payment.Api.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly ILogger<StockReservedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var balance = 3000m;

            if (balance > context.Message.PaymentMessage.TotalPrice)
            {
                _logger.LogInformation($"{context.Message.PaymentMessage.TotalPrice}₺ was withrawn from credit card for userId: {context.Message.BuyerId}");

                await _publishEndpoint.Publish(new PaymentCompletedEvent
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                });
            }
            else
            {
                _logger.LogInformation($"{context.Message.PaymentMessage.TotalPrice}₺ was not withrawn from credit card for userId: {context.Message.BuyerId}");

                await _publishEndpoint.Publish(new PaymentFailedEvent
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "Not enough balance",
                    OrderItemMessages = context.Message.OrderItemMessages,
                });
            }
        }
    }
}
