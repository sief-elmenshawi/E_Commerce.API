using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.DTOs.Orders
{
    public class DeliveryMethodDto
    {
        public int Id { get; set; }
        public string ShortName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string DeliveryTime { get; set; } = default!;
        public decimal Cost { get; set; } = default!;
    }
}
