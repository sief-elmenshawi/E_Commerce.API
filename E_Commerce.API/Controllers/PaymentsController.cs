using E_Commerce.Application.DTOs.Payments;
using E_Commerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CreatePaymentResponse>> CreatePayment([FromBody] CreatePaymentRequest request, CancellationToken ct)
        {
            return ToActionResult(await paymentService.CreatePaymentAsync(request, ct));
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook(CancellationToken ct)
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync(ct);
            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

            var result = await paymentService.HandleStripeWebhookAsync(json, stripeSignature, ct);

            return result.IsSuccess ? Ok() : ToProblem(result.Errors);
        }
    }
}
