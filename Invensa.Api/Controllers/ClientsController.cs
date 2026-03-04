using Invensa.Application.Features.Clients.Commands;
using Invensa.Application.Features.Clients.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Invensa.Api.Controllers;

[Authorize(Policy = "Admin")] // Ajustar política según necesidades
[Route("api/[controller]")]
public sealed class ClientsController : BaseApiController
{
    [HttpPost]
    [SwaggerOperation(Summary = "Crear un nuevo cliente")]
    [SwaggerResponse(StatusCodes.Status200OK, "Id del cliente creado", typeof(Guid))]
    public async Task<IActionResult> Create([FromBody] CreateClientCommand cmd, CancellationToken ct)
    {
        var result = await Mediator.Send(cmd, ct);
        return result.ToOk(id => id);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Obtener lista de clientes paginada")]
    [SwaggerResponse(StatusCodes.Status200OK, "Colección de clientes", typeof(PaginatedResult<ClientDto>))]
    public async Task<IActionResult> Get([FromQuery] GetClientsQuery q, CancellationToken ct)
    {
        var result = await Mediator.Send(q, ct);
        return result.ToOk(x => x);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Obtener un cliente por su Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Información del cliente", typeof(ClientDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Cliente no encontrado")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetClientByIdQuery(id), ct);
        return result.ToOk(x => x);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Actualizar un cliente existente")]
    [SwaggerResponse(StatusCodes.Status200OK, "Éxito de la actualización", typeof(bool))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Cliente no encontrado")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Conflicto de concurrencia")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateClientCommand cmd, CancellationToken ct)
    {
        if (id != cmd.Id) return BadRequest("El ID de la ruta no coincide con el ID del comando.");
        
        var result = await Mediator.Send(cmd, ct);
        return result.ToOk(x => x);
    }
}
