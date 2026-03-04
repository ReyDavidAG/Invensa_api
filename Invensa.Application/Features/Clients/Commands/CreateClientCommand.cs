using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Clients.Commands;

public sealed class CreateClientCommand : IRequest<Result<Guid>>
{
    public string? Identifier { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}
