using E_Commerce.Domain.Common;

namespace E_Commerce.Domain.Entities.Payments
{
    
    public class ProcessedWebhookEvent : BaseEntity<Guid>
    {
        public string StripeEventId { get; set; } = default!;

        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}