using Invensa.Domain.Entities;

public class Sale
{
    public Guid Id { get; set; }
    public DateTime DateUtc { get; set; }
    public decimal Total { get; set; }
    public long TicketNumber { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public byte[] RowVersion { get; set; } = default!;

    public Guid? ClientId { get; set; }
    public Client? Client { get; set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    public ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
}
