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
            var result = await basketService.GetBasketAsync(id,ct);
            return ToActionResult(result);
        }

        // Post baseUrl/api/baskets
        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateOrUpdateBasket([FromBody] BasketDto basket, CancellationToken ct)
        {
            var result = await basketService.CreateOrUpdateBasketAsync(basket, ct:ct);
            return ToActionResult(result);
        }

        // Delete baseUrl/api/baskets/Id
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBasket(string id, CancellationToken ct)
        {
            var result = await basketService.DeleteBasketAsync(id, ct);
            return ToActionResult(result);
        }


    }
}
