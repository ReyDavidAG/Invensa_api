using Invensa.Domain.Dtos;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Inventory.Commands;

public sealed class CreateProductCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public Guid UnitId { get; set; }
    public decimal PriceSale { get; set; }
    public decimal PriceBuy { get; set; }

    // opcional: stock inicial (si > 0, crea movimiento IN)
    public decimal? InitialQuantity { get; set; }
    public decimal? InitialUnitPrice { get; set; } // si no lo pasas, usa PriceBuy
    public string? Note { get; set; }

    // ---------- NUEVO: imagen opcional al crear ----------
    public ImageUploadDto? Image { get; set; }
}
