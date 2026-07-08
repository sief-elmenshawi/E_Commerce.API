using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Baskets;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Contracts
{
    public interface IBasketService
    {
        // Get Basket
        Task<Result<BasketDto>> GetBasketAsync(string basketId,CancellationToken ct = default);

        // Create or Update Basket
        Task<Result<BasketDto>> CreateOrUpdateBasketAsync(BasketDto basket,TimeSpan? TLV = default, CancellationToken ct = default);

        // Delete Basket
        Task<Result<bool>> DeleteBasketAsync(string basketId, CancellationToken ct = default);

    }
}
