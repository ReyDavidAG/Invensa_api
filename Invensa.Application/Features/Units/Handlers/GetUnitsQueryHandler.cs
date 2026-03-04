using Invensa.Application.Features.Units.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Interfaces;
using Invensa.Domain.Models.Responses;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Units.Handlers;

public sealed class GetUnitsQueryHandler
    : IRequestHandler<GetUnitsQuery, Result<PaginatedResult<UnitDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetUnitsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PaginatedResult<UnitDto>>> Handle(GetUnitsQuery request, CancellationToken ct)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var size = request.PageSize <= 0 ? 50 : Math.Min(request.PageSize, 200);

        var pageResult = await _uow.UnitRepository.GetUnitsByParametersAsync(ct, page, size, request.Search);
        return new Result<PaginatedResult<UnitDto>>(pageResult);
    }
}
