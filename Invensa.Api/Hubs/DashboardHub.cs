using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Invensa.Api.Hubs;

/// <summary>
/// Hub de SignalR para notificaciones en tiempo real del Dashboard.
/// Los clientes se conectan aquí y reciben avisos cuando los datos cambian.
/// </summary>
[Authorize(Policy = "Admin")]
public sealed class DashboardHub : Hub
{
    private const string DashboardGroup = "DashboardViewers";

    /// <summary>
    /// Se ejecuta cuando un cliente se conecta al Hub.
    /// Lo agrega al grupo "DashboardViewers" para recibir notificaciones.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, DashboardGroup);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Se ejecuta cuando un cliente se desconecta del Hub.
    /// Lo remueve del grupo "DashboardViewers".
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, DashboardGroup);
        await base.OnDisconnectedAsync(exception);
    }
}
