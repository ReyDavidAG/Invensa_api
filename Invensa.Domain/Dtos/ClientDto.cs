namespace Invensa.Domain.Dtos;

public sealed class ClientDto
{
    public Guid Id { get; init; }
    public string? Identifier { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public bool Active { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public string RowVersion { get; init; } = default!;
}
