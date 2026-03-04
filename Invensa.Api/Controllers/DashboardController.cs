using Invensa.Application.Features.Dashboard.Queries;
using Invensa.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Invensa.Api.Controllers;

[Authorize(Policy = "Admin")]
[Route("api/[controller]")]
public sealed class DashboardController : BaseApiController
{
    [HttpGet("stats")]
    [SwaggerOperation(Summary = "Obtener estadísticas globales para el Dashboard")]
    [SwaggerResponse(StatusCodes.Status200OK, "Estadísticas calculadas", typeof(DashboardStatsDto))]
    public async Task<IActionResult> GetStats([FromQuery] GetDashboardStatsQuery query, CancellationToken ct)
    {
        var result = await Mediator.Send(query, ct);
        return result.ToOk(x => x);
    }
}
