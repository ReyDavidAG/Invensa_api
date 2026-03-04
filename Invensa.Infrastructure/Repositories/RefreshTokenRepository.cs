using Invensa.Domain.Entities;
using Invensa.Domain.Interfaces;
using Invensa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invensa.Infrastructure.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    private readonly ILogger<Repository<RefreshToken>> _logger;

    public RefreshTokenRepository(InvensaDbContext context, ILogger<Repository<RefreshToken>> logger) 
        : base(context, logger)
    {
        _logger = logger;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting refresh token by token string");
        return await Entities
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active tokens for user {UserId}", userId);
        return await Entities
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAtUtc > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking all tokens for user {UserId}", userId);
        
        var tokens = await Entities
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAtUtc = DateTime.UtcNow;
        }
    }
}
