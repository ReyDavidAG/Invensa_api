using Invensa.Domain.Dtos;
using Invensa.Domain.Entities;
using Invensa.Domain.Interfaces;
using Invensa.Domain.Models.Responses;
using Invensa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invensa.Infrastructure.Repositories;

public sealed class UnitRepository : Repository<Unit>, IUnitRepository
{
    private readonly InvensaDbContext _ctx;

    public UnitRepository(InvensaDbContext ctx, ILogger<Repository<Unit>> logger)
        : base(ctx, logger)
    {
        _ctx = ctx;
    }

    public async Task<PaginatedResult<UnitDto>> GetUnitsByParametersAsync(
        CancellationToken ct,
        int page, int pageSize,
        string? search = null)
    {
        var q = _ctx.Set<Unit>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(u => u.Code.Contains(s) || u.Name.Contains(s));
        }

        var total = await q.CountAsync(ct);
        var skip = (page - 1) * pageSize;

        var items = await q.AsNoTracking()
            .OrderBy(u => u.Name)
            .Skip(skip).Take(pageSize)
            .Select(u => new UnitDto
            {
                Id = u.Id,
                Code = u.Code,
                Name = u.Name
            })
            .ToListAsync(ct);

        return new PaginatedResult<UnitDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<UnitDto?> GetByIdAsync(CancellationToken ct, Guid id)
    {
        return await _ctx.Set<Unit>().AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UnitDto
            {
                Id = u.Id,
                Code = u.Code,
                Name = u.Name
            })
            .FirstOrDefaultAsync(ct);
    }
}
