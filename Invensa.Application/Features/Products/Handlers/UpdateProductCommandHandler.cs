// Application/Features/Products/Handlers/UpdateProductCommandHandler.cs
using Invensa.Application.Features.Products.Commands;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IUnitOfWork uow, ILogger<UpdateProductCommandHandler> logger)
    { _uow = uow; _logger = logger; }

    public async Task<Result<bool>> Handle(UpdateProductCommand req, CancellationToken ct)
    {
        var product = await _uow.ProductRepository.FindAsync(ct, req.Id);
        if (product is null)
            return new Result<bool>(new NotFoundException("Producto no existe."));

        if (await _uow.ProductRepository.IsCodeTakenAsync(req.Code.Trim().ToUpperInvariant(), ct, excludeId: req.Id))
            return new Result<bool>(new ValidationException("El código ya existe."));

        // Concurrencia
        if (!product.RowVersion.SequenceEqual(req.RowVersion))
            return new Result<bool>(new BadRequestException("El producto fue modificado por otro usuario."));
        _uow.SetOriginalRowVersion(product, req.RowVersion);

        // Mutación
        product.Name = req.Name.Trim();
        product.Code = req.Code.Trim().ToUpperInvariant();
        product.UnitId = req.UnitId;
        product.PriceSale = req.PriceSale;
        product.PriceBuy = req.PriceBuy;
        product.UpdatedAtUtc = DateTime.UtcNow;

        // Imagen (en el MISMO request)
        if (req.RemoveImage)
        {
            await _uow.ProductRepository.RemoveProductImageAsync(product.Id, ct);
        }
        else if (req.Image is not null)
        {
            var newImgId = await _uow.ProductRepository.UpsertProductImageAsync(
                product.Id,
                req.Image.FileName,
                req.Image.ContentType,
                req.Image.Base64,
                req.Image.SizeBytes,
                req.Image.Sha256,
                ct);

            product.ImageId = newImgId; // ya trackeado; NO Update()
        }

        try
        {
            var ok = await _uow.SaveChangesAsync(ct);
            if (!ok) return new Result<bool>(new InfrastructureException("No se pudo guardar el producto."));
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrencia al actualizar Product {ProductId}", req.Id);
            return new Result<bool>(new BadRequestException("Concurrencia: el producto fue modificado por otro usuario."));
        }
    }
}
