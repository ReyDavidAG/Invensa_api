using System.ComponentModel.DataAnnotations;

namespace Invensa.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }
    public string? Identifier { get; set; } // RFC, NIT, DNI, etc.
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public byte[] RowVersion { get; set; } = default!;

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
