using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace E_Commerce.Domain.Entities.Orders
{
    public class Order : BaseEntity<Guid>
    {
        public string BuyerEmail { get; set; } = default!;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Item { get; set; } = [];

        public OrderAddress ShippingAddress { get; set; } =default!;
        public DeliveryMethod DeliveryMethod { get; set; } =default!;

        [ForeignKey("DeliveryMethod")]
        public int DeliveryMethodId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal SubTotal { get; set; }

        public decimal GetTotal() => SubTotal + (DeliveryMethod? .Cost ?? 0);

    }
}
