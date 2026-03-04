using Invensa.Domain.Dtos;
using Invensa.Domain.Models.Responses;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Clients.Queries;

public sealed class GetClientsQuery : IRequest<Result<PaginatedResult<ClientDto>>>
{
    public string? SearchTerm { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
    public bool? OnlyActive { get; init; } = true;
}
