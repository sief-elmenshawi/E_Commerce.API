using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Contracts
{
    public interface IProductService
    {
        Task<Result<IReadOnlyList<ProductDto>>> GetAllProductsAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<BrandDto>>> GetAllBrandAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<TypeDto>>> GetAllTypeAsync(CancellationToken ct = default);
        Task<Result<ProductDto>> GetProductByIdAsync(int id , CancellationToken ct = default);

    }
}
