namespace Invensa.Domain.Dtos;

public sealed class ImageUploadDto
{
    // Nombre original del archivo (para logs/UI)
    public string FileName { get; init; } = default!;
    // MIME type (ej. image/jpeg, image/png)
    public string ContentType { get; init; } = default!;
    // Tamaño en bytes del archivo
    public long SizeBytes { get; init; }
    // Contenido en Base64 (sin data:uri prefix)
    public string Base64 { get; init; } = default!;
    // Opcional: si ya calculaste el hash en front, lo puedes enviar
    public string? Sha256 { get; init; }
}
