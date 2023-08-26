using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.PaymentEvent;
using Stock.Api.Models;
using System.Threading.Tasks;

namespace Stock.Api.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<PaymentFailedEventConsumer> _logger;

        public PaymentFailedEventConsumer(DataContext dataContext, ILogger<PaymentFailedEventConsumer> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            foreach (var orderItem in context.Message.OrderItemMessages)
            {
                var stock = await _dataContext.Stocks.FirstOrDefaultAsync(s => s.ProductId == orderItem.ProductId);

                if (stock is not null)
                {
                    stock.Count += orderItem.Count;

                    await _dataContext.SaveChangesAsync();
                }
            }

            _logger.LogInformation($"Stock was released for OrderId: {context.Message.OrderId}");
        }
    }
}
