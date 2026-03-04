// Invensa.Api/Controllers/SalesController.cs
using Invensa.Application.Features.Sales.Commands;
using Invensa.Application.Features.Sales.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Invensa.Api.Controllers;

[Authorize(Policy = "Admin")] // si quieres permitir a UserActive, cámbialo aquí
[Route("api/[controller]")]
public sealed class SalesController : BaseApiController
{
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, "Id de la venta", typeof(Guid))]
    public async Task<IActionResult> Create([FromBody] CreateSaleCommand cmd, CancellationToken ct)
    {
        var result = await Mediator.Send(cmd, ct);
        return result.ToOk(id => id);
    }
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, "Sales", typeof(PaginatedResult<SaleDto>))]
    public async Task<IActionResult> GetSales([FromQuery] GetSalesQuery q, CancellationToken ct)
    {
        var result = await Mediator.Send(q, ct);
        return result.ToOk(x => x);
    }

    [HttpGet("{id:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sale", typeof(SaleDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetSaleByIdQuery { Id = id }, ct);
        return result.ToOk(x => x);
    }
}
