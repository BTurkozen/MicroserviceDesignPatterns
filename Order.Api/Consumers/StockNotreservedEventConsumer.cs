using MassTransit;
using Microsoft.Extensions.Logging;
using Order.Api.Models;
using Shared.PaymentEvent;
using Shared.StockEvents;
using System.Threading.Tasks;

namespace Order.Api.Consumers
{
    public class StockNotreservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<StockNotreservedEventConsumer> _logger;

        public StockNotreservedEventConsumer(DataContext dataContext, ILogger<StockNotreservedEventConsumer> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = await _dataContext.Orders.FindAsync(context.Message.OrderId);

            if (order is not null)
            {
                order.Status = Status.Fail;
                order.Fail = context.Message.Message;
                await _dataContext.SaveChangesAsync();

                _logger.LogInformation($"OrderId: {context.Message.OrderId} status changed: {order.Status}");
            }
            else
            {
                _logger.LogInformation($"OrderId: {context.Message.OrderId} not found");
            }
        }
    }
}
