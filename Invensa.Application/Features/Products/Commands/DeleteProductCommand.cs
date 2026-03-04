using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Products.Commands;

public sealed class DeleteProductCommand : IRequest<Result<bool>>
{
    public Guid Id { get; init; }
}

