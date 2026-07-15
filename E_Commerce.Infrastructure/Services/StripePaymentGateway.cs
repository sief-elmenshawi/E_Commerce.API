using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Payments;
using E_Commerce.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_Commerce.Infrastructure.Services
{
    internal class StripePaymentGateway : IPaymentGateway
    {
        private readonly StripeOptions stripeOptions;
        private readonly PaymentIntentService paymentIntentService;

        public StripePaymentGateway(IOptions<StripeOptions> stripeOptions)
        {
            this.stripeOptions = stripeOptions.Value;
            StripeConfiguration.ApiKey = this.stripeOptions.SecretKey;
            paymentIntentService = new PaymentIntentService();
        }

        public async Task<CreatePaymentResponse> CreatePaymentIntentAsync(decimal amount, CancellationToken ct = default)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = ConvertToSmallestCurrencyUnit(amount),
                Currency = stripeOptions.Currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
            };

            var paymentIntent = await paymentIntentService.CreateAsync(options, cancellationToken: ct);

            return new CreatePaymentResponse
            {
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret
            };
        }

        public async Task<CreatePaymentResponse> UpdatePaymentIntentAsync(string paymentIntentId, decimal amount, CancellationToken ct = default)
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = ConvertToSmallestCurrencyUnit(amount)
            };

            var paymentIntent = await paymentIntentService.UpdateAsync(paymentIntentId, options, cancellationToken: ct);

            return new CreatePaymentResponse
            {
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret
            };
        }

        public PaymentWebhookEvent ParseWebhookEvent(string json, string stripeSignatureHeader)
        {
            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignatureHeader, stripeOptions.WebhookSecret);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            var eventType = stripeEvent.Type switch
            {
                "payment_intent.succeeded" => PaymentWebhookEventType.Succeeded,
                "payment_intent.payment_failed" => PaymentWebhookEventType.Failed,
                _ => PaymentWebhookEventType.Other
            };

            return new PaymentWebhookEvent
            {
                StripeEventId = stripeEvent.Id,
                PaymentIntentId = paymentIntent?.Id ?? string.Empty,
                EventType = eventType
            };
        }

        private static long ConvertToSmallestCurrencyUnit(decimal amount) => (long)(amount * 100);
    }
}