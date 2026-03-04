namespace Invensa.Domain.Entities;

public enum MovementType : byte
{
    In = 1,   // entradas
    Out = 2,   // salidas
    Adj = 3    // ajustes (+/-)
}

public class InventoryMovement
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public MovementType MovementType { get; set; }

    // Para IN/OUT (debe ser >0). NULO cuando es ADJ
    public decimal? Quantity { get; set; }

    // Para ADJ (puede ser + o -). NULO cuando es IN/OUT
    public decimal? QuantityAdj { get; set; }

    public decimal? UnitPrice { get; set; }     // precio de referencia
    public Guid? SaleId { get; set; }           // link a una venta (en salidas por venta)
    public string? Note { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public Product Product { get; set; } = default!;
    public Sale? Sale { get; set; }
}
