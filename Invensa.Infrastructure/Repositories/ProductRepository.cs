using System.Security.Cryptography;
using Invensa.Domain.Dtos;
using Invensa.Domain.Entities;
using Invensa.Domain.Interfaces;
using Invensa.Domain.Models.Responses;
using Invensa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invensa.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ILogger<Repository<Product>> _logger;
    private readonly InvensaDbContext _ctx;
    private readonly IImageStorage _storage; // NUEVO

    public ProductRepository(
        InvensaDbContext context,
        ILogger<Repository<Product>> logger,
        IImageStorage storage) // NUEVO
        : base(context, logger)
    {
        _logger = logger;
        _ctx = context;
        _storage = storage;
    }

    public async Task<PaginatedResult<ProductDto>> GetProductsByParametersAsync(
      CancellationToken ct, int page, int pageSize, string? search = null, Guid? unitId = null)
    {
        var q = _ctx.Set<Product>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(p => p.Name.Contains(s) || p.Code.Contains(s));
        }
        if (unitId.HasValue)
            q = q.Where(p => p.UnitId == unitId.Value);

        var total = await q.CountAsync(ct);
        var skip = (page - 1) * pageSize;

        var items = await q.AsNoTracking()
            .Include(p => p.Unit)
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                UnitId = p.UnitId,
                UnitName = p.Unit.Name,
                PriceSale = p.PriceSale,
                PriceBuy = p.PriceBuy,
                ImageId = p.ImageId,
                RowVersion = Convert.ToBase64String(p.RowVersion) // ✅
            })
            .ToListAsync(ct);

        return new PaginatedResult<ProductDto> { Items = items, Page = page, PageSize = pageSize, Total = total };
    }

    public async Task<ProductDto?> GetByIdAsync(CancellationToken ct, Guid id)
    {
        return await _ctx.Set<Product>().AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                UnitId = p.UnitId,
                UnitName = p.Unit.Name,
                PriceSale = p.PriceSale,
                PriceBuy = p.PriceBuy,
                ImageId = p.ImageId,
                RowVersion = Convert.ToBase64String(p.RowVersion) // ✅
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PaginatedResult<ProductDto>> GetByFiltersAsync(
        CancellationToken ct, int page, int pageSize, string? search = null, Guid? unitId = null)
    {
        var q = Entities.AsNoTracking().Include(p => p.Unit).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(p => p.Name.Contains(s) || p.Code.Contains(s));
        }
        if (unitId.HasValue) q = q.Where(p => p.UnitId == unitId.Value);

        var total = await q.CountAsync(ct);
        var skip = (page - 1) * pageSize;

        var items = await q.OrderBy(p => p.Name)
            .Skip(skip).Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                UnitId = p.UnitId,
                UnitName = p.Unit.Name,
                PriceSale = p.PriceSale,
                PriceBuy = p.PriceBuy,
                ImageId = p.ImageId,
                RowVersion = Convert.ToBase64String(p.RowVersion) // ✅
            })
            .ToListAsync(ct);

        return new PaginatedResult<ProductDto> { Items = items, Page = page, PageSize = pageSize, Total = total };
    }

    public async Task<decimal> GetStockAsync(CancellationToken ct, Guid productId)
    {
        var moves = await _ctx.Set<InventoryMovement>().AsNoTracking()
            .Where(m => m.ProductId == productId)
            .Select(m => new { m.MovementType, m.Quantity, m.QuantityAdj })
            .ToListAsync(ct);

        decimal stock = 0m;
        foreach (var m in moves)
        {
            switch (m.MovementType)
            {
                case MovementType.In: stock += m.Quantity ?? 0m; break;
                case MovementType.Out: stock -= m.Quantity ?? 0m; break;
                case MovementType.Adj: stock += m.QuantityAdj ?? 0m; break;
            }
        }
        return Math.Round(stock, 4);
    }

    public async Task<bool> IsCodeTakenAsync(string code, CancellationToken ct, Guid? excludeId = null)
    {
        code = code.Trim().ToUpper();
        return await Entities.AnyAsync(p => p.Code == code && (excludeId == null || p.Id != excludeId), ct);
    }

    // ===================== Manejo de Imagen =====================

    public async Task<Guid> UpsertProductImageAsync(
    Guid productId,
    string fileName,
    string contentType,
    string base64,
    long sizeBytes,
    string? sha256Opt,
    CancellationToken ct)
    {
        // 1) Decodifica y hashea
        byte[] bytes;
        try { bytes = Convert.FromBase64String(base64); }
        catch (FormatException) { throw new ArgumentException("Base64 inválido."); }

        if (bytes.LongLength != sizeBytes)
            _logger.LogWarning("SizeBytes difiere de bytes.Length para product {ProductId}", productId);

        var sha256 = sha256Opt;
        if (string.IsNullOrWhiteSpace(sha256))
        {
            using var hasher = SHA256.Create();
            sha256 = Convert.ToHexString(hasher.ComputeHash(bytes));
        }

        // 2) ¿Ya existe la misma imagen? Reusar por SHA
        var existing = await _ctx.Set<ImageFile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Sha256 == sha256, ct);

        Guid imageId;
        if (existing is not null)
        {
            imageId = existing.Id;  // reusar
        }
        else
        {
            // 3) Nueva imagen: guarda archivo + inserta fila
            var ext = GetSafeExtension(fileName);
            imageId = Guid.NewGuid();
            var storagePath = BuildStoragePath(imageId, ext);

            await _storage.SaveAsync(storagePath, bytes, ct);

            var img = new ImageFile
            {
                Id = imageId,
                OriginalFileName = fileName,
                ContentType = contentType,
                SizeBytes = sizeBytes,
                Sha256 = sha256!,
                StoragePath = storagePath,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _ctx.Set<ImageFile>().AddAsync(img, ct);
        }

        // 4) Vincular al producto en el MISMO contexto
        var product = _ctx.Set<Product>().Local.FirstOrDefault(p => p.Id == productId)
                   ?? await _ctx.Set<Product>().FirstOrDefaultAsync(p => p.Id == productId, ct);

        if (product is null)
            throw new InvalidOperationException("Producto no encontrado para asociar imagen.");

        product.ImageId = imageId;

        // Si está detach, attach y marca SOLO ImageId como modificado
        var entry = _ctx.Entry(product);
        if (entry.State == EntityState.Detached)
        {
            _ctx.Attach(product);
            entry.Property(p => p.ImageId).IsModified = true;
        }

        return imageId;
    }


    public async Task RemoveProductImageAsync(Guid productId, CancellationToken ct)
    {
        var product = await _ctx.Set<Product>().FirstOrDefaultAsync(p => p.Id == productId, ct);
        if (product == null) return;

        if (product.ImageId.HasValue)
        {
            var imageId = product.ImageId.Value;
            product.ImageId = null;

            var entry = _ctx.Entry(product);
            if (entry.State == EntityState.Detached)
            {
                _ctx.Attach(product);
                entry.Property(p => p.ImageId).IsModified = true;
            }

            var img = await _ctx.Set<ImageFile>().FirstOrDefaultAsync(i => i.Id == imageId, ct);
            if (img != null)
            {
                await _storage.DeleteAsync(img.StoragePath, ct);
                _ctx.Set<ImageFile>().Remove(img);
            }
        }
    }
    private static string GetSafeExtension(string fileName)
    {
        var dot = fileName.LastIndexOf('.');
        if (dot < 0 || dot == fileName.Length - 1) return ".bin";
        var ext = fileName.Substring(dot).ToLowerInvariant();
        return ext.Length <= 10 ? ext : ".bin";
    }

    private static string BuildStoragePath(Guid id, string ext)
    {
        var now = DateTime.UtcNow;
        // relativo a la raíz de storage (lo resuelve IImageStorage)
        return $"images/{now:yyyy}/{now:MM}/{id}{ext}";
    }
}
