namespace Invensa.Application.Features.Users.Queries;

using MediatR;
using LanguageExt.Common;
using Invensa.Domain.Entities;

public class GetUsersByParametersQuery : IRequest<Result<IEnumerable<User>>>
{
    public bool? Active { get; set; }
}
