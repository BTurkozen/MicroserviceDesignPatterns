using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Dtos;
using Order.Api.Models;
using Shared.Messages;
using Shared.OrderEvents;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(DataContext dataContext, IPublishEndpoint publishEndpoint)
        {
            _dataContext = dataContext;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto orderCreateDto)
        {
            var newOrder = new Models.Order
            {
                BuyerId = orderCreateDto.BuyerId,
                Status = Status.Suspend,
                Address = new Address
                {
                    District = orderCreateDto.Address.District,
                    Line = orderCreateDto.Address.Line,
                    Province = orderCreateDto.Address.Province,
                },
                CreatedOn = DateTime.Now,
            };

            orderCreateDto.OrderItems.ForEach(orderItem =>
            {
                newOrder.OrderItems.Add(new OrderItem()
                {
                    Count = orderItem.Count,
                    Price = orderItem.Price,
                    ProductId = orderItem.ProductId,
                });
            });

            await _dataContext.AddAsync(newOrder);

            await _dataContext.SaveChangesAsync();

            #region Event Oluşturma

            var orderCreatedEvent = new OrderCreatedEvent()
            {
                BuyerId = newOrder.BuyerId,
                OrderId = newOrder.Id,
                Payment = new PaymentMessage
                {
                    CardName = orderCreateDto.Payment.CardName,
                    CardNumber = orderCreateDto.Payment.CardNumber,
                    CVV = orderCreateDto.Payment.CVV,
                    Expiration = orderCreateDto.Payment.Expiration,
                    TotalPrice = orderCreateDto.OrderItems.Sum(x => x.Price * x.Count),
                }
            };

            orderCreateDto.OrderItems.ForEach(orderItem =>
            {
                orderCreatedEvent.OrderItemMessages.Add(new OrderItemMessage()
                {
                    Count = orderItem.Count,
                    ProductId = orderItem.ProductId,
                });
            });

            #endregion

            #region Publish İşlemi

            // Publish =>RabbitMq'ya bu mesajı gönderiyorsanız, subscribe olan yoksa boşa gider.  
            // RabbitMQ'ya bir event gönderirsin kimlerin dinledğini bilmessin.
            // Buradaki Event Direk kuyruğa gitmez. Exchange'e gider.
            // O Exchange suybscribe olan bir kuyruk yoksa mesaj boşa gider.
            // İhtiyacımız olan burada Exchange subscribe olmuş bir kuyruk.

            // Exchange'e değil de Direk olarak kuyruğa göndermek istediğiniz de Send Methodu direk bu işlemi yapar. Fakat Send IPublishEndpoint interface ile değil. ISendEndPointProvider ile gönderim sağlar.

            await _publishEndpoint.Publish(orderCreatedEvent);

            #endregion

            return Ok();
        }
    }
}
