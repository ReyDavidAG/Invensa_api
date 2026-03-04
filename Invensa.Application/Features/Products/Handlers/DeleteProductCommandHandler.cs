using Invensa.Application.Features.Products.Commands;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Invensa.Application.Features.Products.Handlers;

// Delete handler
public sealed class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(IUnitOfWork uow, ILogger<DeleteProductCommandHandler> logger)
    {
        _uow = uow; _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteProductCommand req, CancellationToken ct)
    {
        var product = await _uow.ProductRepository.FindAsync(ct, req.Id);
        if (product is null) return new Result<bool>(new NotFoundException("Producto no existe."));

        var hasRefs = await _uow.InventoryMovementRepository
                            .AnyAsync(m => m.ProductId == req.Id, ct)
                    || await _uow.SaleItemRepository
                            .AnyAsync(si => si.ProductId == req.Id, ct);

        if (hasRefs) return new Result<bool>(
            new ValidationException("No se puede eliminar: tiene movimientos/ventas."));

        _uow.ProductRepository.Remove(product);
        var ok = await _uow.SaveChangesAsync(ct);
        if (!ok) return new Result<bool>(new InfrastructureException("No se pudo eliminar el producto."));
        return true;
    }
}

