using Invensa.Domain.Dtos;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Clients.Queries;

public sealed record GetClientByIdQuery(Guid Id) : IRequest<Result<ClientDto>>;
