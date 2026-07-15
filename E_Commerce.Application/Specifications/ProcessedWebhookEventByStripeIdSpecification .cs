using E_Commerce.Domain.Entities.Payments;

namespace E_Commerce.Application.Specifications;

internal class ProcessedWebhookEventByStripeIdSpecification : BaseSpecification<ProcessedWebhookEvent, Guid>
{
    public ProcessedWebhookEventByStripeIdSpecification(string stripeEventId)
        : base(x => x.StripeEventId == stripeEventId)
    {
    }
}
