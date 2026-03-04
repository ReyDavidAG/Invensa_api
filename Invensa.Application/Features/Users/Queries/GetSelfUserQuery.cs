using Invensa.Domain.Entities;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Users.Queries;

public class GetSelfUserQuery : IRequest<Result<User>>
{
    public string? Id { get; set; }
}
