using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Entities.Orders
{
    public enum OrderStatus
    {
        Pending = 0,
        PaymentReceived = 1,
        PaymentFailed = 2, 
    }
}
