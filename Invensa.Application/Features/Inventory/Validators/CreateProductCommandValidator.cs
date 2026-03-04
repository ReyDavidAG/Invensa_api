using FluentValidation;
using Invensa.Domain.Dtos;

namespace Invensa.Application.Features.Inventory.Commands;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(64);
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.PriceSale).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PriceBuy).GreaterThanOrEqualTo(0);

        When(x => x.InitialQuantity.HasValue, () =>
        {
            RuleFor(x => x.InitialQuantity!.Value).GreaterThan(0);
            RuleFor(x => x.InitialUnitPrice).GreaterThanOrEqualTo(0).When(x => x.InitialUnitPrice.HasValue);
        });

        // ---------- Validación de imagen opcional ----------
        When(x => x.Image != null, () =>
        {
            RuleFor(x => x.Image!.FileName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Image!.ContentType).NotEmpty().MaximumLength(128);
            RuleFor(x => x.Image!.SizeBytes).GreaterThan(0);
            RuleFor(x => x.Image!.Base64).NotEmpty();
            RuleFor(x => x.Image!.Sha256).MaximumLength(128).When(i => !string.IsNullOrWhiteSpace(value: i.Image!.Sha256));
        });
    }
}
