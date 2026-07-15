using E_Commerce.Domain.Common;
using E_Commerce.Domain.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Entities.Payments
{
    public class Payment : BaseEntity<Guid>
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? TransactionReference { get; set; }
        public string? PaymentIntentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
