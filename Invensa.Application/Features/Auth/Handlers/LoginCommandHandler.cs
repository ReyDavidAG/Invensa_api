using Invensa.Application.DTOs.Auth;
using Invensa.Application.Features.Auth.Commands;
using LanguageExt.Common;
using Invensa.Domain.Entities;
using Invensa.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Invensa.Application.Features.Auth.Validators;
using FluentValidation;
using Invensa.Domain.Exceptions;

namespace Invensa.Application.Features.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var validator = new LoginCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if(!validationResult.IsValid)
        {
            _logger.LogWarning("Login validation failed for email {Email}: {Errors}", request.Email, validationResult.Errors);
            var validationException = new FluentValidation.ValidationException(validationResult.Errors);

            return new Result<AuthResponse>(validationException);
        }

        var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);
            

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
            return new Result<AuthResponse>(new BadRequestException("Email o contraseña incorrectos"));
        }

        if (!user.Active)
        {
            _logger.LogWarning("Login failed: User inactive for email {Email}", request.Email);
            return new Result<AuthResponse>(new UnauthorizedAccessException("Usuario inactivo"));
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: Invalid password for email {Email}", request.Email);
            return new Result<AuthResponse>(new BadRequestException("Email o contraseña incorrectos"));
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshTokenString = _jwtTokenService.GenerateRefreshToken();
        var expiresAt = _jwtTokenService.GetTokenExpiration();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAtUtc = expiresAt.AddDays(7), // Refresh token válido por 7 días
            CreatedAtUtc = DateTime.UtcNow,
            IsRevoked = false
        };

        await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: refreshTokenString,
            ExpiresAt: expiresAt,
            User: new UserInfo(user.Id, user.Email, user.Name, user.IsAdmin, user.Active)
        );

        _logger.LogInformation("Login successful for user {UserId}", user.Id);
        return new Result<AuthResponse>(response);
    }
}
