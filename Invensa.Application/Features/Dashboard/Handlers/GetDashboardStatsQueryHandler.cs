using Invensa.Application.Features.Dashboard.Queries;
using Invensa.Application.Features.Dashboard.Validators;
using Invensa.Domain.Dtos;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Invensa.Application.Features.Dashboard.Handlers;

public sealed class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly IUnitOfWork _uow;

    public GetDashboardStatsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken ct)
    {
        var vr = await new GetDashboardStatsQueryValidator().ValidateAsync(request, ct);
        if (!vr.IsValid) return new Result<DashboardStatsDto>(new FluentValidation.ValidationException(vr.Errors));

        var fromUtc = request.FromUtc ?? DateTime.UtcNow.AddDays(-30);
        var toUtc = request.ToUtc ?? DateTime.UtcNow;

        // ═══════════════════════════════════════════════════════════
        // 1. Ventas e Ingresos (EXISTING)
        // ═══════════════════════════════════════════════════════════
        var salesQuery = _uow.SaleRepository.AsQueryable()
            .Where(s => s.DateUtc >= fromUtc && s.DateUtc <= toUtc);

        var totalSales = await salesQuery.SumAsync(s => (decimal?)s.Total, ct) ?? 0m;
        var totalOrders = await salesQuery.CountAsync(ct);
        var averageTicket = totalOrders > 0 ? totalSales / totalOrders : 0m;

        // ═══════════════════════════════════════════════════════════
        // 2. Utilidad (Venta - Costo) (EXISTING)
        // ═══════════════════════════════════════════════════════════
        var saleItemsQuery = _uow.SaleItemRepository.AsQueryable()
            .Where(si => si.Sale.DateUtc >= fromUtc && si.Sale.DateUtc <= toUtc);

        var totalProfit = await saleItemsQuery
            .SumAsync(si => (decimal?)(si.Quantity * (si.UnitPrice - si.Product.PriceBuy)), ct) ?? 0m;

        // ═══════════════════════════════════════════════════════════
        // 3. Top Productos (EXISTING)
        // ═══════════════════════════════════════════════════════════
        var topProducts = await saleItemsQuery
            .GroupBy(si => new { si.ProductId, si.Product.Name })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .ToListAsync(ct);

        // ═══════════════════════════════════════════════════════════
        // 4. Historial de Ventas por día (EXISTING)
        // ═══════════════════════════════════════════════════════════
        var salesHistory = await salesQuery
            .GroupBy(s => s.DateUtc.Date)
            .Select(g => new SalesByPeriodDto
            {
                Date = g.Key,
                Total = g.Sum(x => x.Total)
            })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        // ═══════════════════════════════════════════════════════════
        // 5. Stock Bajo (EXISTING)
        // ═══════════════════════════════════════════════════════════
        var products = await _uow.ProductRepository.AsQueryable()
            .Select(p => new { p.Id, p.Name, p.Code })
            .ToListAsync(ct);

        var lowStockList = new List<LowStockProductDto>();
        foreach (var p in products)
        {
            var stock = await _uow.ProductRepository.GetStockAsync(ct, p.Id);
            if (stock <= 5)
            {
                lowStockList.Add(new LowStockProductDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Code = p.Code,
                    CurrentStock = stock
                });
            }
        }

        // ═══════════════════════════════════════════════════════════
        // 6. TOP CLIENTES (NEW)
        // ═══════════════════════════════════════════════════════════
        var topClients = await salesQuery
            .Where(s => s.ClientId != null)
            .GroupBy(s => new { s.ClientId, s.Client!.Name, s.Client.Identifier })
            .Select(g => new TopClientDto
            {
                ClientId = g.Key.ClientId!.Value,
                ClientName = g.Key.Name,
                Identifier = g.Key.Identifier,
                OrderCount = g.Count(),
                TotalSpent = g.Sum(x => x.Total),
                AverageTicket = g.Sum(x => x.Total) / g.Count(),
                LastPurchaseUtc = g.Max(x => x.DateUtc),
                RevenueShare = totalSales > 0
                    ? Math.Round(g.Sum(x => x.Total) / totalSales * 100, 2)
                    : 0
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(10)
            .ToListAsync(ct);

        // ═══════════════════════════════════════════════════════════
        // 7. CRECIMIENTO vs PERIODO ANTERIOR (NEW)
        // ═══════════════════════════════════════════════════════════
        var periodLength = (toUtc - fromUtc).TotalDays;
        var prevFrom = fromUtc.AddDays(-periodLength);
        var prevTo = fromUtc.AddSeconds(-1);

        var prevSalesQuery = _uow.SaleRepository.AsQueryable()
            .Where(s => s.DateUtc >= prevFrom && s.DateUtc <= prevTo);

        var prevTotalSales = await prevSalesQuery.SumAsync(s => (decimal?)s.Total, ct) ?? 0m;
        var prevTotalOrders = await prevSalesQuery.CountAsync(ct);

        var revenueGrowth = prevTotalSales > 0
            ? Math.Round((totalSales - prevTotalSales) / prevTotalSales * 100, 2)
            : (totalSales > 0 ? 100m : 0m);

        var orderGrowth = prevTotalOrders > 0
            ? Math.Round((decimal)(totalOrders - prevTotalOrders) / prevTotalOrders * 100, 2)
            : (totalOrders > 0 ? 100m : 0m);

        var salesGrowth = new SalesGrowthDto
        {
            CurrentPeriodSales = totalSales,
            CurrentPeriodOrders = totalOrders,
            PreviousPeriodSales = prevTotalSales,
            PreviousPeriodOrders = prevTotalOrders,
            RevenueGrowthPercent = revenueGrowth,
            OrderGrowthPercent = orderGrowth
        };

        // ═══════════════════════════════════════════════════════════
        // 8. MEJORES MÁRGENES DE GANANCIA (NEW)
        // ═══════════════════════════════════════════════════════════
        var bestMarginProducts = await saleItemsQuery
            .GroupBy(si => new { si.ProductId, si.Product.Name, si.Product.PriceBuy })
            .Select(g => new
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice),
                TotalCost = g.Sum(x => x.Quantity * g.Key.PriceBuy)
            })
            .Where(x => x.TotalRevenue > 0)
            .OrderByDescending(x => (x.TotalRevenue - x.TotalCost) / x.TotalRevenue)
            .Take(5)
            .ToListAsync(ct);

        var bestMarginDtos = bestMarginProducts.Select(x => new ProfitMarginProductDto
        {
            ProductId = x.ProductId,
            ProductName = x.ProductName,
            QuantitySold = x.QuantitySold,
            TotalRevenue = x.TotalRevenue,
            TotalCost = x.TotalCost,
            TotalProfit = x.TotalRevenue - x.TotalCost,
            MarginPercent = Math.Round((x.TotalRevenue - x.TotalCost) / x.TotalRevenue * 100, 2)
        }).ToList();

        // ═══════════════════════════════════════════════════════════
        // 9. VELOCIDAD DE VENTAS (NEW)
        // ═══════════════════════════════════════════════════════════
        var daysInPeriod = Math.Max(1, (int)Math.Ceiling(periodLength));

        // Reutilizamos salesHistory ya calculado para peak/lowest
        var peakDay = salesHistory.OrderByDescending(x => x.Total).FirstOrDefault();
        var lowestDay = salesHistory.Where(x => x.Total > 0).OrderBy(x => x.Total).FirstOrDefault();

        // Contar órdenes del día pico
        var peakDayOrders = 0;
        if (peakDay != null)
        {
            peakDayOrders = await salesQuery
                .Where(s => s.DateUtc.Date == peakDay.Date)
                .CountAsync(ct);
        }

        var salesVelocity = new SalesVelocityDto
        {
            AverageDailySales = Math.Round(totalSales / daysInPeriod, 2),
            AverageDailyOrders = Math.Round((decimal)totalOrders / daysInPeriod, 2),
            PeakSalesDay = peakDay?.Date,
            PeakSalesDayTotal = peakDay?.Total ?? 0,
            PeakSalesDayOrders = peakDayOrders,
            LowestSalesDay = lowestDay?.Date,
            LowestSalesDayTotal = lowestDay?.Total ?? 0
        };

        // ═══════════════════════════════════════════════════════════
        // 10. SEGMENTACIÓN DE CLIENTES (NEW)
        // ═══════════════════════════════════════════════════════════
        var registeredOrders = await salesQuery.Where(s => s.ClientId != null).CountAsync(ct);
        var registeredRevenue = await salesQuery.Where(s => s.ClientId != null)
            .SumAsync(s => (decimal?)s.Total, ct) ?? 0m;
        var anonymousOrders = totalOrders - registeredOrders;
        var anonymousRevenue = totalSales - registeredRevenue;
        var uniqueClients = await salesQuery
            .Where(s => s.ClientId != null)
            .Select(s => s.ClientId)
            .Distinct()
            .CountAsync(ct);

        var clientSegment = new ClientSegmentDto
        {
            RegisteredClientOrders = registeredOrders,
            RegisteredClientRevenue = registeredRevenue,
            AnonymousOrders = anonymousOrders,
            AnonymousRevenue = anonymousRevenue,
            UniqueClients = uniqueClients,
            RegisteredRevenuePercent = totalSales > 0
                ? Math.Round(registeredRevenue / totalSales * 100, 2)
                : 0
        };

        // ═══════════════════════════════════════════════════════════
        // 11. VENTAS POR HORA DEL DÍA (NEW)
        // ═══════════════════════════════════════════════════════════
        var salesByHour = await salesQuery
            .GroupBy(s => s.DateUtc.Hour)
            .Select(g => new SalesByHourDto
            {
                Hour = g.Key,
                OrderCount = g.Count(),
                Total = g.Sum(x => x.Total)
            })
            .OrderBy(x => x.Hour)
            .ToListAsync(ct);

        // ═══════════════════════════════════════════════════════════
        // BUILD RESULT
        // ═══════════════════════════════════════════════════════════
        var result = new DashboardStatsDto
        {
            // Existing
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            TotalProfit = totalProfit,
            AverageTicket = averageTicket,
            TopSellingProducts = topProducts,
            SalesHistory = salesHistory,
            LowStockProducts = lowStockList.OrderBy(x => x.CurrentStock).Take(5).ToList(),

            // New Analytics
            TopClients = topClients,
            SalesGrowth = salesGrowth,
            BestMarginProducts = bestMarginDtos,
            SalesVelocity = salesVelocity,
            ClientSegment = clientSegment,
            SalesByHour = salesByHour
        };

        return new Result<DashboardStatsDto>(result);
    }
}

