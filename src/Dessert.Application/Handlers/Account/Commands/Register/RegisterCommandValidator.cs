using FluentValidation;

namespace Dessert.Application.Handlers.Account.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");
            RuleFor(v => v.Nickname)
                .NotEmpty().WithMessage("Nickname is required.")
                .MaximumLength(256).WithMessage("Nickname must not exceed 256 characters.");
            RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(256).WithMessage("Password must not exceed 256 characters.");
        }
    }
}