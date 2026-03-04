using FluentValidation;
using Invensa.Application.Features.Sales.Commands;
using Invensa.Domain.Entities;
using Invensa.Domain.Interfaces;

namespace Invensa.Application.Features.Sales.Validators;

public sealed class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator(IUnitOfWork uow)
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("La venta debe tener al menos un artículo.");

        RuleFor(x => x.ClientId)
            .MustAsync(async (clientId, ct) =>
            {
                if (!clientId.HasValue) return true;
                return await uow.ClientRepository.AnyAsync(c => c.Id == clientId.Value, ct);
            })
            .WithMessage("El cliente especificado no existe.");

        RuleForEach(x => x.Items).ChildRules(it =>
        {
            it.RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("El ID del producto es obligatorio.");

            it.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

            it.RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).When(x => x.UnitPrice.HasValue)
                .WithMessage("El precio unitario no puede ser negativo.");

            // Validar existencia del producto y stock
            it.RuleFor(x => x)
                .CustomAsync(async (item, context, ct) =>
                {
                    // Usamos FirstOrDefaultAsync para obtener la entidad Product
                    var product = await uow.ProductRepository.FirstOrDefaultAsync(p => p.Id == item.ProductId, ct, asNoTracking: true);
                    
                    if (product == null)
                    {
                        context.AddFailure("ProductId", $"El producto con ID {item.ProductId} no existe.");
                        return;
                    }

                    var stock = await GetStockOnHandAsync(uow, item.ProductId, ct);
                    if (stock < item.Quantity)
                    {
                        context.AddFailure("Quantity", $"Stock insuficiente para el producto {product.Id} - {product.Name}. Disponible: {stock}, Requerido: {item.Quantity}");
                    }
                });
        });
    }

    private async Task<decimal> GetStockOnHandAsync(IUnitOfWork uow, Guid productId, CancellationToken ct)
    {
        var moves = await uow.InventoryMovementRepository.WhereAsync(m => m.ProductId == productId, ct, asNoTracking: true);
        decimal stock = 0m;
        foreach (var m in moves)
        {
            if (m.MovementType == MovementType.In) stock += m.Quantity ?? 0;
            else if (m.MovementType == MovementType.Out) stock -= m.Quantity ?? 0;
            else if (m.MovementType == MovementType.Adj) stock += m.QuantityAdj ?? 0;
        }
        return stock;
    }
}
