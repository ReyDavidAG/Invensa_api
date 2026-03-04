using Invensa.Domain.Dtos;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Products.Commands;

public sealed class UpdateProductCommand : IRequest<Result<bool>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Code { get; init; } = default!;
    public Guid UnitId { get; init; }
    public decimal PriceSale { get; init; }
    public decimal PriceBuy { get; init; }
    public byte[] RowVersion { get; init; } = default!;

    // ---------- NUEVO ----------
    // Si envías Image => reemplaza imagen (crea nueva ImageFile y asigna)
    public ImageUploadDto? Image { get; init; }
    // Si true => quita la imagen (pone null)
    public bool RemoveImage { get; init; }
}
