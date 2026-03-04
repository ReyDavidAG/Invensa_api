using Invensa.Domain.Entities;
using System.Security.Claims;

namespace Invensa.Infrastructure.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetTokenExpiration();
}
