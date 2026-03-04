using Invensa.Domain.Dtos;
using Invensa.Domain.Models.Responses;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Products.Queries;

public sealed class GetProductsQuery : IRequest<Result<PaginatedResult<ProductDto>>>
{
    public int? Page { get; init; }     // default 1
    public int? PageSize { get; init; } // default 50
    public string? Search { get; init; }
    public Guid? UnitId { get; init; }
}
