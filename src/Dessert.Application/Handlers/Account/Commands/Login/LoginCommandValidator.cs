using FluentValidation;

namespace Dessert.Application.Handlers.Account.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");
            RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(256).WithMessage("Password must not exceed 256 characters.");
        }
    }
}