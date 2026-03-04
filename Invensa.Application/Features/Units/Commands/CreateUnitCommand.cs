using LanguageExt.Common;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Invensa.Application.Features.Units.Commands;

public sealed class CreateUnitCommand : IRequest<Result<Guid>>
{
    [Required] public string Code { get; init; } = default!;
    [Required] public string Name { get; init; } = default!;
}
