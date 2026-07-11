using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{

    public class OrderController : ApiBaseController
    {
        private readonly IOrderService orderService; 

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder([FromBody] OrderDto orderDto, [FromQuery] string email,CancellationToken ct)
        {
            return ToActionResult(await orderService.CreateOrderAsync(orderDto, email, ct));
        }


        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethodDto>>> GetAllDeliveryMethod(CancellationToken ct)
        {
            return ToActionResult(await orderService.GetAllDeliveryMethodsAsync(ct));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersByEmail([FromQuery] string email,CancellationToken ct)
        {
            return ToActionResult(await orderService.GetAllOrdersByEmailAsync(email,ct));
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdAndEmail(Guid id,[FromQuery] string email, CancellationToken ct)
        {
            return ToActionResult(await orderService.GetOrderByIdAndEmailAsync(id,email, ct));
        }


    }
}
