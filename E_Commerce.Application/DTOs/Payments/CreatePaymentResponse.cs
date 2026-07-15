using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.DTOs.Payments
{
    public class CreatePaymentResponse  
    {
        public string PaymentIntentId { get; set; } = default!;

        public string ClientSecret { get; set; } = default!;
    }
}
