namespace Invensa.Domain.Entities;

public class ImageFiles
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }
    public string Sha256 { get; set; } = null!;
    public string StoragePath { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}
