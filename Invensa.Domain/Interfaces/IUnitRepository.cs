using Invensa.Domain.Dtos;
using Invensa.Domain.Entities;
using Invensa.Domain.Models.Responses;

namespace Invensa.Domain.Interfaces;

public interface IUnitRepository : IRepository<Unit>
{
    Task<PaginatedResult<UnitDto>> GetUnitsByParametersAsync(
        CancellationToken ct,
        int page, int pageSize,
        string? search = null    // busca por Code/Name
    );

    Task<UnitDto?> GetByIdAsync(CancellationToken ct, Guid id);
}
