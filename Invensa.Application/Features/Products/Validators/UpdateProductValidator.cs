using FluentValidation;

namespace Invensa.Application.Features.Products.Commands;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(64);
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.PriceSale).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PriceBuy).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RowVersion)
            .NotNull().WithMessage("RowVersion es requerido.")
            .NotEmpty().WithMessage("RowVersion es requerido.");


        // Si manda imagen para reemplazo
        When(x => x.Image != null, () =>
        {
            RuleFor(x => x.Image!.FileName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Image!.ContentType).NotEmpty().MaximumLength(128);
            RuleFor(x => x.Image!.SizeBytes).GreaterThan(0);
            RuleFor(x => x.Image!.Base64).NotEmpty();
            RuleFor(x => x.Image!.Sha256).MaximumLength(128).When(i => !string.IsNullOrWhiteSpace(value: i.Image!.Sha256));
        });

        // No tiene sentido que pida remover y a la vez mandar nueva imagen
        RuleFor(x => x.RemoveImage)
            .Must((cmd, remove) => !(remove && cmd.Image != null))
            .WithMessage("No puedes enviar Image y RemoveImage=true a la vez.");
    }
}
