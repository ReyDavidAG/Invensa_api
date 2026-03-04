using Invensa.Application.Features.Products.Queries;
using Invensa.Domain.Dtos;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Products.Handlers;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IUnitOfWork _uow;

    public GetProductByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var dto = await _uow.ProductRepository.GetByIdAsync(ct, request.Id);
        if (dto is null) return new Result<ProductDto>(new NotFoundException("Producto no encontrado."));
        return new Result<ProductDto>(dto);
    }
}
