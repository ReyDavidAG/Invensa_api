using Invensa.Api.Hubs;
using Invensa.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Invensa.Api.Services;

/// <summary>
/// Implementación de <see cref="IDashboardNotifier"/> usando SignalR.
/// Envía notificaciones al grupo "DashboardViewers" cuando hay cambios.
/// </summary>
public sealed class DashboardNotifier : IDashboardNotifier
{
    private const string DashboardGroup = "DashboardViewers";
    private readonly IHubContext<DashboardHub> _hubContext;

    public DashboardNotifier(IHubContext<DashboardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <inheritdoc />
    public async Task NotifyDashboardChangedAsync(string reason, CancellationToken ct = default)
    {
        await _hubContext.Clients
            .Group(DashboardGroup)
            .SendAsync("DashboardUpdated", new
            {
                Reason = reason,
                Timestamp = DateTime.UtcNow
            }, ct);
    }
}
