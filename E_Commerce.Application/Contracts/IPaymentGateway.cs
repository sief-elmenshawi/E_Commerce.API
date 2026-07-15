using E_Commerce.Application.DTOs.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Contracts
{
    public interface IPaymentGateway
    {
        Task<CreatePaymentResponse> CreatePaymentIntentAsync(decimal amount, CancellationToken ct = default);

        Task<CreatePaymentResponse> UpdatePaymentIntentAsync(string paymentIntentId, decimal amount, CancellationToken ct = default);

        PaymentWebhookEvent ParseWebhookEvent(string json, string stripeSignatureHeader);
    }
}
