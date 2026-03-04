using System.Threading;

namespace Invensa.Domain.Interfaces;

public interface IImageStorage
{
    Task SaveAsync(string relativePath, byte[] bytes, CancellationToken ct);
    Task<(byte[] Bytes, string ContentType)?> ReadAsync(string relativePath, CancellationToken ct);
    Task<bool> DeleteAsync(string relativePath, CancellationToken ct);
}
