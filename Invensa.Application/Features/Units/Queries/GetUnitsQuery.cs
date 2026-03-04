using Invensa.Domain.Dtos;
using Invensa.Domain.Models.Responses;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Units.Queries;

public sealed class GetUnitsQuery : IRequest<Result<PaginatedResult<UnitDto>>>
{
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
