using E_Commerce.Domain.Entities.Orders;

namespace E_Commerce.Application.Specifications;

internal class OrderForPaymentSpecification : BaseSpecification<Order, Guid>
{
    public OrderForPaymentSpecification(Guid orderId, string buyerEmail)
        : base(x => x.Id == orderId && x.BuyerEmail == buyerEmail)
    {
        AddInclude(x => x.Item);
        AddInclude(x => x.DeliveryMethod);
        AddInclude(x => x.Payments);
    }
}