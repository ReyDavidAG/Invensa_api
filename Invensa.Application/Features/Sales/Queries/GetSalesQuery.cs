using Invensa.Domain.Dtos;
using Invensa.Domain.Models.Responses;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Sales.Queries;

public sealed class GetSalesQuery : IRequest<Result<PaginatedResult<SaleDto>>>
{
    public int? Page { get; init; }          // default 1
    public int? PageSize { get; init; }      // default 50
    public DateTime? FromUtc { get; init; }  // filtro opcional
    public DateTime? ToUtc { get; init; }    // filtro opcional
    public long? TicketNumber { get; init; } // filtro opcional
    public Guid? ClientId { get; init; }     // filtro opcional
}
