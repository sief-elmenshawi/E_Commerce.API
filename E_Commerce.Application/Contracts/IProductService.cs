using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Contracts
{
    public interface IProductService
    {
        Task<Result<PaginatedResult<ProductDto>>> GetAllProductsAsync(ProductQueryParams queryParams ,CancellationToken ct = default);
        Task<Result<IReadOnlyList<BrandDto>>> GetAllBrandAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<TypeDto>>> GetAllTypeAsync(CancellationToken ct = default);
        Task<Result<ProductDto>> GetProductByIdAsync(int id , CancellationToken ct = default);

    }
}
