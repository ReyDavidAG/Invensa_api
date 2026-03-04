using Invensa.Domain.Dtos;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Units.Queries;

public sealed class GetUnitByIdQuery : IRequest<Result<UnitDto>>
{
    public Guid Id { get; init; }
}
