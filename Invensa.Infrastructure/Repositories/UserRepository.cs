namespace Invensa.Infrastructure.Repositories;

using Invensa.Domain.Entities;
using Invensa.Domain.Interfaces;
using Invensa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


public class UserRepository : Repository<User>, IUserRepository
{
    private readonly ILogger<Repository<User>> _logger;
    private readonly InvensaDbContext _ctx;

    public UserRepository(InvensaDbContext context, ILogger<Repository<User>> logger) : base(context, logger)
    {
        _logger = logger;
        _ctx = context;
    }

    public async Task<List<User>> GetUsersByParams(CancellationToken cancellationToken, bool? active = null)
    {
        var query = _ctx.Set<User>().AsQueryable();

        if (active != null)
        {
            _logger.LogInformation("Filtering {field} with value {value}", nameof(active), active);
            query = query.Where(user => user.Active == active.Value);
        }

        _logger.LogInformation("Retrieving users");

        var users = await query
            .AsNoTracking()
            .Select(user => new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Active = user.Active,
                IsAdmin = user.IsAdmin
            })
            .ToListAsync(cancellationToken);

        return users;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting user by email: {Email}", email);
        return await _ctx.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking if email exists: {Email}", email);
        return await _ctx.Set<User>()
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting user by id: {Id}", id);
        return await _ctx.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}
