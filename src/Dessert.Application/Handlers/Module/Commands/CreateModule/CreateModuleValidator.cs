using FluentValidation;

namespace Dessert.Application.Handlers.Module.Commands.CreateModule
{
    public class CreateModuleValidator : AbstractValidator<CreateModule>
    {
        public CreateModuleValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(256).WithMessage("Name must not exceed 256 characters.");
            RuleFor(v => v.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(256).WithMessage("Description must not exceed 256 characters.");
            RuleFor(v => v.Token)
                .NotEmpty().WithMessage("Token is required.");
            RuleFor(v => v.Replacements)
                .NotNull().WithMessage("Token is required.");
        }
    }
}