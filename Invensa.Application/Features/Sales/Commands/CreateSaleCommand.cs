using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Sales.Commands;

public sealed class CreateSaleItemDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }     // > 0
    public decimal? UnitPrice { get; set; }   // si no se manda, usa Product.PriceSale
}

public sealed class CreateSaleCommand : IRequest<Result<Guid>>
{
    public DateTime? DateUtc { get; set; }
    public Guid? ClientId { get; set; }
    public List<CreateSaleItemDto> Items { get; set; } = new();
}
