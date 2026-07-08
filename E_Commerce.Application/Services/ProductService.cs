using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Services
{
    internal class ProductService : IProductService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<Result<IReadOnlyList<BrandDto>>> GetAllBrandAsync(CancellationToken ct = default)
        {
            var brand = await unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync(ct);
            var data = mapper.Map<IReadOnlyList<BrandDto>>(brand);
            return Result<IReadOnlyList<BrandDto>>.Ok(data);

        }

        public async Task<Result<PaginatedResult<ProductDto>>> GetAllProductsAsync(ProductQueryParams queryParams,CancellationToken ct = default)
        {
            var spec = new ProductWithTypeAndBrandSpec(queryParams);
            var products = await unitOfWork.GetRepository<Product,int>().GetAllAsync(spec);
            var data = mapper.Map<IReadOnlyList<ProductDto>>(products);
            var countSpec = new ProductCountSpecifications(queryParams);
            var countOfAllProduct = await unitOfWork.GetRepository<Product, int>().CountAsync(countSpec);
            var result = new PaginatedResult<ProductDto>(queryParams.PageIndex , queryParams.PageSize , countOfAllProduct , data);
            return Result<PaginatedResult<ProductDto>>.Ok(result);
        }

        public async Task<Result<IReadOnlyList<TypeDto>>> GetAllTypeAsync(CancellationToken ct = default)
        {
            var Types = mapper.Map<IReadOnlyList<TypeDto>>(await unitOfWork.GetRepository<ProductType, int>().GetAllAsync(ct));
            return Result<IReadOnlyList<TypeDto>>.Ok(Types);

        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(int id, CancellationToken ct = default)
        {
            var spec = new ProductWithTypeAndBrandSpec(id);
            var product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec, ct);
            if (product == null)
                return Error.NotFound("Product Not Found" ,$"Product With Id {id} Is Not Found");
           
            return mapper.Map<ProductDto>(product);
        }
    }
}
