using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.DTOs.Orders
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }  = default!;
        public string PictureUrl { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }
}
