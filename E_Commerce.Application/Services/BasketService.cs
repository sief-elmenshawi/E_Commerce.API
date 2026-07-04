using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Services
{
    internal class BasketService : IBasketService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketService(IBasketRepository basketRepository,IMapper mapper)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }
        public async Task<Result<BasketDto>> CreateOrUpdateBasketAsync(BasketDto basket, TimeSpan? TLV = null, CancellationToken ct = default)
        {
            var customerBasket = mapper.Map<CustomerBasket>(basket);
            var basketResult = await  basketRepository.CreateOrUpdateBasketAsync(customerBasket, TLV,ct);

            return basketResult == null ? Result<BasketDto>.Fail(Error.Failure("Basket Create Failure","Can Not Create Or Update Basket"))
                : Result<BasketDto>.Ok(basket);

        }

        public async Task<Result<bool>> DeleteBasketAsync(string basketId, CancellationToken ct = default)
        {
            var result = await basketRepository.DeleteBasketAsync(basketId, ct);
            return result ? Result<bool>.Ok(true) : Result<bool>.Fail(Error.Failure("Basket Delete Failure", "Can Not Delete Basket"));
        }

        public async Task<Result<BasketDto>> GetBasketAsync(string basketId, CancellationToken ct = default)
        {
            var basket = await basketRepository.GetBasketAsync(basketId, ct);
            return basket == null ? Result<BasketDto>.Fail(Error.Failure("Basket Not Found", "Basket not found")) :mapper.Map<BasketDto>(basket);
        }
    }
}
