using Invensa.Domain.Dtos;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Sales.Queries;

public sealed class GetSaleByIdQuery : IRequest<Result<SaleDto>>
{
    public Guid Id { get; init; }
}
