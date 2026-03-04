using LanguageExt.Common;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Invensa.Application.Features.Units.Commands;

public sealed class UpdateUnitCommand : IRequest<Result<bool>>
{
    [Required] public Guid Id { get; init; }
    [Required] public string Code { get; init; } = default!;
    [Required] public string Name { get; init; } = default!;
}
