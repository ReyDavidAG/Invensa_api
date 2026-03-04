using Invensa.Domain.Entities;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Invensa.Application.Features.Inventory.Commands;

public sealed class RegisterEntryCommandHandler : IRequestHandler<RegisterEntryCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterEntryCommandHandler> _logger;

    public RegisterEntryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<RegisterEntryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(RegisterEntryCommand request, CancellationToken ct)
    {
        var product = await _unitOfWork.ProductRepository.FindAsync(ct, request.ProductId);
        if (product is null) return new Result<bool>(new ValidationException("El producto no existe."));

        var price = request.UnitPrice ?? product.PriceBuy;

        var move = new InventoryMovement
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            MovementType = MovementType.In,
            Quantity = request.Quantity,
            UnitPrice = price,
            Note = string.IsNullOrWhiteSpace(request.Note) ? "Entrada" : request.Note,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.InventoryMovementRepository.AddAsync(move, ct);

        var saved = await _unitOfWork.SaveChangesAsync(ct);
        if (!saved) return new Result<bool>(new InfrastructureException("No se pudo registrar la entrada."));

        return new Result<bool>(true);
    }
}
