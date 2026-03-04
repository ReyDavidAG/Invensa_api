using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Inventory.Commands;

public sealed class RegisterEntryCommand : IRequest<Result<bool>>
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }     // > 0
    public decimal? UnitPrice { get; set; }   // si no se manda, puedes usar PriceBuy
    public string? Note { get; set; }
}
