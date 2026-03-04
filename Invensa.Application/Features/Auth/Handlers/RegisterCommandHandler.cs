using Invensa.Application.DTOs.Auth;
using Invensa.Application.Features.Auth.Commands;
using LanguageExt.Common;
using Invensa.Domain.Entities;
using Invensa.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Invensa.Application.Features.Auth.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<RegisterCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        if (await _unitOfWork.UserRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            _logger.LogWarning("Registration failed: Email already exists {Email}", request.Email);
            return new Result<AuthResponse>(new Exception("El email ya está registrado"));
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsAdmin = request.IsAdmin,
            Active = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshTokenString = _jwtTokenService.GenerateRefreshToken();
        var expiresAt = _jwtTokenService.GetTokenExpiration();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAtUtc = expiresAt.AddDays(7),
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

        _logger.LogInformation("Registration successful for user {UserId}", user.Id);
        return new Result<AuthResponse>(response);
    }
}
