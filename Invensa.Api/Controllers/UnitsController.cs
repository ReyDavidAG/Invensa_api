// Invensa.Api/Controllers/UnitsController.cs
using Invensa.Application.Features.Units.Commands;
using Invensa.Application.Features.Units.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Invensa.Api.Controllers;

[Authorize(Policy = "UserActive")]
[Route("api/[controller]")]
public sealed class UnitsController : BaseApiController
{
    // GET /api/units
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, "Listado paginado de unidades", typeof(PaginatedResult<UnitDto>))]
    public async Task<IActionResult> Get([FromQuery] GetUnitsQuery q, CancellationToken ct)
        => (await Mediator.Send(q, ct)).ToOk(r => r);

    // GET /api/units/{id}
    [HttpGet("{id:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Unidad", typeof(UnitDto))]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        => (await Mediator.Send(new GetUnitByIdQuery { Id = id }, ct)).ToOk(r => r);

    // POST /api/units  (Admin)
    [Authorize(Policy = "Admin")]
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, "Id de la nueva unidad", typeof(Guid))]
    public async Task<IActionResult> Create([FromBody] CreateUnitCommand cmd, CancellationToken ct)
        => (await Mediator.Send(cmd, ct)).ToOk(id => id);

    // PUT /api/units/{id}  (Admin)
    [Authorize(Policy = "Admin")]
    [HttpPut("{id:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Actualizado", typeof(bool))]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUnitCommand body, CancellationToken ct)
    {
        return (await Mediator.Send(body, ct)).ToOk(ok => ok);
    }
}
