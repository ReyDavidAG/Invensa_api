using Invensa.Application.Features.Sales.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Sales.Handlers;

public sealed class GetSaleByIdQueryHandler
    : IRequestHandler<GetSaleByIdQuery, Result<SaleDto>>
{
    private readonly IUnitOfWork _uow;

    public GetSaleByIdQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<SaleDto>> Handle(GetSaleByIdQuery request, CancellationToken ct)
    {
        var sale = await _uow.SaleRepository.GetByIdAsync(ct, request.Id);
        if (sale is null) return new Result<SaleDto>(new NotFoundException("Venta no encontrada."));
        return new Result<SaleDto>(sale);
    }
}
