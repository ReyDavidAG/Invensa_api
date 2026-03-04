// Application/Features/Inventory/Handlers/CreateProductCommandHandler.cs
using Invensa.Application.Features.Inventory.Commands;
using Invensa.Domain.Entities;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IUnitOfWork uow, ILogger<CreateProductCommandHandler> logger)
    { _uow = uow; _logger = logger; }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (!await _uow.UnitRepository.AnyAsync(u => u.Id == request.UnitId, ct))
            return new Result<Guid>(new ValidationException("La unidad no existe."));

        if (await _uow.ProductRepository.AnyAsync(p => p.Code == request.Code.Trim().ToUpper(), ct))
            return new Result<Guid>(new ValidationException("El código de producto ya existe."));

        var now = DateTime.UtcNow;

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpper(),
            UnitId = request.UnitId,
            PriceSale = request.PriceSale,
            PriceBuy = request.PriceBuy,
            CreatedAtUtc = now
        };

        await _uow.ProductRepository.AddAsync(product, ct);

        // Imagen en el MISMO request (opcional)
        if (request.Image is not null)
        {
            var imgId = await _uow.ProductRepository.UpsertProductImageAsync(
                product.Id,
                request.Image.FileName,
                request.Image.ContentType,
                request.Image.Base64,
                request.Image.SizeBytes,
                request.Image.Sha256,
                ct);

            // Solo asigna en memoria. NO Update(product).
            product.ImageId = imgId;
        }

        // Stock inicial (opcional)
        if (request.InitialQuantity is > 0)
        {
            var unitPrice = request.InitialUnitPrice ?? request.PriceBuy;

            var moveIn = new InventoryMovement
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                MovementType = MovementType.In,
                Quantity = request.InitialQuantity,
                UnitPrice = unitPrice,
                Note = string.IsNullOrWhiteSpace(request.Note) ? "Alta inicial" : request.Note,
                CreatedAtUtc = now
            };
            await _uow.InventoryMovementRepository.AddAsync(moveIn, ct);
        }

        var ok = await _uow.SaveChangesAsync(ct);
        if (!ok) return new Result<Guid>(new InfrastructureException("No se pudo guardar el producto."));

        return product.Id;
    }
}
