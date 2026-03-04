// Invensa.Application/Features/Inventory/Validators/RegisterEntryCommandValidator.cs
using FluentValidation;

namespace Invensa.Application.Features.Inventory.Commands;

public sealed class RegisterEntryCommandValidator : AbstractValidator<RegisterEntryCommand>
{
    public RegisterEntryCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).When(x => x.UnitPrice.HasValue);
    }
}
