using Invensa.Domain.Dtos;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Products.Queries;

public sealed class GetProductStockQuery : IRequest<Result<ProductStockDto>>
{
    public Guid ProductId { get; init; }
}
