using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Payments;

namespace E_Commerce.Application.Services;

public interface IPaymentService
{
    Task<Result<CreatePaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken ct = default);

    Task<Result> HandleStripeWebhookAsync(string json, string stripeSignature, CancellationToken ct = default);
}