using Invensa.Application.Features.Products.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Interfaces;
using Invensa.Domain.Models.Responses;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Invensa.Application.Features.Products.Handlers;

public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, Result<PaginatedResult<ProductDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(IUnitOfWork uow, ILogger<GetProductsQueryHandler> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ProductDto>>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var page = request.Page.GetValueOrDefault(1);
        var size = Math.Clamp(request.PageSize.GetValueOrDefault(50), 1, 200);

        var result = await _uow.ProductRepository.GetByFiltersAsync(
            ct, page, size, request.Search, request.UnitId);

        return new Result<PaginatedResult<ProductDto>>(result);
    }
}
