using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Specifications
{
    internal class ProductWithIdSpecification :BaseSpecification<Product,int>
    {
        public ProductWithIdSpecification(IEnumerable<int> Ids):base(p=>Ids.Contains(p.Id))
        {
             
        }
    }
}
