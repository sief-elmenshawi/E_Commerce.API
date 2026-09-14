using E_Commerce.Application.DTOs.Orders;
using FluentValidation;

namespace E_Commerce.Application.Validators.Orders
{
    public class OrderDtoValidator : AbstractValidator<OrderDto>
    {
        public OrderDtoValidator()
        {
            RuleFor(x => x.BasketId)
                .NotEmpty().WithMessage("Basket id is required.");

            RuleFor(x => x.DeliveryMethodId)
                .GreaterThan(0).WithMessage("Please select a valid delivery method.");

            RuleFor(x => x.ShippingAddress)
                .NotNull().WithMessage("Shipping address is required.")
                .SetValidator(new Authentications.AddressDtoValidator());
        }
    }
}