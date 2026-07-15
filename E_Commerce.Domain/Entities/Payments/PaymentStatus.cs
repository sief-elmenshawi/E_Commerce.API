using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Entities.Payments
{
    public enum PaymentStatus
    {
        Pending = 0,
        Succeeded = 1,
        Failed = 2
    }
}
