using Invensa.Domain.Dtos;
using Invensa.Domain.Entities;
using Invensa.Domain.Models.Responses;

namespace Invensa.Domain.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<ClientDto?> GetByIdAsync(CancellationToken ct, Guid id);
    Task<PaginatedResult<ClientDto>> GetByFiltersAsync(CancellationToken ct, int page, int pageSize, string? search = null, bool? onlyActive = null);
}
