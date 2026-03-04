namespace Invensa.Domain.Dtos;

public sealed class DashboardStatsDto
{
    // ── Existing ──
    public decimal TotalSales { get; init; }
    public int TotalOrders { get; init; }
    public decimal TotalProfit { get; init; }
    public decimal AverageTicket { get; init; }
    public List<TopProductDto> TopSellingProducts { get; init; } = new();
    public List<LowStockProductDto> LowStockProducts { get; init; } = new();
    public List<SalesByPeriodDto> SalesHistory { get; init; } = new();

    // ── Sales Analytics (NEW) ──

    /// <summary>Top clients ranked by total revenue in the period.</summary>
    public List<TopClientDto> TopClients { get; init; } = new();

    /// <summary>Growth comparison vs. the previous period of equal length.</summary>
    public SalesGrowthDto SalesGrowth { get; init; } = new();

    /// <summary>Products with the best profit margin percentage.</summary>
    public List<ProfitMarginProductDto> BestMarginProducts { get; init; } = new();

    /// <summary>Sales velocity / throughput metrics.</summary>
    public SalesVelocityDto SalesVelocity { get; init; } = new();

    /// <summary>Breakdown: registered-client sales vs anonymous sales.</summary>
    public ClientSegmentDto ClientSegment { get; init; } = new();

    /// <summary>Sales breakdown grouped by hour of day (0-23).</summary>
    public List<SalesByHourDto> SalesByHour { get; init; } = new();
}

// ── Existing DTOs (unchanged) ─────────────────────────────────────

public sealed class TopProductDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal QuantitySold { get; init; }
    public decimal TotalRevenue { get; init; }
}

public sealed class LowStockProductDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public decimal CurrentStock { get; init; }
}

public sealed class SalesByPeriodDto
{
    public DateTime Date { get; init; }
    public decimal Total { get; init; }
}

// ── NEW Analytics DTOs ────────────────────────────────────────────

/// <summary>Represents a top client with aggregated metrics.</summary>
public sealed class TopClientDto
{
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string? Identifier { get; init; }
    public int OrderCount { get; init; }
    public decimal TotalSpent { get; init; }
    public decimal AverageTicket { get; init; }
    public DateTime LastPurchaseUtc { get; init; }
    /// <summary>% of total revenue this client represents.</summary>
    public decimal RevenueShare { get; init; }
}

/// <summary>Compares current period vs previous period of equal length.</summary>
public sealed class SalesGrowthDto
{
    public decimal CurrentPeriodSales { get; init; }
    public int CurrentPeriodOrders { get; init; }
    public decimal PreviousPeriodSales { get; init; }
    public int PreviousPeriodOrders { get; init; }
    /// <summary>Percentage change in revenue. Positive = growth.</summary>
    public decimal RevenueGrowthPercent { get; init; }
    /// <summary>Percentage change in order count. Positive = growth.</summary>
    public decimal OrderGrowthPercent { get; init; }
}

/// <summary>Product ranked by profit margin percentage.</summary>
public sealed class ProfitMarginProductDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal TotalRevenue { get; init; }
    public decimal TotalCost { get; init; }
    public decimal TotalProfit { get; init; }
    /// <summary>Margin % = (Revenue - Cost) / Revenue * 100</summary>
    public decimal MarginPercent { get; init; }
    public decimal QuantitySold { get; init; }
}

/// <summary>Sales throughput / velocity metrics.</summary>
public sealed class SalesVelocityDto
{
    public decimal AverageDailySales { get; init; }
    public decimal AverageDailyOrders { get; init; }
    public DateTime? PeakSalesDay { get; init; }
    public decimal PeakSalesDayTotal { get; init; }
    public int PeakSalesDayOrders { get; init; }
    public DateTime? LowestSalesDay { get; init; }
    public decimal LowestSalesDayTotal { get; init; }
}

/// <summary>Segmentation: registered clients vs anonymous (walk-in).</summary>
public sealed class ClientSegmentDto
{
    public int RegisteredClientOrders { get; init; }
    public decimal RegisteredClientRevenue { get; init; }
    public int AnonymousOrders { get; init; }
    public decimal AnonymousRevenue { get; init; }
    /// <summary>Number of unique registered clients who purchased.</summary>
    public int UniqueClients { get; init; }
    /// <summary>% of revenue from registered clients.</summary>
    public decimal RegisteredRevenuePercent { get; init; }
}

/// <summary>Sales aggregated by hour of day for heat-map / bar charts.</summary>
public sealed class SalesByHourDto
{
    public int Hour { get; init; }
    public int OrderCount { get; init; }
    public decimal Total { get; init; }
}
