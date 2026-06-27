using E_Commerce.Application.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Specifications
{
    internal class ProductWithTypeAndBrandSpec : BaseSpecification<Product,int>
    {
        public ProductWithTypeAndBrandSpec(ProductQueryParams queryParams)
            : base(p => (!queryParams.BrandId.HasValue || p.BrandId == queryParams.BrandId.Value)
            && (!queryParams.TypeId.HasValue || p.TypeId == queryParams.TypeId.Value)
            && (string.IsNullOrWhiteSpace(queryParams.SearchValue) || p.Name.ToLower().Contains(queryParams.SearchValue.ToLower())))
        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);

            switch (queryParams.Sort)   
            {
                case ProductSortOptions.NameAcc:
                    AddOrderBy(p => p.Name);
                    break;
                case ProductSortOptions.NameDesc:
                    AddOrderDescBy(p => p.Name);
                    break;
                case ProductSortOptions.PriceAcc:
                    AddOrderBy(p => p.Price);
                    break;
                case ProductSortOptions.PriceDesc:
                    AddOrderDescBy(p => p.Price);
                    break;
                default:
                    AddOrderDescBy(p=>p.Id);    
                    break;
            }

        }

        public ProductWithTypeAndBrandSpec(int id) : base(x=>x.Id == id)
        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);
        }
    }
}
