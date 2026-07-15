namespace E_Commerce.Application.DTOs.Payments
{
    public class PaymentWebhookEvent
    {
        public string StripeEventId { get; set; } = default!;

        public string PaymentIntentId { get; set; } = default!;

        public PaymentWebhookEventType EventType { get; set; }
    }

    public enum PaymentWebhookEventType
    {
        Succeeded,
        Failed,
        Other
    }
}







