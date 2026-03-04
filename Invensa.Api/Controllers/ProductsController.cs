using Invensa.Application.Features.Inventory.Commands;
using Invensa.Application.Features.Products.Commands;
using Invensa.Application.Features.Products.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Invensa.Api.Controllers;

[Authorize(Policy = "Admin")] // crear/alterar productos -> Admin
[Route("api/[controller]")]
public sealed class ProductsController : BaseApiController
{
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, "Id del nuevo producto", typeof(Guid))]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand cmd, CancellationToken ct)
    {
        var result = await Mediator.Send(cmd, ct);
        return result.ToOk(id => id);
    }

    // ProductsController.cs (PUT)
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductCommand body, CancellationToken ct)
    {
        // Re-creamos el comando para poder asignar Id (props init-only)
        var cmd = new UpdateProductCommand
        {
            Id = id,
            Name = body.Name,
            Code = body.Code,
            UnitId = body.UnitId,
            PriceSale = body.PriceSale,
            PriceBuy = body.PriceBuy,
            RowVersion = body.RowVersion,
            RemoveImage = body.RemoveImage,
            // ⬇️ usa el mismo DTO que ya definiste en el comando
            Image = body.Image // <- NADA de ImagePayload
        };

        var result = await Mediator.Send(cmd, ct);

        return result.Match<IActionResult>(
            _ => NoContent(),
            err => err switch
            {
                ValidationException => BadRequest(new { detail = err.Message }),
                BadRequestException => Conflict(new { detail = err.Message }),
                NotFoundException => NotFound(new { detail = err.Message }),
                InfrastructureException => StatusCode(500, new { detail = err.Message }),
                _ => StatusCode(500, new { detail = err.Message })
            }
        );
    }



    [HttpDelete("{id:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new DeleteProductCommand { Id = id }, ct);
        return result.ToOk(_ => _);
    }


    [HttpPost("entry")]
    [SwaggerResponse(StatusCodes.Status200OK, "Entrada registrada", typeof(bool))]
    public async Task<IActionResult> RegisterEntry([FromBody] RegisterEntryCommand cmd, CancellationToken ct)
    {
        var result = await Mediator.Send(cmd, ct);
        return result.ToOk(ok => ok);
    }

    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, "Products", typeof(PaginatedResult<ProductDto>))]
    public async Task<IActionResult> Get([FromQuery] GetProductsQuery query, CancellationToken ct)
    {
        var result = await Mediator.Send(query, ct);
        return result.ToOk(x => x);
    }

    [HttpGet("{id:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Producto", typeof(ProductDto))]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        => (await Mediator.Send(new GetProductByIdQuery { Id = id }, ct)).ToOk(r => r);

    [HttpGet("{id:guid}/stock")]
    [SwaggerResponse(StatusCodes.Status200OK, "Stock", typeof(ProductStockDto))]
    public async Task<IActionResult> GetStock([FromRoute] Guid id, CancellationToken ct)
        => (await Mediator.Send(new GetProductStockQuery { ProductId = id }, ct)).ToOk(r => r);
}
