namespace Invensa.Domain.Dtos;

public sealed class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Code { get; init; } = default!;
    public Guid UnitId { get; init; }
    public string UnitName { get; init; } = default!;
    public decimal PriceSale { get; init; }
    public decimal PriceBuy { get; init; }

    // NUEVO: id de la imagen asociada (si existe)
    public Guid? ImageId { get; init; }
    public string RowVersion { get; init; } = default!;
}

public sealed class ProductStockDto
{
    public Guid ProductId { get; init; }
    public decimal Stock { get; init; }
}
