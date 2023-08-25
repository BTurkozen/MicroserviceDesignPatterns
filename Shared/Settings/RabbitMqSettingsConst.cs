using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Settings
{
    public class RabbitMqSettingsConst
    {
        public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
        public const string StockReservedEventQueueName = "stock-reserved-queue";
    }
}
