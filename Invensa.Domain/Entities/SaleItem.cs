namespace Invensa.Domain.Entities;

public class SaleItem
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }    
    public decimal UnitPrice { get; set; }    
    public decimal Subtotal { get; private set; }

    public DateTime CreatedAtUtc { get; set; }
    public byte[] RowVersion { get; set; } = default!;

    public Sale Sale { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
