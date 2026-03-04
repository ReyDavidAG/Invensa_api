namespace Invensa.Domain.Interfaces;

using Invensa.Domain.Dtos;
using Invensa.Domain.Models.Responses;

public interface ISaleRepository : IRepository<Sale>
{
    Task<PaginatedResult<SaleDto>> GetByFiltersAsync(
        CancellationToken ct, int page, int size, DateTime? fromUtc = null, DateTime? toUtc = null, long? ticket = null, Guid? clientId = null);

    Task<SaleDto?> GetByIdAsync(CancellationToken ct, Guid id);
}

