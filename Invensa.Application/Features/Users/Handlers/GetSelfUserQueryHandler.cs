namespace Invensa.Application.Features.Users.Handlers;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Services.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Queries;
using Validators;
using ValidationException = FluentValidation.ValidationException;

public class GetSelfUserQueryHandler : IRequestHandler<GetSelfUserQuery, Result<User>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly ILogger<GetSelfUserQueryHandler> _logger;

    public GetSelfUserQueryHandler(IUnitOfWork unitOfWork, IUserService userService, ILogger<GetSelfUserQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _logger = logger;
    }

    public async Task<Result<User>> Handle(GetSelfUserQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start request to retrieve self user information. ");
        var userResult = _userService.GetUser();

        if (userResult.IsFaulted)
        {
            _logger.LogError("Failed to retrieve user information: {Error}", userResult.Match(_ => null!, ex => ex).Message);
            return new Result<User>(new InfrastructureException(userResult.Match(_ => null!, ex => ex).Message));
        }
        var userAuthenticated = userResult.Match(u => u, ex => null!);
        request.Id = userAuthenticated?.UserId;

        var validator = new GetSelfUserQueryValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogInformation("User authenticated does not exist.");
            return new Result<User>(new ValidationException(validationResult.Errors));
        }
        _logger.LogInformation("Validation success. Retrieving user.");

        if (!Guid.TryParse(userAuthenticated?.UserId, out var userGuid))
        {
            _logger.LogError("Invalid user ID format: {UserId}", userAuthenticated?.UserId);
            return new Result<User>(new Exception("ID de usuario no válido"));
        }

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userGuid, cancellationToken);

        return new Result<User>(user!);
    }
}