using Invensa.Domain.Dtos;
using Invensa.Domain.Entities;
using Invensa.Domain.Models.Responses;

namespace Invensa.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<PaginatedResult<ProductDto>> GetProductsByParametersAsync(
        CancellationToken ct,
        int page, int pageSize,
        string? search = null,
        Guid? unitId = null);

    Task<ProductDto?> GetByIdAsync(CancellationToken ct, Guid id);

    Task<decimal> GetStockAsync(CancellationToken ct, Guid productId);

    Task<PaginatedResult<ProductDto>> GetByFiltersAsync(
        CancellationToken ct,
        int page, int pageSize,
        string? search = null,
        Guid? unitId = null);

    Task<bool> IsCodeTakenAsync(string code, CancellationToken ct, Guid? excludeId = null);

    // ---------- NUEVO: manejo de imagen vinculada ----------
    /// <summary>
    /// Crea o reemplaza la imagen de un producto. Devuelve el ImageId asignado.
    /// </summary>
    Task<Guid> UpsertProductImageAsync(
        Guid productId,
        string fileName,
        string contentType,
        string base64,
        long sizeBytes,
        string? sha256Opt,
        CancellationToken ct);

    /// <summary>
    /// Quita la referencia de imagen del producto y (opcionalmente) elimina el registro de ImageFile huérfano.
    /// </summary>
    Task RemoveProductImageAsync(Guid productId, CancellationToken ct);
}
