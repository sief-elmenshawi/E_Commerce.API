using E_Commerce.API.Attributes;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ApiBaseController
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        // Get All Products
        [HttpGet]
        [RedisCache(90)]
        public async Task<ActionResult<PaginatedResult<ProductDto>>> GetALlProducts([FromQuery]ProductQueryParams queryParams,CancellationToken ct)
        {
            var result = await productService.GetAllProductsAsync(queryParams,ct);
            return ToActionResult(result);
        }

        // Get Product By Id 
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id , CancellationToken ct)
        {
            var result = await productService.GetProductByIdAsync(id,ct);
            return ToActionResult(result);
             

        }
        // Get All Types    
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<TypeDto>>> GetAllTypes( CancellationToken ct)
        {
            return ToActionResult(await productService.GetAllTypeAsync(ct));
        }
        // Get All Brands
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<BrandDto>>> GetAllBrands(CancellationToken ct)
        {
            return ToActionResult(await productService.GetAllBrandAsync(ct));
        }
    }
}
