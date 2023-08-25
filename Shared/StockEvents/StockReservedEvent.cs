using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.StockEvents
{
    public class StockReservedEvent
    {
        public StockReservedEvent()
        {
            OrderItemMessages = new List<OrderItemMessage>();
        }

        public int OrderId { get; set; }
        public int BuyerId { get; set; }
        public PaymentMessage PaymentMessage { get; set; }
        public List<OrderItemMessage> OrderItemMessages { get; set; }
    }
}
