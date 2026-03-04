using Invensa.Application.Features.Products.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Products.Handlers;

public sealed class GetProductStockQueryHandler
    : IRequestHandler<GetProductStockQuery, Result<ProductStockDto>>
{
    private readonly IUnitOfWork _uow;

    public GetProductStockQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<ProductStockDto>> Handle(GetProductStockQuery request, CancellationToken ct)
    {
        var exists = await _uow.ProductRepository.AnyAsync(p => p.Id == request.ProductId, ct);
        if (!exists) return new Result<ProductStockDto>(new NotFoundException("Producto no existe."));

        var stock = await _uow.ProductRepository.GetStockAsync(ct, request.ProductId);

        return new Result<ProductStockDto>(new ProductStockDto
        {
            ProductId = request.ProductId,
            Stock = stock
        });
    }
}
