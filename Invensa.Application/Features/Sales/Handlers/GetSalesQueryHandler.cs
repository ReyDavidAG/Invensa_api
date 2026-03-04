using Invensa.Application.Features.Sales.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Interfaces;
using Invensa.Domain.Models.Responses;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Sales.Handlers;

public sealed class GetSalesQueryHandler
    : IRequestHandler<GetSalesQuery, Result<PaginatedResult<SaleDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetSalesQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<SaleDto>>> Handle(GetSalesQuery request, CancellationToken ct)
    {
        var page = request.Page.GetValueOrDefault(1);
        var size = Math.Clamp(request.PageSize.GetValueOrDefault(50), 1, 200);

        var data = await _uow.SaleRepository.GetByFiltersAsync(
            ct, page, size, request.FromUtc, request.ToUtc, request.TicketNumber, request.ClientId);

        return new Result<PaginatedResult<SaleDto>>(data);
    }
}
