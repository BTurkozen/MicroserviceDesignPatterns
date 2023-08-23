using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Messages;

namespace Shared.OrderEvents
{
    public class OrderCreatedEvent
    {
        public OrderCreatedEvent()
        {
            OrderItemMessages = new List<OrderItemMessage>();
        }
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItemMessages { get; set; }
    }
}
