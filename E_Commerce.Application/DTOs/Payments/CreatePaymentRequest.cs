using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.DTOs.Payments
{
    public class CreatePaymentRequest
    {
        public Guid OrderId { get; set; }

    }
}
