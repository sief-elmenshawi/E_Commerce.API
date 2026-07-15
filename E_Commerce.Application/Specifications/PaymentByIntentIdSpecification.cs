using E_Commerce.Domain.Entities.Payments;

namespace E_Commerce.Application.Specifications;

internal class PaymentByIntentIdSpecification : BaseSpecification<Payment, Guid>
{
    public PaymentByIntentIdSpecification(string paymentIntentId)
        : base(x => x.PaymentIntentId == paymentIntentId)
    {
        AddInclude(x => x.Order);
    }
}
