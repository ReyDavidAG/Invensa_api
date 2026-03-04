using Invensa.Application.Features.Units.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Units.Handlers;

public sealed class GetUnitByIdQueryHandler
    : IRequestHandler<GetUnitByIdQuery, Result<UnitDto>>
{
    private readonly IUnitOfWork _uow;
    public GetUnitByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<UnitDto>> Handle(GetUnitByIdQuery request, CancellationToken ct)
    {
        var dto = await _uow.UnitRepository.GetByIdAsync(ct, request.Id);
        if (dto is null) return new Result<UnitDto>(new NotFoundException("Unidad no encontrada."));
        return new Result<UnitDto>(dto);
    }
}
