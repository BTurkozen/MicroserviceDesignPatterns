﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.PaymentEvent
{
    public class PaymentSuccessedEvent
    {
        public int OrderId { get; set; }
        public int BuyerId { get; set; }
    }
}
