using Invensa.Domain.Entities;
namespace Invensa.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<List<User>> GetUsersByParams(
        CancellationToken cancellationToken,
        bool? active = null
    );
    
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
