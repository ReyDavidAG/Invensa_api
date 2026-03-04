using Invensa.Domain.Dtos;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Products.Queries;

public sealed class GetProductByIdQuery : IRequest<Result<ProductDto>>
{
    public Guid Id { get; init; }
}
