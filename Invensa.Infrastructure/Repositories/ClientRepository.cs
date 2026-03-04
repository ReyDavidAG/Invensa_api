using Invensa.Domain.Entities;
using Invensa.Domain.Dtos;
using Invensa.Domain.Interfaces;
using Invensa.Domain.Models.Responses;
using Invensa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invensa.Infrastructure.Repositories;

public class ClientRepository : Repository<Client>, IClientRepository
{
    private readonly InvensaDbContext _ctx;

    public ClientRepository(InvensaDbContext context, ILogger<Repository<Client>> logger) : base(context, logger)
    {
        _ctx = context;
    }

    public async Task<ClientDto?> GetByIdAsync(CancellationToken ct, Guid id)
    {
        return await _ctx.Set<Client>().AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Identifier = c.Identifier,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                Active = c.Active,
                CreatedAtUtc = c.CreatedAtUtc,
                RowVersion = Convert.ToBase64String(c.RowVersion)
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PaginatedResult<ClientDto>> GetByFiltersAsync(CancellationToken ct, int page, int pageSize, string? search = null, bool? onlyActive = null)
    {
        var q = _ctx.Set<Client>().AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(c => c.Name.ToLower().Contains(s) || (c.Identifier != null && c.Identifier.ToLower().Contains(s)) || (c.Email != null && c.Email.ToLower().Contains(s)));
        }

        if (onlyActive.HasValue)
        {
            q = q.Where(c => c.Active == onlyActive.Value);
        }

        var total = await q.CountAsync(ct);
        var skip = (page - 1) * pageSize;

        var items = await q.OrderBy(c => c.Name)
            .Skip(skip)
            .Take(pageSize)
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Identifier = c.Identifier,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                Active = c.Active,
                CreatedAtUtc = c.CreatedAtUtc,
                RowVersion = Convert.ToBase64String(c.RowVersion)
            })
            .ToListAsync(ct);

        return new PaginatedResult<ClientDto> { Items = items, Page = page, PageSize = pageSize, Total = total };
    }
}
