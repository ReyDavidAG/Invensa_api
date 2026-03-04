using Invensa.Application.Features.Sales.Validators;
using Invensa.Domain.Entities;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invensa.Application.Features.Sales.Commands;

public sealed class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSaleCommandHandler> _logger;
    private readonly IDashboardNotifier _dashboardNotifier;

    public CreateSaleCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateSaleCommandHandler> logger,
        IDashboardNotifier dashboardNotifier)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dashboardNotifier = dashboardNotifier;
    }

    public async Task<Result<Guid>> Handle(CreateSaleCommand request, CancellationToken ct)
    {
        var vr = await new CreateSaleCommandValidator(_unitOfWork).ValidateAsync(request, ct);
        if (!vr.IsValid) return new Result<Guid>(new FluentValidation.ValidationException(vr.Errors));

        // 1) Trae productos (para precios)
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _unitOfWork.ProductRepository
            .WhereAsync(p => productIds.Contains(p.Id), ct, asNoTracking: true);
        var map = products.ToDictionary(p => p.Id);

        // 3) Crea sale + items + movimientos OUT (en una sola transacción SaveChanges)
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            DateUtc = request.DateUtc ?? DateTime.UtcNow,
            ClientId = request.ClientId,
            Total = 0m, // se recalculará con los items
            CreatedAtUtc = DateTime.UtcNow
        };
        await _unitOfWork.SaleRepository.AddAsync(sale, ct);

        decimal total = 0m;

        foreach (var i in request.Items)
        {
            var p = map[i.ProductId];
            var unitPrice = i.UnitPrice ?? p.PriceSale;

            var si = new SaleItem
            {
                Id = Guid.NewGuid(),
                SaleId = sale.Id,
                ProductId = p.Id,
                Quantity = i.Quantity,
                UnitPrice = unitPrice,
                CreatedAtUtc = DateTime.UtcNow
                // Subtotal se calcula por columna computada
            };
            await _unitOfWork.SaleItemRepository.AddAsync(si, ct);

            // Movimiento OUT
            var outMove = new InventoryMovement
            {
                Id = Guid.NewGuid(),
                ProductId = p.Id,
                MovementType = MovementType.Out,
                Quantity = i.Quantity,
                UnitPrice = unitPrice,
                SaleId = sale.Id,
                Note = "Venta",
                CreatedAtUtc = DateTime.UtcNow
            };
            await _unitOfWork.InventoryMovementRepository.AddAsync(outMove, ct);

            total += Math.Round(i.Quantity * unitPrice, 2);
        }

        sale.Total = total;

        var saved = await _unitOfWork.SaveChangesAsync(ct);
        if (!saved) return new Result<Guid>(new InfrastructureException("No se pudo guardar la venta."));

        // Notificar a los dashboards conectados en tiempo real
        await _dashboardNotifier.NotifyDashboardChangedAsync("SaleCreated", ct);

        return new Result<Guid>(sale.Id);
    }
}
