// Invensa.Domain/Entities/ImageFile.cs
namespace Invensa.Domain.Entities;

public sealed class ImageFile
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long SizeBytes { get; set; }
    public string Sha256 { get; set; } = default!;
    public string StoragePath { get; set; } = default!;
    public DateTime CreatedAtUtc { get; set; }  // DB default: SYSUTCDATETIME()
}
