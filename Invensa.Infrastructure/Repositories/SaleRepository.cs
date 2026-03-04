namespace Invensa.Infrastructure.Repositories;

// SaleRepository.cs
using Invensa.Domain.Dtos;
using Invensa.Domain.Entities;
using Invensa.Domain.Interfaces;
using Invensa.Domain.Models.Responses;
using Invensa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(InvensaDbContext ctx, ILogger<Repository<Sale>> logger) : base(ctx, logger) { }

    public async Task<PaginatedResult<SaleDto>> GetByFiltersAsync(
        CancellationToken ct, int page, int size, DateTime? fromUtc = null, DateTime? toUtc = null, long? ticket = null, Guid? clientId = null)
    {
        var q = Entities.AsNoTracking().AsQueryable();
        if (fromUtc.HasValue) q = q.Where(s => s.DateUtc >= fromUtc.Value);
        if (toUtc.HasValue) q = q.Where(s => s.DateUtc <= toUtc.Value);
        if (ticket.HasValue) q = q.Where(s => s.TicketNumber == ticket.Value);
        if (clientId.HasValue) q = q.Where(s => s.ClientId == clientId.Value);

        var total = await q.CountAsync(ct);
        var skip = (page - 1) * size;

        var items = await q.OrderByDescending(s => s.DateUtc)
            .Skip(skip).Take(size)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                DateUtc = s.DateUtc,
                TicketNumber = s.TicketNumber,
                Total = s.Total,
                ClientId = s.ClientId,
                ClientName = s.Client != null ? s.Client.Name : null,
                Items = s.Items.Select(i => new SaleItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal
                }).ToList()
            })
            .ToListAsync(ct);

        return new PaginatedResult<SaleDto>
        {
            Items = items,
            Page = page,
            PageSize = size,
            Total = total
        };
    }

    public async Task<SaleDto?> GetByIdAsync(CancellationToken ct, Guid id)
    {
        return await Entities.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                DateUtc = s.DateUtc,
                TicketNumber = s.TicketNumber,
                Total = s.Total,
                ClientId = s.ClientId,
                ClientName = s.Client != null ? s.Client.Name : null,
                Items = s.Items.Select(i => new SaleItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);
    }
}
