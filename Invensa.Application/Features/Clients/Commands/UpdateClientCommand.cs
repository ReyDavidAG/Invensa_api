using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Clients.Commands;

public sealed class UpdateClientCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool Active { get; set; }
    public byte[] RowVersion { get; set; } = default!;
}
