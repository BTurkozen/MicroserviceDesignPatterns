using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.OrderEvents;
using Shared.Settings;
using Shared.StockEvents;
using Stock.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.Api.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly DataContext _dataContext;
        private readonly ILogger _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(DataContext dataContext, ILogger logger, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _dataContext = dataContext;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();

            foreach (var orderItem in context.Message.OrderItemMessages)
            {
                stockResult.Add(await _dataContext.Stocks.AnyAsync(s => s.ProductId == orderItem.ProductId && s.Count > orderItem.Count));
            }

            if (stockResult.All(sr => sr.Equals(true)))
            {
                foreach (var orderItem in context.Message.OrderItemMessages)
                {
                    var stock = await _dataContext.Stocks.FirstOrDefaultAsync(oi => oi.ProductId == orderItem.ProductId);

                    if (stock is not null)
                    {
                        stock.Count -= orderItem.Count;

                    }
                    await _dataContext.SaveChangesAsync();

                }
                _logger.LogInformation($"Stock was reserved for Buyer Id : {context.Message.BuyerId}");

                var sendEnpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMqSettingsConst.StockReservedEventQueueName}"));

                var stockReservedEvent = new StockReservedEvent
                {
                    PaymentMessage = context.Message.Payment,
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    OrderItemMessages = context.Message.OrderItemMessages
                };

                await _sendEndpointProvider.Send(stockReservedEvent);
            }
            else
            {
                await _publishEndpoint.Publish(new StockNotReservedEvent
                {
                    OrderId = context.Message.OrderId,
                    Message = "Not enough stock"
                });
            }
        }
    }
}
