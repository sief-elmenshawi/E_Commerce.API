using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Common
{
    public class ProductQueryParams
    {
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public string? SearchValue { get; set; }
        public ProductSortOptions Sort { get; set; }

    }
}
