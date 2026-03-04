namespace Invensa.Domain.Interfaces;

/// <summary>
/// Abstracción para notificar cambios en el Dashboard en tiempo real.
/// La implementación real usa SignalR, pero las capas inferiores no lo saben.
/// </summary>
public interface IDashboardNotifier
{
    /// <summary>
    /// Notifica a todos los clientes conectados que el Dashboard tiene datos nuevos.
    /// </summary>
    /// <param name="reason">Razón del cambio (ej: "SaleCreated", "StockUpdated")</param>
    /// <param name="ct">Token de cancelación</param>
    Task NotifyDashboardChangedAsync(string reason, CancellationToken ct = default);
}
