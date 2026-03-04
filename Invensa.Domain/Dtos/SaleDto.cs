namespace Invensa.Domain.Dtos;

public sealed class SaleDto
{
    public Guid Id { get; init; }
    public DateTime DateUtc { get; init; }
    public long TicketNumber { get; init; }
    public decimal Total { get; init; }
    public Guid? ClientId { get; init; }
    public string? ClientName { get; init; }
    public IReadOnlyList<SaleItemDto> Items { get; init; } = Array.Empty<SaleItemDto>();
}
