using System.Net.Mime;
using Invensa.Domain.Interfaces;
using Microsoft.Extensions.Hosting;               // <-- IHostEnvironment
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Invensa.Infrastructure.Services;

/// <summary>
/// Opciones de almacenamiento de imágenes.
/// BaseRootRelative: carpeta base relativa al ContentRoot (default: "wwwroot").
/// </summary>
public sealed class ImageStorageOptions
{
    public string BaseRootRelative { get; set; } = "wwwroot";
}

public sealed class FileSystemImageStorage : IImageStorage
{
    private readonly string _rootPath;
    private readonly ILogger<FileSystemImageStorage> _logger;

    // Usamos IHostEnvironment para NO depender de ASP.NET (IWebHostEnvironment).
    public FileSystemImageStorage(
        IHostEnvironment env,
        ILogger<FileSystemImageStorage> logger,
        IOptions<ImageStorageOptions> opts)
    {
        _logger = logger;

        var baseRootRelative = opts.Value.BaseRootRelative?.Trim();
        if (string.IsNullOrWhiteSpace(baseRootRelative))
            baseRootRelative = "wwwroot";

        _rootPath = Path.Combine(env.ContentRootPath, baseRootRelative);
        Directory.CreateDirectory(_rootPath);
    }

    public async Task SaveAsync(string relativePath, byte[] bytes, CancellationToken ct)
    {
        var fullPath = CombineRelative(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await File.WriteAllBytesAsync(fullPath, bytes, ct);
    }

    public async Task<(byte[] Bytes, string ContentType)?> ReadAsync(string relativePath, CancellationToken ct)
    {
        var fullPath = CombineRelative(relativePath);
        if (!File.Exists(fullPath)) return null;

        var bytes = await File.ReadAllBytesAsync(fullPath, ct);
        var contentType = GuessContentType(fullPath);
        return (bytes, contentType);
    }

    public Task<bool> DeleteAsync(string relativePath, CancellationToken ct)
    {
        var fullPath = CombineRelative(relativePath);
        if (!File.Exists(fullPath)) return Task.FromResult(false);
        File.Delete(fullPath);
        return Task.FromResult(true);
    }

    private string CombineRelative(string relative)
        => Path.Combine(_rootPath, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string GuessContentType(string fullPath)
    {
        var ext = Path.GetExtension(fullPath).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            _ => MediaTypeNames.Application.Octet
        };
    }
}
