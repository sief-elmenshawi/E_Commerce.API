using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{

    public class BasketsController : ApiBaseController
    {
        private readonly IBasketService basketService;

        public BasketsController(IBasketService basketService)
        {
            this.basketService = basketService;
        }

        // Get BaseUrl/api/baskets/Id
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<BasketDto>> GetBasket(string id, CancellationToken ct)
        {
            // The basket is owned by the authenticated user and keyed by their id - never serve someone else's basket.
            if (!string.Equals(id, GetBasketOwnerId(), StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var result = await basketService.GetBasketAsync(GetBasketOwnerId(), ct);
            return ToActionResult(result);
        }

        // Post baseUrl/api/baskets
        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateOrUpdateBasket([FromBody] BasketDto basket, CancellationToken ct)
        {
            // Baskets are always keyed by the authenticated user's id - never accept a client-supplied basket id.
            basket.Id = GetBasketOwnerId();
            var result = await basketService.CreateOrUpdateBasketAsync(basket, ct:ct);
            return ToActionResult(result);
        }

        // Delete baseUrl/api/baskets/Id
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBasket(string id, CancellationToken ct)
        {
            // Baskets are keyed by their owner's id - only the authenticated owner may delete their own basket.
            if (!string.Equals(id, GetBasketOwnerId(), StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var result = await basketService.DeleteBasketAsync(GetBasketOwnerId(), ct);
            return ToActionResult(result);
        }


    }
}
