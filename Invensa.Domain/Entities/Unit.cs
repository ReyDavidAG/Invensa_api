namespace Invensa.Domain.Entities;

public class Unit
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public byte[] RowVersion { get; set; } = default!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
