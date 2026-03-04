using Invensa.Application.DTOs.Auth;
using Invensa.Application.Features.Auth.Commands;
using LanguageExt.Common;
using Invensa.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Invensa.Application.Features.Auth.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refresh token attempt");

        var storedToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        
        if (storedToken == null)
        {
            _logger.LogWarning("Refresh token not found");
            return new Result<AuthResponse>(new Exception("Token inválido"));
        }

        if (storedToken.IsRevoked)
        {
            _logger.LogWarning("Refresh token revoked for user {UserId}", storedToken.UserId);
            return new Result<AuthResponse>(new Exception("Token revocado"));
        }

        if (storedToken.IsExpired)
        {
            _logger.LogWarning("Refresh token expired for user {UserId}", storedToken.UserId);
            return new Result<AuthResponse>(new Exception("Token expirado"));
        }

        var user = await _unitOfWork.UserRepository.GetByIdAsync(storedToken.UserId, cancellationToken);
        
        if (user == null || !user.Active)
        {
            _logger.LogWarning("User not found or inactive for refresh token");
            return new Result<AuthResponse>(new Exception("Usuario no válido"));
        }

        // Revocar el token anterior
        storedToken.IsRevoked = true;
        storedToken.RevokedAtUtc = DateTime.UtcNow;
        _unitOfWork.RefreshTokenRepository.Update(storedToken);

        // Generar nuevos tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshTokenString = _jwtTokenService.GenerateRefreshToken();
        var expiresAt = _jwtTokenService.GetTokenExpiration();

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshTokenString,
            ExpiresAtUtc = expiresAt.AddDays(7),
            CreatedAtUtc = DateTime.UtcNow,
            IsRevoked = false
        };

        await _unitOfWork.RefreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: newRefreshTokenString,
            ExpiresAt: expiresAt,
            User: new UserInfo(user.Id, user.Email, user.Name, user.IsAdmin, user.Active)
        );

        _logger.LogInformation("Token refreshed successfully for user {UserId}", user.Id);
        return new Result<AuthResponse>(response);
    }
}
