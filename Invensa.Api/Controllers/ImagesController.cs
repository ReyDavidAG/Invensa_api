using Invensa.Domain.Entities;
using Invensa.Domain.Interfaces;
using Invensa.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Invensa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ImagesController : ControllerBase
{
    private readonly InvensaDbContext _ctx;
    private readonly IImageStorage _storage;

    public ImagesController(InvensaDbContext ctx, IImageStorage storage)
    {
        _ctx = ctx;
        _storage = storage;
    }

    /// <summary>
    /// Devuelve la imagen binaria por su Id (ImageFile.Id).
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous] // o pon la policy que necesites
    public async Task<IActionResult> GetImage([FromRoute] Guid id, CancellationToken ct)
    {
        var img = await _ctx.Set<ImageFile>().AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, ct);
        if (img is null) return NotFound();

        var blob = await _storage.ReadAsync(img.StoragePath, ct);
        if (blob is null) return NotFound();

        // Cache 1 día (ajústalo)
        Response.Headers.CacheControl = "public,max-age=86400,immutable";
        Response.Headers.ETag = $"W/\"{img.Sha256}\"";

        return File(blob.Value.Bytes, img.ContentType);
    }
}
