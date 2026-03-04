using Invensa.Application.Features.Users.Queries;
using Invensa.Domain.Entities;
using MediatR;
using LanguageExt.Common;
using Invensa.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Invensa.Application.Features.Users.Handlers;

public class GetUsersByParametersQueryHandler : IRequestHandler<GetUsersByParametersQuery, Result<IEnumerable<User>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUsersByParametersQueryHandler> _logger;

    public GetUsersByParametersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUsersByParametersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<User>>> Handle(GetUsersByParametersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting request to retrieve users by parameters");

        var users = await _unitOfWork.UserRepository
            .GetUsersByParams(cancellationToken, active: request.Active);

        return new Result<IEnumerable<User>>(users);
    }
}
