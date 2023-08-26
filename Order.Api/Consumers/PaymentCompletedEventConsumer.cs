using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order.Api.Models;
using Shared.PaymentEvent;
using System.Threading.Tasks;

namespace Order.Api.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<PaymentCompletedEventConsumer> _logger;

        public PaymentCompletedEventConsumer(DataContext dataContext, ILogger<PaymentCompletedEventConsumer> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = await _dataContext.Orders.FindAsync(context.Message.OrderId);

            if (order is not null)
            {
                order.Status = Status.Complete;
                await _dataContext.SaveChangesAsync();

                _logger.LogInformation($"OrderId: {context.Message.OrderId} status changed: {order.Status}");
            }
            else
            {
                _logger.LogError($"OrderId: {context.Message.OrderId} not found");
            }
        }
    }
}
