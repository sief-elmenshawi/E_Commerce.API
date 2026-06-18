using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Entities.Products
{
    public class ProductType : BaseEntity<int>
    {
        public string Name { get; set; } = default!;

    }
}
