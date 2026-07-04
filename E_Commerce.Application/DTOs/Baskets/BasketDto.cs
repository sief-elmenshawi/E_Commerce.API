using E_Commerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.DTOs.Baskets
{
    public class BasketDto
    {
        public string Id { get; set; } = default!;
        public ICollection<BasketItemDto> Items { get; set; }
    }
}
