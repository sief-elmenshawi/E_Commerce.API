using E_Commerce.Application.DTOs.Authentications;
using FluentValidation;

namespace E_Commerce.Application.Validators.Authentications
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
               .NotEmpty().WithMessage("Password is required.")
               .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
               .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
               .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
               .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("Display name is required.")
                .MaximumLength(100).WithMessage("Display name must not exceed 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^01[0-2,5]{1}[0-9]{8}$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Invalid Egyptian phone number format.");
        }
    }
}
